using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logger
{
    public class EventViewerLogger:ILogger
    {
        string parentLogFolderPath;
        public void Log(string data, LogType logType, bool enableLogging = true)
        {
            if (!enableLogging)
            {
                if (logType == LogType.Error)
                    DoLog(data, logType);
            }
            else
            {
                DoLog(data, logType);
            }
       }

        private void DoLog(string data, LogType logType)
        {
            string EventViewerName = "application";

            if (!EventLog.SourceExists(EventViewerName))
                EventLog.CreateEventSource(EventViewerName, EventViewerName);
            if (logType == LogType.Error)
                EventLog.WriteEntry(EventViewerName, data, EventLogEntryType.Error);
            else if (logType == LogType.Warning)
                EventLog.WriteEntry(EventViewerName, data, EventLogEntryType.Warning);
            else if (logType == LogType.Info)
                EventLog.WriteEntry(EventViewerName, data, EventLogEntryType.Information);
        }

        public string ParentLogFolderPath
        {
            set
            {
                parentLogFolderPath = value;
            }
        }
    }
}
