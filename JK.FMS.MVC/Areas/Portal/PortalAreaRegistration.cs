using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.Portal
{
    public class PortalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Portal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Portal_default",
                "Portal/{controller}/{action}/{id}",
                new { action = "Index", controller="Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}