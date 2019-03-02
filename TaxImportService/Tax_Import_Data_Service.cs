using Core.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaxImportLogic;

namespace WindowsService
{
    public partial class Tax_Import_Data_Service : ServiceBase
    {
        System.Timers.Timer timer;
        bool IsWorking = false;

        public string APIUrl { get; set; }
        public string APIKey{ get; set; }
        public int SleepTime { get; set; }

        public Tax_Import_Data_Service()
        {
            InitializeComponent();
        }
        public static string strServiceStartPath
        {
            get
            {
                return
                string.Format("{0}\\", Path.GetDirectoryName(

                System.Reflection.Assembly.

                GetExecutingAssembly().GetModules()[0].

                FullyQualifiedName));
            }
        }
        private void RefreshConfig()
        {
            XmlDocument xDoc;

            xDoc = new XmlDocument();

            string path = strServiceStartPath + "Tax_Import_Data.exe.config";

         //   LoggerFactory.CreateLogger().Log(path, LogType.Info);

            xDoc.Load(path);
         //   LoggerFactory.CreateLogger().Log("after load", LogType.Info);

            XmlNodeList xmlNodelst = xDoc.DocumentElement.SelectNodes("//add");

          //  LoggerFactory.CreateLogger().Log("after select node", LogType.Info);


            LoggerFactory.CreateLogger().Log(xmlNodelst.Count.ToString(), LogType.Info);

            foreach (XmlNode node in xmlNodelst)
            {
                if (node.Attributes["key"] == null)
                    continue;
                
                if (node.Attributes["key"].Value == "ServiceSleepTime")
                    SleepTime = Convert.ToInt32(node.Attributes["value"].Value);

                else if (node.Attributes["key"].Value == "APIURL")
                    APIUrl = node.Attributes["value"].Value;

                else if (node.Attributes["key"].Value == "APIKey")
                    APIKey = node.Attributes["value"].Value;


            }

        }
        protected override void OnStart(string[] args)
        {
            LoggerFactory.CreateLogger().Log("-------------  Tax Import Started -------------", LogType.Info);
            RefreshConfig();
            int TimeInMiliSec = SleepTime * 60 * 60 * 1000;
            timer = new System.Timers.Timer(TimeInMiliSec);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            RunLogic();
            LoggerFactory.CreateLogger().Log("-------------  Tax Import Started  -------------", LogType.Info);
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunLogic();
        }
        private void RunLogic()
        {
            try
            {
                if (!IsWorking)
                {
                    IsWorking = true;

                    new LogicExecuter().CallAPIAndImportData(APIUrl,APIKey);
                    IsWorking = false;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger().Log("Error::Timer_Elapsed: " + ex.ToString(), LogType.Error);
            }
        }
        protected override void OnStop()
        {
            LoggerFactory.CreateLogger().Log("-------------  Tax Import Stopped  -------------", LogType.Info);
            timer.Stop();
        }
    }
}
