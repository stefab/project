using System;
using System.Diagnostics;
using System.Management;
using System.Reflection;

using OpenQA.Selenium.Firefox;

namespace EtsyRobot.Engine.WebSession
{
	internal class CustomFirefoxDriver : FirefoxDriver
	{
		public CustomFirefoxDriver(FirefoxBinary binary, FirefoxProfile profile)
			: base(binary, profile)
		{}
		public CustomFirefoxDriver(FirefoxDriverService service, FirefoxOptions opt, TimeSpan commandTimeout)
			: base(service, opt, commandTimeout)
		{}


		protected override void Dispose(bool disposing)
		{
			var processId = this.GetProcess().Id;
			try
			{
				base.Dispose(disposing);
			}
			finally
			{
				KillProcessAndChildren(processId);
			}
		}

		private Process GetProcess()
		{
			FieldInfo processField = this./*Binary.*/GetType().GetField("process", BindingFlags.Instance | BindingFlags.NonPublic);
          
            Debug.Assert(processField != null);
            return new Process(); //processField.GetValue(this.Binary);
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