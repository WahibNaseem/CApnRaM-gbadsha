using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Service;
using JKViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace JK.FMS.MVC
{
    public class LoggingFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        NLogger nLogger = NLogger.Instance;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            var routeData = filterContext.RequestContext.RouteData;
            string area = string.Empty;
            if (routeData.DataTokens["area"] != null)
                area = routeData.DataTokens["area"].ToString().ToLower();
            string controllerName = routeData.Values["controller"].ToString().ToLower();
            string actionName = routeData.Values["action"].ToString().ToLower();
            if (HttpContext.Current.Session["UserData"] == null && controllerName != "user" && controllerName != "error" && controllerName != "manage")
            {
                // First we clean the authentication ticket like always
                System.Web.Security.FormsAuthentication.SignOut();

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //filterContext.Result = RedirectToAction("index", "user", new { area = "JKControl" });
                    filterContext.Result = new JsonResult
                    {
                        Data = 501,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    string redirectTo = "~/JKControl/User/Index";
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.RawUrl))
                    {
                        redirectTo = string.Format("~/JKControl/User/Index", HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
                        filterContext.Result = new RedirectResult(redirectTo);
                        return;
                    }
                }
            }

            ClaimView _ClaimView = ClaimView.Instance;

            if (HttpContext.Current.Session["UserData"] != null)
            {
                if (_ClaimView.IsLoggedinTracking(Convert.ToInt32(_ClaimView.GetCLAIM_USERID()), _ClaimView.GetCLAIM_LoginTrackingId()) > 0)
                {
                    List<RoleAccessModel> menus = null;
                    var userViewModel = (UserViewModel)_ClaimView.GetCLAIM_PERSON_INFORMATION();
                    if (userViewModel != null && userViewModel.RoleAccesss != null)
                    {
                        menus = userViewModel.RoleAccesss;
                    }

                    if (!CheckChildAction(filterContext, menus))
                    {
                        //redirectToUnauthorize(filterContext, "UnAuthorize", "Error");
                    }

                    filterContext.HttpContext.Items["RequestTime"] = DateTime.Now;
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    HttpContext.Current.Session.Abandon();

                    string redirectTo = "~/JKControl/User/Index";
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.RawUrl))
                    {
                        redirectTo = string.Format("~/JKControl/User/Index", HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
                        filterContext.Result = new RedirectResult(redirectTo);
                        return;
                    }
                }
            }
        }

        bool CheckChildAction(ActionExecutingContext filterContext, List<RoleAccessModel> menus)
        {


            var routeData = filterContext.RequestContext.RouteData;

            string area = string.Empty;
            if (routeData.DataTokens["area"] != null)
                area = routeData.DataTokens["area"].ToString().ToLower();
            string controllerName = routeData.Values["controller"].ToString().ToLower();
            string actionName = routeData.Values["action"].ToString().ToLower();

            string CurrentURL = "/" + area + "/" + controllerName + "/" + actionName;
            if (actionName == "index")
                CurrentURL = "/" + area + "/" + controllerName;

            string RefUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
            ClaimView _ClaimView = ClaimView.Instance;
            bool idFound = false;
            List<MenuModel> lstMenu = getAllMenus();
            MenuModel itmMenu = lstMenu.Where(m => m.MenuUrl.ToLower().Contains(CurrentURL.ToLower())).FirstOrDefault();
            if (itmMenu != null && !String.IsNullOrEmpty(itmMenu.PageName))
            {
                //List<int> lstParentMenus = GetMenuIDList(itmMenu.PageName);
                //foreach (int iMenu in lstParentMenus)
                //{
                //    if (menus.Select(m => m.MenuId).Contains(iMenu))
                //    {
                //        _ClaimView.UpdateLoginTracking(Convert.ToInt32(_ClaimView.GetCLAIM_USERID()), _ClaimView.GetCLAIM_LoginTrackingId(), iMenu, RefUrl);
                //        idFound = true;
                //        break;
                //    }
                //}

                _ClaimView.UpdateLoginTracking(Convert.ToInt32(_ClaimView.GetCLAIM_USERID()), _ClaimView.GetCLAIM_LoginTrackingId(), itmMenu.MenuId, RefUrl);
                idFound = true;
            }
            return idFound;
        }

        public List<MenuModel> getAllMenus()
        {
            if (HttpRuntime.Cache["AllMenus"] == null)
            {
                UserService _menuBusiness = new UserService();
                List<MenuModel> lstMenus = new List<MenuModel>();
                List<ErrorModel> objError = new List<ErrorModel>();
                lstMenus = _menuBusiness.GetAllMenu().lstMenu;
                HttpRuntime.Cache.Insert("AllMenus", lstMenus, null, DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
                return lstMenus;
            }
            else
            {
                return (List<MenuModel>)HttpRuntime.Cache["AllMenus"];
            }

        }



        List<int> GetMenuIDList(string strMenuIDs)
        {
            List<int> lstMenuID = new List<int>();
            string[] aMenu = strMenuIDs.Split(",".ToCharArray());
            foreach (string str in aMenu)
            {
                int iMenu = 0;
                int.TryParse(str, out iMenu);
                if (iMenu > 0)
                    lstMenuID.Add(iMenu);
            }
            return lstMenuID;
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Items["ResponseTime"] = DateTime.Now;
            if (filterContext.Exception == null)
            {
                //  nLogger.InfoWeb(filterContext);
            }
        }

        void redirectToUnauthorize(ActionExecutingContext filterContext, string actionName, string controllerName, string area)
        {
            System.Web.Routing.RouteValueDictionary route = new System.Web.Routing.RouteValueDictionary();
            route.Add("action", actionName);
            route.Add("controller", controllerName);
            route.Add("area", area);
            if (actionName.ToLower() == "home")
                filterContext.Controller.TempData["UnauthorizedAccess"] = "You are not authorized to access this page.";
            filterContext.Result = new RedirectToRouteResult(route);
        }
    }
}