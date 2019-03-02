using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.JKControl
{
    public class JKControlAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "JKControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "JKControl_default",
                "JKControl/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}