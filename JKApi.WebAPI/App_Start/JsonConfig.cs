using System.Web.Http;
using Newtonsoft.Json;

namespace JKApi.WebAPI
{
    /// <summary>
    /// JsonConfig
    /// </summary>
    public class JsonConfig
    {
        /// <summary>
        /// General configuration for all the JSON serialization.
        /// </summary>
        public static void Configure()
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
            };
        }
    }
}