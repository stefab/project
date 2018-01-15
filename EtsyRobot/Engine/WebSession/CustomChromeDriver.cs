﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Management;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Reflection;

namespace EtsyRobot.Engine.WebSession
{
    internal class CustomChromeDriver : ChromeDriver
    {
        private static Object _buildLock = new Object();

        protected int driverPid = -1;
        public static CustomChromeDriver Build(ChromeOptions profile, TimeSpan commandTimeout)
        {
            CustomChromeDriver driver = null;
            lock (_buildLock) {
            // catch and story chromedriver pid
                IList<int> pidsBefore = getProcessDrivers(Process.GetCurrentProcess().Id);
                driver = new CustomChromeDriver(profile, commandTimeout);
                IList<int> pidsAfter = getProcessDrivers(Process.GetCurrentProcess().Id);
                IList<int> rest = pidsAfter.Except(pidsBefore).ToList();
                driver.driverPid = rest.DefaultIfEmpty(-1).First();
            }
            return driver;
        }

        protected CustomChromeDriver(ChromeOptions profile, TimeSpan commandTimeout) : base(profile)
        {
            this.Manage().Timeouts().ImplicitWait = commandTimeout;// svt2 //SetScriptTimeout(commandTimeout);
        }

        public static IList<int> getProcessDrivers(int parentPid)
        {
            List<int> pids =  new List<int>();
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + parentPid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                //foreach (PropertyData prop in mo.Properties) { Console.WriteLine("{0}: {1}", prop.Name, prop.Value); }
                try {
                    if (mo["ExecutablePath"].ToString().ToLower().Contains("chromedriver.exe"))
                    {
                        int webPid = Convert.ToInt32(mo["ProcessID"]);
                        if (webPid > 0) {
                            pids.Add(webPid);                           
                        }
                    }                
                }
                catch(Exception e) {
                    Debug.WriteLine("Can not get ManagementObject property: " + e.Message);
                }
            }
            return pids;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                if (GetDriverProcessId() > 0) {
                    KillProcessAndChildren(GetDriverProcessId());                
                }
            }
        }

        private int GetDriverProcessId()
        {
            return driverPid;
        }

        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        static private void KillProcessAndChildren(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

    }
}
