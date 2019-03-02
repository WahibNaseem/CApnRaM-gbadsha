using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Tax_Import_Data
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
           /* Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if((!CheckExeExists()) && GAC_Import.BL_Import.CheckProtection()) // Rashad for no doungle
                Application.Run(new Form1());*/
        }
        static bool CheckExeExists()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if ((Process.GetCurrentProcess().ProcessName == p.ProcessName) && (Process.GetCurrentProcess().Id != p.Id))
                {
                    MessageBox.Show("The program is already opened", "Tax Import Tool");
                    return true;
                }
            }
            return false;
        }
    }
}
