using Application.Web.Core;

using JKViewModels;
using JKViewModels.Common;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Net;
using JKApi.Core;
using System.Threading;
using JK.Resources;
using JKViewModels.JKControl;
using System.Reflection;
using System.Data;
using JKApi.Service;
using JKApi.Service.ServiceContract.Outlook;
using System.Configuration;
using JKViewModels.User;
using LinqToExcel.Extensions;
using WebGrease.Css.Ast;
//using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JK.FMS.MVC.Models.Users;

namespace JK.FMS.MVC.Areas.JKControl.Controllers
{
    public class UserController : ViewControllerBase
    {
        private readonly IOutlookService _outlookService;
        private static readonly Uri response;

        // GET: JKControl/User
        public UserController(IUserService userService, IOutlookService outlookService, IEncryptDecrypt encryptDecrypt)
        {
            this._userService = userService;
            this._outlookService = outlookService;
            this._encryptDecrypt = encryptDecrypt;
        }

        #region Login/LogOFF
        [HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            RemoveSettings();

            UserLoginViewModel userLoginViewModel = new UserLoginViewModel();
            ViewBag.ReturnUrl = returnUrl;
            if (Request.Cookies["RememberMe"] != null)
            {
                userLoginViewModel.Username = _encryptDecrypt.Decrypt(Request.Cookies["RememberMe"].Values["UserName"].ToString());
                userLoginViewModel.Password = _encryptDecrypt.Decrypt(Request.Cookies["RememberMe"].Values["Password"]?.ToString());
                userLoginViewModel.RememberMe = true;
            }

            return View(userLoginViewModel);
        }


        public void GetAllAction()
        {
            // Only Action Names
            List<MenuModel> lstControllerActions = new List<MenuModel>();
            var allActionNames = from aName in Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
            .SelectMany(type => type.GetMethods())
            .Where(method => method.IsPublic && method.ReturnType == typeof(ActionResult))
                                 select aName;//filter actions;


            // Controller and Action Names
            var allController = from controllers in Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(Controller).IsAssignableFrom(type))
                                select controllers;
            foreach (var controller in allController)
            {
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    if (method.ReturnType == typeof(ActionResult))
                    {
                        lstControllerActions.Add(new MenuModel()
                        {
                            MenuUrl = controller.Name + "/" + method.Name
                        });
                    }
                }
            }

