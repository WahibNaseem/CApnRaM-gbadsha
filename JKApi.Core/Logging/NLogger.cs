#region Using namespaces.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Tracing;
using NLog;
using System.Net.Http;
using System.Text;
using JKApi.Core;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Configuration;
using System.Security.Claims;
using System.Threading;
using System.Collections.Specialized;
using Newtonsoft.Json;
using JK.Resources;
using JKViewModels.Common;

#endregion

namespace JKApi.Core
{
    public class NLogger
    {
        private static readonly Lazy<NLogger> Lazy = new Lazy<NLogger>(() => new NLogger());
        public static NLogger Instance => Lazy.Value;

        #region Private member variables.
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        //GEt User from claim
        private string userName = string.Empty;
        private string request = string.Empty;

        #endregion

        #region Private member variables By Web config.
        protected string LoggingDateFormate { get { return Convert.ToString(WebConfigResource.LoggingDateFormate); } }

        protected bool LoggingDebug { get { return Convert.ToBoolean(WebConfigResource.LoggingDebug); } }

        protected bool LoggingInfo { get { return Convert.ToBoolean(WebConfigResource.LoggingInfo); } }

        protected bool LoggingError { get { return Convert.ToBoolean(WebConfigResource.LoggingError); } }

        protected bool LoggingException { get { return Convert.ToBoolean(WebConfigResource.LoggingException); } }
        #endregion

        #region private member methods
        /// <summary>
        /// User Activity Log when LoggingDebug(Web.Config) Value is True
        /// </summary>
        /// <param name="userName">GET Logged user name </param>
        /// <param name="Text">Log Text</param>
        /// <param name="RequestUri">Request Action Path</param>
        protected void Debug(string userName, string Text, string RequestUri)
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Debug, "Exception", Text);
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = RequestUri;
            theEvent.Properties["RequestTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["ResponseTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["RegionId"] = 0; 
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            ClassLogger.Log(theEvent);
        }

