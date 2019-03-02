using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Core.Logger;

namespace Core.Exceptions
{
    public class WebException:Exception
    {
        string logPath;
        ILogger Logger;
        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="logPath">[Optional] If provided, the log file will be written in the the provided folder path</param>
        public WebException(string logPath)
        {
            this.logPath = logPath;
            Logger = LoggerFactory.CreateLogger(logPath);
        }

        /// <summary>
        /// Display nice user message to the client, log it then end the request.
        /// </summary>
        /// <param name="userMsg">Text message that will appear to the client who calling the application</param>
        /// <param name="logMsg">[Optional] If provided, This message will be logged in the log file. otherwise, the same 'userMsg' message will be logged</param>
        public void ThrowException(string userMsg,string logMsg="")
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/plain";
            if(logMsg=="")

                Logger.Log("Error:: " + userMsg,LogType.Error);

            else

                Logger.Log("Error:: " + logMsg, LogType.Error);

            HttpContext.Current.Response.Write(userMsg);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Display nice user message to the client, log it then end the request.
        /// </summary>
        /// <param name="userMsg">Text message that will appear to the client who calling the application</param>
        /// <param name="innerException">The Exception throwed by the system</param>
        /// <param name="logMsg">[Optional] in case provided, This message will be logged in the log file. otherwise, the same 'userMsg' message will be logged</param>
        public void ThrowException(string userMsg, Exception innerException, string logMsg = "")
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/plain";
            if (logMsg == "")
                Logger.Log("Error:: " + userMsg + Environment.NewLine + innerException.ToString(), LogType.Error);
            else
                Logger.Log("Error:: " + logMsg + Environment.NewLine + innerException.ToString(), LogType.Error);
            
            HttpContext.Current.Response.Write(userMsg);
            HttpContext.Current.Response.End();
        }
    }
}