            DataTable dt = ToDataTable(lstControllerActions);
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public ActionResult Index(string returnUrl = "")
        {
            RemoveSettings();

            UserLoginViewModel userLoginViewModel = new UserLoginViewModel();
            ViewBag.ReturnUrl = returnUrl;
            if (Request.Cookies["RememberMe"] != null)
            {
                userLoginViewModel.Username = _encryptDecrypt.Decrypt(Request.Cookies["RememberMe"].Values["UserName"].ToString());
                userLoginViewModel.Password = _encryptDecrypt.Decrypt(Request.Cookies["RememberMe"].Values["Password"]?.ToString());
                userLoginViewModel.RememberMe = true;
            }
            var regionlist = _userService.getRegion().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", 1);


            return View(userLoginViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Login(UserLoginViewModel inputUserModel, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                inputUserModel.ErrorMessage = CommonResource.msgModelStateError;
                return View("Index", inputUserModel);
            }

            string strPassowrd = _encryptDecrypt.Encrypt(inputUserModel.Password);

            inputUserModel.IPAddress = GetLocalIPAddress();
            UserViewModel userViewModel = _userService.Login(inputUserModel);

            //Proceed if no errors            
            if (userViewModel != null && userViewModel.objErrorModel != null && userViewModel.objErrorModel.Count == 0)
            {
                var defaultRegionId = userViewModel.DefaultRegionId;

                Session["UserData"] = userViewModel;
                //Session.Timeout = 720;
                System.Web.Security.FormsAuthentication.SetAuthCookie(inputUserModel.Username, false);
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    Session["SelectedRegionId"] = userViewModel.DefaultRegionId;
                    Session["SelectedPeriodId"] = userViewModel.lstPeriodAccess.Where(x => x.RegionId == userViewModel.DefaultRegionId)?.FirstOrDefault().PeriodId;
                }
                else
                {
                    Session["SelectedRegionId"] = userViewModel.Regions.FirstOrDefault().RegionId;
                    if (userViewModel.lstPeriodAccess.Where(r => r.RegionId == Convert.ToInt32(Session["SelectedRegionId"])).Count() > 0)
                        Session["SelectedPeriodId"] = userViewModel.lstPeriodAccess.Where(r => r.RegionId == SelectedRegionId).FirstOrDefault()?.PeriodId;
                    else
                        Session["SelectedPeriodId"] = 0;
                }



                Session["LoginTrackingId"] = userViewModel.LoginTrackingId;

                #region OLD Login Code due to some issue commenting this code
                ////If the user is already authenticated, then remove the claims
                //bool isAlreadyAuthenticated = false;
                //ClaimsIdentity newIdentityFromExisting = System.Threading.Thread.CurrentPrincipal.Identity as ClaimsIdentity;

                ////if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
                //if (newIdentityFromExisting != null && newIdentityFromExisting.Claims != null && newIdentityFromExisting.Claims.Count() > 1) //If existing identity exists and has multiple claims
                //{
                //    //Try removing all the claims from the existing identity
                //    Claim[] claimsArray = newIdentityFromExisting.Claims.ToArray();
                //    for (int index = 0; index < claimsArray.Count(); index++)
                //    {
                //        Claim claim = claimsArray[index];
                //        newIdentityFromExisting.TryRemoveClaim(claim);
                //    }

                //    //set isAlreadyAuthenticated as 'true'
                //    isAlreadyAuthenticated = true;
                //}

                ////Prepare the user claims identity and add user details
                //ClaimsIdentity userClaimsIdentity = !isAlreadyAuthenticated ? new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie) : newIdentityFromExisting;
                //userClaimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userViewModel.UserId.ToString()));
                //userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_USERNAME, userViewModel.UserName));

                //Session["UserData"] = userViewModel;


                ////Added person model into claim. 
                ////userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_PERSON_INFORMATION, JsonConvert.SerializeObject(userViewModel)));
                //userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_USERID, Convert.ToString(userViewModel.UserId)));

                //if (userViewModel.Regions != null && userViewModel.Regions.Count > 0)
                //{
                //    userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(!string.IsNullOrWhiteSpace(returnUrl) ? userViewModel.DefaultRegionId : userViewModel.Regions.FirstOrDefault().RegionId)));
                //    userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(!string.IsNullOrWhiteSpace(returnUrl) ? userViewModel.Regions.Where(x => x.RegionId == userViewModel.DefaultRegionId).FirstOrDefault() : userViewModel.Regions.FirstOrDefault())));
                //}

                //if (userViewModel.lstPeriodAccess != null && userViewModel.lstPeriodAccess.Count > 0)
                //{
                //    userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_PERIOD_ID, Convert.ToString(!string.IsNullOrWhiteSpace(returnUrl) ? (userViewModel.lstPeriodAccess.Where(x => x.RegionId == userViewModel.DefaultRegionId)?.FirstOrDefault().PeriodId) : (userViewModel.lstPeriodAccess.Where(x => x.RegionId == userViewModel.Regions.Where(y => y.RegionId > 0)?.FirstOrDefault().RegionId).FirstOrDefault().PeriodId))));
                //}

                //if (userViewModel.lstRolebasedARAccessDetailModel != null && userViewModel.lstRolebasedARAccessDetailModel.Count > 0)
                //{
                //    userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_AR_PERMISSION, JsonConvert.SerializeObject(userViewModel.lstRolebasedARAccessDetailModel)));
                //}

                ////if (userViewModel.Roles != null && userViewModel.Roles.Count > 0)
                ////{
                ////    //Claims for the user roles
                ////    foreach (var role in userViewModel.Roles)
                ////    {
                ////        userClaimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));

                ////        if (!userClaimsIdentity.Claims.Any(clm => clm.Type == ClaimTypeName.CLAIM_ROLE_TYPE && clm.Value == role.RoleName))
                ////            userClaimsIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_ROLE_TYPE, role.RoleName));
                ////    }
                ////}

                ////If not already authenticated, then generate a new ticket
                //if (!isAlreadyAuthenticated)
                //{
                //    AuthenticationProperties authProperties = new AuthenticationProperties();
                //    authProperties.IsPersistent = true;
                //    HttpContext.GetOwinContext().Authentication.SignIn(authProperties, userClaimsIdentity);
                //}
                ////Else update the existing ticket
                //else
                //{
                //    //Regenerate user identity
                //    HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(userClaimsIdentity), new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddHours(1) });
                //}

                ////Set the user's identity in the current thread for other requests to API on the same thread                
                //System.Threading.Thread.CurrentPrincipal =
                //   new GenericPrincipal(
                //       userClaimsIdentity,
                //       userViewModel.Roles.Select(role => role.RoleName).ToArray()
                //   );
                #endregion

                if (inputUserModel.RememberMe)
                {
                    HttpCookie cookie = new HttpCookie("RememberMe");
                    cookie.Values.Add("UserName", _encryptDecrypt.Encrypt(inputUserModel.Username));
                    cookie.Values.Add("Password", strPassowrd);
                    cookie.Expires = DateTime.Now.AddDays(Convert.ToInt32(WebConfigResource.RememberMeExpireIn_Days));
                    cookie.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("RememberMe");
                    cookie.Values.Add("UserName", string.Empty);
                    cookie.Values.Add("Password", string.Empty);
                    cookie.Expires = DateTime.Now.AddDays(-2);
                    Response.Cookies.Add(cookie);
                }

                // attempt to subscribe user to Outlook streaming notifications
                _outlookService.Subscribe(userViewModel.UserId);

                /* 
                 * Login to API, get api key and token
                 Eventually this can relpace the call to the local service 
                 German Sosa 11/12/2018 
                 */

        //        var apiCredentials = LoginToApi(inputUserModel.Username, inputUserModel.Password);



                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    if (returnUrl == "/Home/HView")
                    {
                        returnUrl = "/Portal/DashBoard";
                    }
                    return Redirect(returnUrl);
                }
                else
                {
                    /*
                     Changed the method to bypass the Regional Office selection page and pass the Default Region ID
                     German Sosa - 10/15/2018
                     */
                    //return RedirectToAction("RegionSelect", "DashBoard", new { area = "Portal", RegionId = defaultRegionId });

                    /* Changed back to old region selection screen per Peter to reflect petition from management to change it the way it was*/
                    return RedirectToAction("Region", "DashBoard", new { area = "Portal" });
                }
                //return RedirectToAction("Index", "JKHome", new { area = "JKControl" });
            }
            else
            {
                inputUserModel.ErrorMessage = userViewModel.objErrorModel[0].ErrorMessage;
            }
            return View("Index", inputUserModel);
        }

        static async Task<Uri> LoginToApi(string user, string password)
        {
            Dictionary<string, string> userCredentials = new Dictionary<string, string>
            {
                { "username", user },
                { "password", password }
            };

            var apiEndPoint = ConfigurationManager.AppSettings["apiEndpoint"].ToString();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.DownloadString(...);

                HttpResponseMessage responseApi = await client.PostAsJsonAsync(
                    apiEndPoint+ "/v1/authentication/login", userCredentials);
                responseApi.EnsureSuccessStatusCode();
                DetailedLoginResponseModel response = await responseApi.Content.ReadAsAsync<DetailedLoginResponseModel>();
            }

            //static HttpClient client = new HttpClient();


            // return URI of the created resource.
            return response;
        }

        public ActionResult LogOff()
        {

            var UserLogDetails = jkEntityModel.AuthLoginTrackings.Where(x => x.UserId == LoginUserId && x.LogoutDateTime == null).ToList();
            if (UserLogDetails != null && UserLogDetails.Count > 0)
            {
                foreach (var item in UserLogDetails)
                {
                    item.LogoutDateTime = DateTime.Now;
                    jkEntityModel.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    jkEntityModel.SaveChanges();
                }
            }

            RemoveSettings();


            return RedirectToAction("Index", "User", new { area = "JKControl" });
        }

        public void RemoveSettings()
        {
            Session.RemoveAll();
            Session.Abandon();

            //Sign out user from identity
            //HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        #endregion

        #region User List/Add/Edit/Details
        public ActionResult Save(string prm = "")
        {
            return View();
        }

        public ActionResult List()
        {
            UserViewListModel userViewListModel = new UserViewListModel();
            userViewListModel.CurrentPage = 1;
            userViewListModel.CurrentPage = Convert.ToInt32(WebConfigResource.ListPageSize);
            return View(userViewListModel);
        }

        public ActionResult ListUser(UserViewListModel userViewListModel)
        {
            return PartialView("_UserList", userViewListModel);
        }

        public ActionResult DeactivateUser(UserViewListModel userViewListModel)
        {
            return Json(new { Message = userViewListModel.Message, MessageType = userViewListModel.MessageType }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ActivateUser(UserViewListModel userViewListModel)
        {
            return Json(new { Message = userViewListModel.Message, MessageType = userViewListModel.MessageType }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteUser(UserViewListModel userViewListModel)
        {
            return Json(new { Message = userViewListModel.Message, MessageType = userViewListModel.MessageType }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult ResetUserInfo(string user)
        {
            List<string> stringList = user.Split(';').ToList();
            var date = DateTime.Now;
            string username = stringList[0];
            int regionID = Convert.ToInt32(stringList[1]);
            string email = stringList[2];

            var results = -1;

            forgetpasswordEmail(username, email);
            results = 1;
            return this.Json(results, JsonRequestBehavior.AllowGet);
        }

        private void forgetpasswordEmail(string username, string email)
        {
            try
            {
                var SendEmailcontext = System.Web.HttpContext.Current.Request;
                string body = "Forgot Password Detail-";
                if (SendEmailcontext.Url.Authority.StartsWith("localhost"))
                {
                    body += "Hello Admin,<br /><p>User Name - " + username + "</p><br /><p>Email Id - <b>" +
                            email + "</b></p>";
                }
                else
                {
                    body += "Hello Admin,<br /><p>User Name - " + username + "</p><br /><p>Email Id - <b>" +
                              email + "</b></p>";
                }
                string subject = "Forgot Password";
                _mailService.SendEmailAsync("pnguyen@janiking.com", body, subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        public JsonResult SetNewPassword(string NewPass)
        {
            var stringList = NewPass.Split(';').ToList();
            var pass = stringList[0].Trim();
            var tempPass = stringList[1].Trim();
            var mail = stringList[2].Trim();
            var data = _userService.GetUserDetailByEmail(mail);
            var results = 0;
            UserLoginViewModel inputUserModel = new UserLoginViewModel();
            if (data != null)
            {
                inputUserModel.Username = data.UserName;
                inputUserModel.Password = tempPass;
                inputUserModel.IPAddress = GetLocalIPAddress();
                UserViewModel userViewModel = _userService.Login(inputUserModel);

                //Proceed if no errors            
                if (userViewModel != null && userViewModel.objErrorModel != null && userViewModel.objErrorModel.Count == 0)
                {
                    UserDetailViewModel UserViewModel = new UserDetailViewModel();
                    UserViewModel.UserId = data.UserId;
                    UserViewModel.ActionType = "S";
                    UserViewModel = _userService.SaveUser(UserViewModel);

                    userViewModel.PasswordHash =
                    UserViewModel.ActionType = UserViewModel.UserId > 0 ? "U" : "I";
                    UserViewModel.Password = pass;
                    UserViewModel = _userService.SaveUser(UserViewModel);
                    results = 1;
                }
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ChangeRegion(int regionId)
        {
            /*
             * Used to reset the regional office from the header nav selection dropdown.
             * Updates and returns the available periods for the selected region
             * German Sosa - 10/13/2018
             */
            var userViewModel = _claimView.GetCLAIM_PERSON_INFORMATION();
            Session["SelectedPeriodId"] = userViewModel.lstPeriodAccess.Where(r => r.RegionId == regionId).FirstOrDefault()?.PeriodId;

            var regionPeriods = userViewModel.lstPeriodAccess.Where(x => x.RegionId == regionId).ToList();
            ViewBag.selectedRegionId = regionId;
            Session["SelectedRegionId"] = regionId;
            var SelectedRegionId = Convert.ToInt32(_claimView.GetCLAIM_SELECTED_COMPANY_ID());
            return Json(new { returnedData = regionPeriods }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePeriod(int periodId)
        {
            /*
             * Used to reset the selected period from the header nav selection dropdown.
             * Updates and returns the available periods for the selected region
             * German Sosa - 11/13/2018
             */
            Session["SelectedPeriodId"] = periodId;

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetCurrentView()
        {
            /*
             * For Future use as of 10/19/2018
             * Reloads the current view after change region ajax call.
             * Use Ajax .net Helpers??
             * German Sosa - 10/1//2018
             */
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult UserProfile()
        {
            UserViewModel oUserViewModel = _userService.GetUserDetail(LoginUserId);
            return View(oUserViewModel);
        }
        public ActionResult UpdateFMSTabPassword(string fmsoldpwd, string fmsnewpwd, string fmsconfmpwd)
        {
            string results = string.Empty;

            if (string.IsNullOrWhiteSpace(fmsoldpwd) || string.IsNullOrWhiteSpace(fmsnewpwd) || string.IsNullOrWhiteSpace(fmsconfmpwd))
            {
                results = "Please fill the all fields!!";
            }
            else if (fmsoldpwd == fmsnewpwd)
            {
                results = "Old & New Password Should not same!!";
            }
            else if (fmsnewpwd != fmsconfmpwd)
            {
                results = "New Password & Confirm Password not match!!";
            }

            if (string.IsNullOrWhiteSpace(results))
            {
                results = _userService.UserResetPassword("FMS", Convert.ToInt32(LoginUserId), fmsoldpwd, fmsnewpwd);
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateOutlookTabPassword(string otlnewpwd, string otlconfmpwd)
        {
            string results = string.Empty;

            if (string.IsNullOrWhiteSpace(otlnewpwd) || string.IsNullOrWhiteSpace(otlconfmpwd))
            {
                results = "Please fill the all fields!!";
            }
            else if (otlnewpwd != otlconfmpwd)
            {
                results = "New Password & Confirm Password not match!!";
            }

            if (string.IsNullOrWhiteSpace(results))
            {
                results = _userService.UserResetPassword("Outlook", Convert.ToInt32(LoginUserId), "", otlnewpwd);
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}