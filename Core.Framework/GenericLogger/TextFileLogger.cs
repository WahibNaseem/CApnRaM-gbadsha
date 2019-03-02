using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Core.Logger
{
    public class TextFileLogger:ILogger
    {
        string logPath;
        string parentLogFolderPath;
        object lockObject = new object();

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
            logPath = parentLogFolderPath;
            if (!logPath.EndsWith("\\"))
                logPath += "\\";

            LogData(data, logType);
        }

        private  void LogData(string data, LogType logType)
        {
            lock (lockObject)
            {
                logPath = logPath + "\\" + DateTime.Now.ToString("dd-MM-yyyy");
                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
                using (StreamWriter sw = new StreamWriter(logPath +"\\" +DateTime.Now.ToString("HH") + ".log", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + logType.ToString() + " --> " + data);
                    sw.Flush();
                    sw.Close();
                }
            }
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
