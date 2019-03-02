using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Logger
{
    public class LoggerUtility : ILoggerUtility
    {

        private static NLog.Logger GetLoggerInstance(string source)
        {
            return LogManager.GetLogger(source);
        }

        public void Trace(string source, string message)
        {
            GetLoggerInstance(source).Trace(message);
        }

        public void Debug(string source, string message)
        {
            GetLoggerInstance(source).Debug(message);
        }

        public void Information(string source, string message)
        {
            GetLoggerInstance(source).Info(message);
        }

        public void Warn(string source, string message)
        {
            GetLoggerInstance(source).Warn(message);
        }

        public void Error(string source, string message)
        {
            GetLoggerInstance(source).Error(message);
        }

        public void Error(string source, Exception exception)
        {
            GetLoggerInstance(source).Error(exception);
        }

        public void Error(string source, string message, Exception exception)
        {
            GetLoggerInstance(source).Error(message, exception);
        }
    }
}
