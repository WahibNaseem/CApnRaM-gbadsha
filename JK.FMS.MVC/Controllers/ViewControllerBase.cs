using JK.FMS.MVC;
using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.ServiceContract.AccountPayable;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.CustomerInvoice;
using JKApi.Service.ServiceContract.Franchisee;
using JKApi.Service.ServiceContract.Inspection;
using JKApi.Service.ServiceContract.JKControl;
using JKApi.Service.ServiceContract.Management;
using JKViewModels;
using JKViewModels.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AjaxForm.Serialization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;

namespace Application.Web.Core


{
    
    public class ViewControllerBase : Controller
    {
        //These are common for all
        protected MailService _mailService = MailService.Instance;
        protected IEncryptDecrypt _encryptDecrypt;
        protected ClaimView _claimView = ClaimView.Instance;
        private NLogger _nLogger = NLogger.Instance;

        
        public jkDatabaseEntities jkEntityModel = jkDatabaseEntities.Instance;
        //public JKApi.Data.JkControl.jkControlEntities jkControlEntites = JKApi.Data.JkControl.jkControlEntities.Instance;
        //public JKApi.Data.DAL.jkDatabaseEntities jkEntityModel = new JKApi.Data.DAL.jkDatabaseEntities();
        //public JKApi.Data.JkControl.jkControlEntities jkControlEntites = new JKApi.Data.JkControl.jkControlEntities();


        protected IUserService _userService;
        protected ICompanyService _companyService;
        protected ICustomerService CustomerService;
        protected ICustomerInvoiceService _customerinvoiceservice;
        protected ICacheProvider _cacheProvider;
        protected IManagementService ManagementService;
        protected IFranchiseeService FranchiseeService;
        //protected IRegionService RegionService;
        protected IAccountReceivableService AccountReceivableService;
        protected ICommonService _commonService;
        protected IAccountPayableService AccountPayableService;
        protected ICRM_Service _crmService;
        protected IInspectionService _inspectionService;

        #region Constructor

        public ViewControllerBase()
        {
            //if (String.IsNullOrEmpty(CurrentUserIdentityName))
            //{
            //    RedirectToAction("index", "User", new { area = "JKControl" });
            //    return;
            //}
        }
      

        protected string CurrentUserIdentityName
        {
            get
            {
                return System.Web.HttpContext.Current == null
                           ? String.Empty
                           : System.Web.HttpContext.Current.User.Identity.Name;
            }
        }

        protected string CurrentUserIdentityShortName
        {
            get
            {
                return System.Web.HttpContext.Current == null
                            ? String.Empty
                            : GetShortUserName(CurrentUserIdentityName);
            }
        }

        #endregion

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            const string culture = "en-US";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        //Change because login issue:: Gaurav M. :31/12/2017 

        #region intercept action and check session exist or not...
        public AppSession appSession;
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var descriptor = filterContext.ActionDescriptor;
        //    var actionName = descriptor.ActionName;
        //    var controllerName = descriptor.ControllerDescriptor.ControllerName;
        //    var sessionOutUrl = string.Empty;

        //    if (base.Session[AppSetting.UserSession] != null)
        //    {
        //        appSession = CurrentSession.GetCurrentSession();

        //        if (controllerName == "User" && actionName == "Index")
        //        {
        //            filterContext.Result = new RedirectResult("~/Portal/DashBoard/Region");
        //        }
        //    }
        //    else
        //    {
        //        sessionOutUrl = "/" + controllerName + "/" + actionName;
        //        base.Session[AppSetting.UserSession] = null;
        //        if (controllerName != "User")
        //        {
        //            filterContext.Result = new RedirectResult("~/JKControl/User?returnUrl="+ sessionOutUrl);
        //        }
        //    }

        //    //Session passed to client side
        //    ViewBag.SessionOutUrl = sessionOutUrl;
        //    ViewBag.Session = appSession;
        //    base.OnActionExecuting(filterContext);
        //}

        #endregion


