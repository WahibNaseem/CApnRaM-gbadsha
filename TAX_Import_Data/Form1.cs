using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Threading;
using System.ServiceProcess;
using System.Globalization;
using TaxImportLogic;

namespace Tax_Import_Data
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public string LogPath { get; set; }
        public int SleepTime { get; set; }
        public string ServiceName { get; set; }
        public string APIURL { get; set; }
        public string APIKey { get; set; }

        public void RefreshConfig()
        {
            try
            {

                ConfigurationManager.RefreshSection("appSettings");
               
                LogPath = Convert.ToString(ConfigurationManager.AppSettings["LogPath"]);
                SleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["ServiceSleepTime"]);
                ServiceName = Convert.ToString(ConfigurationManager.AppSettings["ServiceName"]);
                APIURL= Convert.ToString(ConfigurationManager.AppSettings["APIURL"]);
                APIKey = Convert.ToString(ConfigurationManager.AppSettings["APIKey"]);
            }
            
            
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
        public void SaveConfig()
        {
            // Open App.Config of executable
            System.Configuration.Configuration config =
             ConfigurationManager.OpenExeConfiguration
                        (ConfigurationUserLevel.None);

            // Add an Application Setting.

            
            config.AppSettings.Settings.Remove("APIURL");
            config.AppSettings.Settings.Remove("ServiceSleepTime");

            config.AppSettings.Settings.Add("APIURL", APIURL);
            
            config.AppSettings.Settings.Add("ServiceSleepTime", SleepTime.ToString());

            // Save the changes in App.config file.

            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of a changed section.

            ConfigurationManager.RefreshSection("appSettings");
        }
        private void button9_Click(object sender, EventArgs e)
        {
            imgWait.Visible = true;
            btnImport.Enabled = false;
            ImportWorker.RunWorkerAsync();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(txtSleepTime.Text==string.Empty)
            {
                MessageBox.Show("Must Enter Sleep Time", "Tax Import Tool");
                return;
            }
            APIURL = txtAPIURL.Text;
            SleepTime = Convert.ToInt32(txtSleepTime.Text);
            SaveConfig();
            if (RestartService())
                MessageBox.Show("Data Save Complete.", "Tax Import Tool");
            else
                MessageBox.Show("Fail to restart service try restarting service manually.", "Tax Import Tool");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                 System.Diagnostics.Process.Start("explorer.exe", LogPath);
            }
            catch
            { MessageBox.Show("Error When Open Log Location,Check Config And Try Again."); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshConfig();
            txtAPIURL.Text=APIURL;
            txtSleepTime.Text = SleepTime.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //BL_Import.SetDBConnection(2);
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //BL_Import.SetDBConnection(1);
            }
            catch { }
        }

      
       

        private void ImportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                 RefreshConfig();
                 new LogicExecuter().CallAPIAndImportData(APIURL, APIKey);

             }
             catch(Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
         }

        private void ImportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            imgWait.Visible = false;
            btnImport.Enabled = true;
            MessageBox.Show("Data Import Complete.Refare to log for more information.", "Tax Import Tool");
        }
        public bool StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RestartService()
        {
            StopService(ServiceName, 10 * 1000);

            if (StartService(ServiceName, 10 * 1000))
                return true;
            else
                return false;
        }

        public bool StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
          //  MessageBox.Show(ServiceName);
            if(StartService(ServiceName, 60 * 1000))
                MessageBox.Show("Service Started.", "Tax Import Tool");
            else
                MessageBox.Show("Faild to start service or service already running.", "Tax Import Tool");
            btnStart.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            if(StopService(ServiceName, 60 * 1000))
                MessageBox.Show("Service Stopped.", "Tax Import Tool");
            else
                MessageBox.Show("Faild to stop service or service not running.", "Tax Import Tool");

            btnStop.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtSleepTime_TextChanged(object sender, EventArgs e)
        {
            int i;
            if (!Int32.TryParse(((TextBox)sender).Text, out i))
            {
                ((TextBox)sender).Clear();

            }
            else if (i < 0)
                ((TextBox)sender).Clear();
        }

        private void imgWait_Click(object sender, EventArgs e)
        {

        }
    }
}
