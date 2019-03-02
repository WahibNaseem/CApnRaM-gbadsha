using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Logger
{
    public interface ILoggerUtility
    {
        void Trace(string source, string message);

        void Debug(string source, string message);

        void Information(string source, string message);

        void Warn(string source, string message);

        void Error(string source, string message);

        void Error(string source, Exception exception);

        void Error(string source, string message, Exception exception);
    }
}
