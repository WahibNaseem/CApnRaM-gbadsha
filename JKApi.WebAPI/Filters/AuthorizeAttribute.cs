using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using JKApi.Core.EncryptDecrypt;
using JKApi.WebAPI.Dtos.Account;
using Newtonsoft.Json;

namespace JKApi.WebAPI.Filters
{
    public class Authorized : System.Web.Http.AuthorizeAttribute
    {
        private readonly EncryptDecrypt _objEncryptDecrypt = new EncryptDecrypt();

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //base.OnAuthorization(actionContext);
            if (actionContext.Request.Headers.GetValues("token") != null && actionContext.Request.Headers.GetValues("api_key") != null)
            {
                // get value from header
                var authenticationToken = Convert.ToString(actionContext.Request.Headers.GetValues("token").FirstOrDefault());
                var apiKey = _objEncryptDecrypt.Decrypt(Convert.ToString(actionContext.Request.Headers.GetValues("api_key").FirstOrDefault()));

                var loginModel = JsonConvert.DeserializeObject<LoginResponseModel>(_objEncryptDecrypt.Decrypt(authenticationToken));

                if (loginModel.Id.ToString() == apiKey &&
                    Convert.ToInt32(apiKey) > 0 &&
                    loginModel.LoginDateTime.AddDays(Convert.ToInt32(ConfigurationSettings.AppSettings["TokenExpireDays"])) < DateTime.Now)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                    actionContext.Response.ReasonPhrase = "Token expired, please re-generate";
                    return;
                }
                if (loginModel.Id.ToString() == apiKey && Convert.ToInt32(apiKey) > 0)
                {
                    HttpContext.Current.Response.AddHeader("token", authenticationToken);
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "Authorized");
                }
                else
                {
                    HttpContext.Current.Response.AddHeader("authenticationToken", authenticationToken);
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                actionContext.Response.ReasonPhrase = "Please provide valid inputs";
            }
        }
    }


}