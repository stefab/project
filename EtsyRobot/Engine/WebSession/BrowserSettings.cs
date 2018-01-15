using System;
using System.Drawing;
using System.Linq;

using EtsyRobot.Properties;

namespace EtsyRobot.Engine.WebSession
{
	public class BrowserSettings
	{
		public BrowserSettings(string pluginFileName)
		{
			this.PluginFileName = pluginFileName;
		}

		public BrowserType BrowserType { get; internal set; }
		public int PageLoadTimeout { get; internal set; }
		public int CommandTimeout { get; internal set; }
		public Size WindowSize { get; private set; }
		public string PluginFileName { get; private set; }

		static public BrowserSettings CreateDefault()
		{
			string pluginPathOption = Environment.GetCommandLineArgs().FirstOrDefault(
				a => a.StartsWith("-pluginPath", StringComparison.InvariantCultureIgnoreCase));

			string pluginFileName = null;
			if (pluginPathOption != null)
			{
				pluginFileName = pluginPathOption.Substring("-pluginPath".Length + 1);
			}
            
			return Settings.Default.UseRemoteWebDriver
				       ? new RemoteBrowserSettings(pluginFileName)
					       {
						       BrowserType = Settings.Default.BrowserType,
						       PageLoadTimeout = Settings.Default.PageLoadTimeout,
						       CommandTimeout = Settings.Default.RemoteCommandTimeout,
						       WindowSize = new Size(1280, 768),
						       RemoteAddress = new Uri(Settings.Default.RemoteWebDriverServerUrl),
					       }
				       : new BrowserSettings(pluginFileName)
					       {
						       BrowserType = Settings.Default.BrowserType,
                               PageLoadTimeout = Settings.Default.PageLoadTimeout,
						       CommandTimeout = Settings.Default.RemoteCommandTimeout,
						       WindowSize = new Size(1280, 768),
					       };
		}
	}
}