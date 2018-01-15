using System;

namespace EtsyRobot.Engine.WebSession
{
	public class RemoteBrowserSettings : BrowserSettings
	{
		public RemoteBrowserSettings(string pluginFileName) : base(pluginFileName)
		{}

		public Uri RemoteAddress { get; set; }
	}
}