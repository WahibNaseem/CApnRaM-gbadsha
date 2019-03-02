using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logger
{
    public enum LogType
    {
        Info = 0,
        Error = 1,
        Warning = 2,
        Required = 3
    };

    public class LoggerFactory
    {
        static public ILogger CreateLogger(string logFolderName = "")
        {
            //  NOTE:
            //       1) Logging Place should be configuration file Key "LogInFileOrEvent" with values (file | event )
            //       2) A key in web config of each solution must be added under name "ParentLogFolderPath" contains parent log folder name
            //
            ILogger log = null;
            string place = "file";
            if(String.IsNullOrEmpty(place))
                throw new Exception("Error::LoggerFactory:: The key \"LogInFileOrEvent\" in config file of the project is not found or have invalid values, please add it with one value of (file | event).");
            
            switch (place.ToLower())
            {
                case "file":
                    {
                        log = new TextFileLogger();
                        if (String.IsNullOrEmpty(logFolderName))
                            log.ParentLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
                        else
                            log.ParentLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + logFolderName;
                        break;
                    }
                case "event":
                    {
                        log = new EventViewerLogger();
                        if (String.IsNullOrEmpty(logFolderName))
                            log.ParentLogFolderPath = "Log";
                        else
                            log.ParentLogFolderPath = logFolderName;
                        break;
                    }
                default:
                    throw new Exception("Error::LoggerFactory:: The key \"LogInFileOrEvent\" in config file of the project has invalid values, please add it with one value of (file | event).");
            }
            
            return log;
        }
    }
}