        public int LoginUserId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_claimView.GetCLAIM_USERID()))
                {
                    return Convert.ToInt32(_claimView.GetCLAIM_USERID());
                }
                else
                    return 0;
            }
        }

        public int SelectedRegionId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_claimView.GetCLAIM_SELECTED_COMPANY_ID()))
                {
                    return Convert.ToInt32(_claimView.GetCLAIM_SELECTED_COMPANY_ID());
                }
                else
                    return 0;
            }
        }

        public int SelectedUserId
        {
            get
            {
                var roles = _claimView.GetCLAIM_ROLELIST();
                if (roles != null && roles.Count > 0)
                {
                    if (roles.FindAll(x => x.RoleTypeName == RoleType.Admin.ToString() || x.RoleTypeName == RoleType.SuperAdmin.ToString()).Count > 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return LoginUserId;
                    }
                }
                else
                    return LoginUserId;
            }
        }


        public string GetLocalIPAddress()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        /// <summary>
        /// In controllers that catch their own exceptions, call this to send to the error view
        /// </summary>
        /// <param name="errorMsg">A <see cref="String"/> representing the error message</param>
        /// <returns><see cref="System.Web.Mvc.RedirectToRouteResult"/> routes to an error view</returns>
        protected PartialViewResult HandleErrorConditionPartial(string errorMsg)
        {
            return PartialView("~/Views/Error/ExceptionError.cshtml", errorMsg);
        }

        /// <summary>
        /// In controllers that catch their own exceptions, call this to send to the error view
        /// </summary>
        /// <param name="errorMsg">A <see cref="String"/> representing the error message</param>
        /// <returns><see cref="System.Web.Mvc.RedirectToRouteResult"/> routes to an error view</returns>
        protected ActionResult HandleErrorCondition(string errorMsg)
        {
            return RedirectToAction("ExceptionError", "Error", new { @area = "", msg = errorMsg });
        }



        // TODO : Refactor.  Why is there an indexer being created on Controllers that tries to return a session object?
        public object this[string index]
        {
            get
            {
                return HttpContext.Session[index];
            }
            set
            {
                HttpContext.Session[index] = value;
            }
        }


        public long PrimaryKey
        {
            get
            {
                return ViewBag.PrimaryKey;
            }
            set
            {
                ViewBag.PrimaryKey = value;
            }
        }

        protected NLogger NLogger
        {
            get
            {
                return _nLogger;
            }

            set
            {
                _nLogger = value;
            }
        }

        public string GetShortUserName(string longName)
        {
            int stop = longName.IndexOf("\\", System.StringComparison.Ordinal);
            return (stop > -1) ? longName.Substring(stop + 1, longName.Length - stop - 1) : longName;
        }

        #region Get Bulk XML
        public static string GetBulkXML<T>(T data)
        {

            XmlSerializer xs = new XmlSerializer(typeof(T));

            XDocument d = new XDocument();

            using (XmlWriter xw = d.CreateWriter())
            {
                xs.Serialize(xw, data);

            }
            return Convert.ToString(d);
        }

        #endregion


        #region GetLatlng
        public RootObjectlatlngViewModel GetLatLongByAddress(string address)
        {
            var root = new RootObjectlatlngViewModel();
            //string.Format("https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(AddSTR) + "&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";
            try
            {
                var url =
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8", address);
                var req = (HttpWebRequest)WebRequest.Create(url);

                var res = (HttpWebResponse)req.GetResponse();

                using (var streamreader = new StreamReader(res.GetResponseStream()))
                {
                    var result = streamreader.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        root = JsonConvert.DeserializeObject<RootObjectlatlngViewModel>(result);
                    }
                }
            }
            catch (Exception)
            {

            }
            return root;


        }
        #endregion


        public ActionResult NewtonSoftJsonResult(object data)
        {
            return new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult CreateModelStateErrors()
        {
            StringBuilder errorSummary = new StringBuilder();
            errorSummary.Append(@"<div class=""validation-summary-errors"" data-valmsg-summary=""true""><ul>");
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            foreach (var error in errors)
            {
                errorSummary.Append("<li>" + error.ErrorMessage + "</li>");
            }
            errorSummary.Append("</ul></div>");
            return Content(errorSummary.ToString());
        }
    }
}