        /// <summary>
        /// User Activity Log when LoggingInfo(Web.Config) Value is True
        /// </summary>
        /// <param name="userName">GET Logged user name </param>
        /// <param name="Text">Log Text</param>
        /// <param name="RequestUri">Request Action Path</param>
        protected void Info(string userName, string Text, string RequestUri, DateTime? RequestTime, DateTime? ResponseTime)
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Info, "Exception", Text);
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = RequestUri;
            theEvent.Properties["RequestTime"] = RequestTime == null ? DateTime.Now.ToString(LoggingDateFormate) : Convert.ToDateTime(RequestTime).ToString(LoggingDateFormate);
            theEvent.Properties["ResponseTime"] = ResponseTime == null ? DateTime.Now.ToString(LoggingDateFormate) : Convert.ToDateTime(ResponseTime).ToString(LoggingDateFormate);
            theEvent.Properties["RegionId"] = 0;
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            ClassLogger.Log(theEvent);
        }

        /// <summary>
        /// User Activity Log when LoggingError(Web.Config) Value is True
        /// </summary>
        /// <param name="userName">GET Logged user name </param>
        /// <param name="Text">Log Text</param>
        /// <param name="RequestUri">Request Action Path</param>
        protected void Error(string userName, string Text, string RequestUri)
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Error, "Exception", Text);
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = RequestUri;
            theEvent.Properties["RequestTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["ResponseTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["RegionId"] = 0;
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            ClassLogger.Log(theEvent);
        }

        /// <summary>
        ///  Error Exception Log when LoggingErrorException(Web.Config) Value is True
        /// </summary>
        /// <param name="userName">GET Logged user name</param>
        /// <param name="context">Get details about HttpActionExecutedContext</param>
        protected void ErrorException(string userName, HttpActionExecutedContext context)
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Error, "Exception", "JSON Method : " + context.Request.Method + " URL : " + context.Request.RequestUri + " Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + " Action : " + context.ActionContext.ActionDescriptor.ActionName);
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = context.Request.RequestUri;
            theEvent.Properties["error-source"] = context.Exception.Source;
            theEvent.Properties["error-class"] = context.Exception.TargetSite;
            theEvent.Properties["error-method"] = context.Exception.StackTrace;
            theEvent.Properties["error-message"] = context.Exception.Message;
            theEvent.Properties["RequestTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["ResponseTime"] = DateTime.Now.ToString(LoggingDateFormate);
            theEvent.Properties["RegionId"] = 0;
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            if (context.Exception.InnerException != null && !string.IsNullOrWhiteSpace(context.Exception.InnerException.Message))
                theEvent.Properties["inner-error-message"] = context.Exception.InnerException.Message;
            // deprecated

            ClassLogger.Log(theEvent);
        }

        protected void Info(string userName, ActionExecutedContext filterContext)
        {
            var RequestTime = ((DateTime)filterContext.HttpContext.Items["RequestTime"]).ToString(LoggingDateFormate);
            var ResponseTime = ((DateTime)filterContext.HttpContext.Items["ResponseTime"]).ToString(LoggingDateFormate);
            string strValues = string.Empty;

            try
            {
                strValues = JSONHelper.ToJSON(filterContext.HttpContext.Request.Form.AllKeys.ToDictionary(x => x, x => filterContext.HttpContext.Request.Form[x]));
            }
            catch { }

            LogEventInfo theEvent = new LogEventInfo(LogLevel.Info, "Exception", "JSON Method : " + filterContext.HttpContext.Request.HttpMethod + " URL : " + filterContext.HttpContext.Request.Url + " Controller : " + filterContext.RouteData.Values["Controller"] + Environment.NewLine + " Action : " + filterContext.RouteData.Values["Action"] + strValues);
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = filterContext.HttpContext.Request.Url;
            theEvent.Properties["RequestTime"] = RequestTime;
            theEvent.Properties["ResponseTime"] = ResponseTime;
            theEvent.Properties["RegionId"] = 0;
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            // deprecated
            ClassLogger.Log(theEvent);
        }

        /// <summary>
        ///  User Log info when LoggingInfo(Web.Config) Value is True
        /// </summary>
        /// <param name="userName">GET Logged user name</param>
        /// <param name="context">Get details about System.Web.Http.Controllers.HttpActionContext</param>
        protected void Info(string userName, HttpActionExecutedContext filterContext)
        {
            var RequestTime = ((DateTime)filterContext.Request.Properties["RequestTime"]).ToString(LoggingDateFormate);
            var ResponseTime = ((DateTime)filterContext.Request.Properties["ResponseTime"]).ToString(LoggingDateFormate); ;

            string HttpUserAgent = string.Empty;
            bool IsMobileBrowser = false;
            //trace.Info(filterContext.Request, "Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName, "JSON", filterContext.ActionArguments);
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Info, "Exception", "JSON Method : " + filterContext.Request.Method + " URL : " + filterContext.Request.RequestUri + " Controller : " + filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + " Action : " + filterContext.ActionContext.ActionDescriptor.ActionName + JSONHelper.ToJSON(filterContext.ActionContext.ActionArguments));
            theEvent.Properties["username"] = GetUserClaim(ClaimTypeName.CLAIM_USERNAME);
            theEvent.Properties["RequestUri"] = filterContext.Request.RequestUri;
            theEvent.Properties["IsMobileRequest"] = IsMobileBrowser;
            //theEvent.Properties["CompanyType"] = GetUserClaim(WebAPIUserClaimTypes.CLAIM_Company_TYPE);
            theEvent.Properties["HttpUserAgent"] = HttpUserAgent;
            theEvent.Properties["RequestTime"] = RequestTime;
            theEvent.Properties["ResponseTime"] = ResponseTime;
            theEvent.Properties["RegionId"] = 0;
            theEvent.Properties["srCon"] = GetCompanyConnectionString();
            // deprecated

            ClassLogger.Log(theEvent);
        }


        #region Get user claim
        /// <summary>
        /// Gets a user's claim
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        protected string GetUserClaim(string claimType)
        {
            if (!string.IsNullOrEmpty(claimType))
            {
                //Get the user's claims
                var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

                //If there are claims present, then get the type of Company
                if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
                {
                    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == claimType.ToLower()).Select(clm => clm.Value).FirstOrDefault();
                }
            }

            return null;
        }

        protected string GetCompanyConnectionString()
        {
            ////Get the user's Company unique id from the claims
            //CompanyModel selectedCompanyInformation = string.IsNullOrWhiteSpace(GetUserClaim(WebAPIUserClaimTypes.CLAIM_SELECTED_Company_INFORMATION)) ? null : JsonConvert.DeserializeObject<CompanyModel>(GetUserClaim(WebAPIUserClaimTypes.CLAIM_SELECTED_Company_INFORMATION));

            ////If the Company unique id is present, then get the corresponding Company information
            //return selectedCompanyInformation != null ? selectedCompanyInformation.DBConnectionString : null;

            return string.Empty;
        }

        #endregion
        #endregion

        #region Public Methods
        /// <summary>
        /// User Activity Log when LoggingDebug(Web.Config) Value is True
        /// </summary>
        /// <param name="Text">Log Text</param>
        public void Debug(string Text)
        {
            if (LoggingDebug)
            {
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                Text = "Class: " + stackFrame.GetMethod().DeclaringType + " Method: " + stackFrame.GetMethod().Name + " Line: " + stackFrame.GetFileLineNumber() + " Column: " + stackFrame.GetFileColumnNumber() + " Debug: " + Text;
                Debug(userName, Text, request);
            }
        }

        /// <summary>
        /// User Log info when LoggingInfo(Web.Config) Value is True
        /// </summary>
        /// <param name="Text">Log Text</param>
        public void Info(string Text, DateTime? RequestTime, DateTime? ResponseTime)
        {
            if (LoggingInfo)
            {
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                Text = "Class: " + stackFrame.GetMethod().DeclaringType + " Method: " + stackFrame.GetMethod().Name + " Line: " + stackFrame.GetFileLineNumber() + " Column: " + stackFrame.GetFileColumnNumber() + " Info: " + Text;
                Info(userName, Text, request, RequestTime, ResponseTime);
            }
        }

        /// <summary>
        /// User Log Error when LoggingError(Web.Config) Value is True
        /// </summary>
        /// <param name="Text">Log Text</param>
        public void Error(string Text)
        {
            if (LoggingError)
            {
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                Text = "Class: " + stackFrame.GetMethod().DeclaringType + " Method: " + stackFrame.GetMethod().Name + " Line: " + stackFrame.GetFileLineNumber() + " Column: " + stackFrame.GetFileColumnNumber() + " Error: " + Text;
                Error(userName, Text, request);
            }
        }

        /// <summary>
        /// User Log ErrorException when LoggingErrorException(Web.Config) Value is True
        /// </summary>
        /// <param name="context">Get Detials Avout HttpActionExecutedContext</param>
        public void ErrorException(HttpActionExecutedContext context)
        {
            if (LoggingException)
            {
                ErrorException(userName, context);
            }
        }

        /// <summary>
        /// User Log info when LoggingInfo(Web.Config) Value is True
        /// </summary>
        /// <param name="context">Get Detials Avout ActionExecutingContext</param>
        public void InfoWeb(ActionExecutedContext filterContext)
        {
            if (LoggingInfo)
            {
                Info(userName, filterContext);
            }
        }

        /// <summary>
        /// User Log info when LoggingInfo(Web.Config) Value is True
        /// </summary>
        /// <param name="context">Get Detials Avout System.Web.Http.Controllers.HttpActionContext</param>
        public void InfoAPI(HttpActionExecutedContext filterContext)
        {
            if (LoggingInfo)
            {
                Info(userName, filterContext);
            }
        }
        #endregion
    }

}