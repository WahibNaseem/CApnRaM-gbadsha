using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http.Routing;

namespace JKApi.WebAPI
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Register the configuration for the Api application.
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            var constraintResolver = new DefaultInlineConstraintResolver() { ConstraintMap = {["apiVersion"] = typeof(ApiVersionRouteConstraint) } };
            config.AddApiVersioning(o => o.ReportApiVersions = true);
            config.MapHttpAttributeRoutes(constraintResolver);
        }
    }
}
