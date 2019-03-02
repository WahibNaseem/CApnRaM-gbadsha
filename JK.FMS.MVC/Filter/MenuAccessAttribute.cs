using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using System.Security.Claims;
using System.Threading;

using Newtonsoft.Json;
using JKViewModels;
using JKApi.Service;
using JKApi.Core.Common;

namespace JK.FMS.MVC.Filter
{
    public class MenuAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string CurrentURL = filterContext.HttpContext.Request.Url.AbsolutePath;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                ClaimView _ClaimView = ClaimView.Instance;
                List<RoleAccessModel> menus = null;

                var userViewModel = (UserViewModel)_ClaimView.GetCLAIM_PERSON_INFORMATION();
                if (userViewModel != null && userViewModel.RoleAccesss != null)
                {
                    menus = userViewModel.RoleAccesss;
                }

                if (!CheckChildAction(CurrentURL, menus))
                {
                    redirectToUnauthorize(filterContext, "UnAuthorize", "Error");
                }
            }
            else
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    if (filterContext.HttpContext.Response.StatusCode != 403)
                    {
                        filterContext.HttpContext.Response.Clear();
                        filterContext.HttpContext.Response.StatusCode = 403;
                    }
                }
                else
                {
                    redirectToUnauthorize(filterContext, "Login", "Account");
                }
            }
            base.OnActionExecuting(filterContext);
        }


        void redirectToUnauthorize(ActionExecutingContext filterContext, string actionName, string controllerName)
        {
            System.Web.Routing.RouteValueDictionary route = new System.Web.Routing.RouteValueDictionary();
            route.Add("action", actionName);
            route.Add("controller", controllerName);
            if (actionName.ToLower() == "home")
                filterContext.Controller.TempData["UnauthorizedAccess"] = "You are not authorized to access this page.";
            filterContext.Result = new RedirectToRouteResult(route);
        }

        bool CheckChildAction(string CurrentURL, List<RoleAccessModel> menus)
        {
            ClaimView _ClaimView = ClaimView.Instance;
            bool idFound = false;
            List<MenuModel> lstMenu = getAllMenus();
            MenuModel itmMenu = lstMenu.Where(m => m.MenuUrl.ToLower().Equals(CurrentURL.ToLower())).FirstOrDefault();
            if (itmMenu != null && !String.IsNullOrEmpty(itmMenu.PageName))
            {
                List<int> lstParentMenus = GetMenuIDList(itmMenu.PageName);
                foreach (int iMenu in lstParentMenus)
                {
                    if (menus.Select(m => m.MenuId).Contains(iMenu))
                    {
                        _ClaimView.UpdateLoginTracking(Convert.ToInt32(_ClaimView.GetCLAIM_USERID()), _ClaimView.GetCLAIM_LoginTrackingId(), iMenu, CurrentURL);
                        idFound = true;
                        break;
                    }
                }
            }
            return idFound;
        }


        #region Get all Menu

        public static List<MenuModel> getAllMenus()
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

        #endregion

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

        /// <summary>
        /// Get User Claim.
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public string GetUserClaim(string claimType)
        {
            if (!string.IsNullOrEmpty(claimType))
            {
                //Get the user's claims
                var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

                //If there are claims present, then get the type of client
                if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
                {
                    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == claimType.ToLower()).Select(clm => clm.Value).FirstOrDefault();
                }
            }

            return null;
        }
    }


}