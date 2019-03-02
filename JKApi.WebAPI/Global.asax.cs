using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;
using JKApi.Service.Service.ManagerClass;

namespace JKApi.WebAPI
{
    /// <summary>
    /// WebApiApplication
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        #region Life Cycle

        /// <summary>
        /// This method is invoked when the API application starts.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
            JsonConfig.Configure();

            DataManager.Instance.StartRepeatingProcess();
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
        }

        /// <summary>
        /// This method is invoked when the API application ends.
        /// </summary>
        protected void Application_End()
        {
            
        }

        #endregion

    }
}
