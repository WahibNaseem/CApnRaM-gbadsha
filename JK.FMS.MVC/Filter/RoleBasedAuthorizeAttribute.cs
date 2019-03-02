using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Security.Claims;
using System.Threading;
using JKApi.Core.Common;
using System.Web.Routing;

namespace JK.FMS.MVC.Filter
{
    public class RoleBasedAuthorizeAttribute : AuthorizeAttribute
    {
        ClaimView _claimView = ClaimView.Instance;


        #region Old ROlebasedCode
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    var url = filterContext.HttpContext.Request.Url;

        //    var userViewModel = (JKViewModels.UserViewModel)_claimView.GetCLAIM_PERSON_INFORMATION();
        //    List<JKViewModels.RoleAccessModel> AllMenus = null;

        //    if (userViewModel != null && userViewModel.RoleAccesss != null)
        //    {
        //        AllMenus = userViewModel.RoleAccesss;
        //        if (AllMenus.Where(x => x.MenuUrl == url.PathAndQuery).Any())
        //        {
        //            //var menu = AllMenus.Where(x => x.MenuUrl.Contains(url.PathAndQuery) && x.IsViewAccess == false).ToList();

        //            var menu = AllMenus.Where(x => x.MenuUrl.Trim() == url.PathAndQuery.Trim() && x.IsViewAccess == false).ToList();
        //            if (menu != null && menu.Count > 0)
        //            {
        //                filterContext.Result = new RedirectResult("/Portal/DashBoard/Region");
        //            }
        //        }
        //    }


        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        bool isAllowedAccess = true;
        //        //Check the user for accepted roles for this action

        //        //Code Commented as we are going to implement the Role based access dynamically through Menu Access Attribute.
        //        //if (!string.IsNullOrEmpty(Roles))
        //        //{
        //        //    string[] roles = Roles.Split(',');
        //        //    if (roles.Where(role => filterContext.HttpContext.User.IsInRole(role)).Any())
        //        //    {
        //        //        //Allow access: If the user is authenticated and is in required role for this URL
        //        //        isAllowedAccess = true;
        //        //    }
        //        //    else
        //        //        isAllowedAccess = false;
        //        //}

        //        //Allow access: If the user is authenticated
        //        //If the user is allowed access, then check for client id in session
        //        if (isAllowedAccess)
        //        {
        //            string accessedURL = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + filterContext.ActionDescriptor.ActionName;

        //            //If the page is other than client selection, then check for an existing client id
        //            if (string.IsNullOrEmpty(_claimView.GetCLAIM_SELECTED_COMPANY_ID()))
        //            {
        //                if (string.IsNullOrEmpty(_claimView.GetCLAIM_USERID()))
        //                {
        //                    redirectToLogin(filterContext);
        //                }
        //                else
        //                {
        //                    filterContext.Result = new RedirectResult("/Portal/DashBoard/Region");
        //                }
        //            }

        //            //allow access
        //        }
        //        else
        //            redirectToLogin(filterContext);
        //    }
        //    else
        //    {
        //        redirectToLogin(filterContext);
        //    }
        //}
        #endregion

        //private Logging objLogging = new Logging();
        /// <summary>
        /// Called on authorization of a request
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //var url = filterContext.HttpContext.Request.Url;

            //var userViewModel = (JKViewModels.UserViewModel)_claimView.GetCLAIM_PERSON_INFORMATION();
            //List<JKViewModels.RoleAccessModel> AllMenus = null;

            //if (userViewModel != null && userViewModel.RoleAccesss != null)
            //{
            //    AllMenus = userViewModel.RoleAccesss;
            //    if (AllMenus.Where(x => x.MenuUrl == url.PathAndQuery).Any())
            //    {
            //        //var menu = AllMenus.Where(x => x.MenuUrl.Contains(url.PathAndQuery) && x.IsViewAccess == false).ToList();

            //        var menu = AllMenus.Where(x => x.MenuUrl.Trim()==url.PathAndQuery.Trim() && x.IsViewAccess == false).ToList();
            //        if (menu != null && menu.Count > 0)
            //        {
            //            filterContext.Result = new RedirectResult("/Portal/DashBoard/Region");
            //        }
            //    }
            //}


            //if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    bool isAllowedAccess = true;
            //    //Check the user for accepted roles for this action

            //    //Code Commented as we are going to implement the Role based access dynamically through Menu Access Attribute.
            //    //if (!string.IsNullOrEmpty(Roles))
            //    //{
            //    //    string[] roles = Roles.Split(',');
            //    //    if (roles.Where(role => filterContext.HttpContext.User.IsInRole(role)).Any())
            //    //    {
            //    //        //Allow access: If the user is authenticated and is in required role for this URL
            //    //        isAllowedAccess = true;
            //    //    }
            //    //    else
            //    //        isAllowedAccess = false;
            //    //}

            //    //Allow access: If the user is authenticated
            //    //If the user is allowed access, then check for client id in session
            //    if (isAllowedAccess)
            //    {
            //        string accessedURL = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + filterContext.ActionDescriptor.ActionName;

            //        //If the page is other than client selection, then check for an existing client id
            //        if (string.IsNullOrEmpty(_claimView.GetCLAIM_SELECTED_COMPANY_ID()))
            //        {
            //            if (string.IsNullOrEmpty(_claimView.GetCLAIM_USERID()))
            //            {
            //                redirectToLogin(filterContext);
            //            }
            //            else
            //            {
            //                filterContext.Result = new RedirectResult("/Portal/DashBoard/Region");
            //            }
            //        }

            //        //allow access
            //    }
            //    else
            //        redirectToLogin(filterContext);
            //}
            //else
            //{
            //    redirectToLogin(filterContext);
            //}
        }

        /// <summary>
        /// This method is used to redirect to Login.
        /// </summary>
        /// <param name="filterContext"></param>
        void redirectToLogin(AuthorizationContext filterContext)
        {
            //handling for Ajax Error.
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                if (filterContext.HttpContext.Response.StatusCode != 403)
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 403;
                }
            }
            //handling for other pages.
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "User",
                                action = "Index",
                                area = "JKControl",
                                returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                            }));
                // returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                //filterContext.Result = new RedirectResult("/JKControl/User");
            }
        }



        /// <summary>
        /// Sets unauthorize result
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        private void SetUnauthorizedResult(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult("Unauthorized");
        }


    }
}