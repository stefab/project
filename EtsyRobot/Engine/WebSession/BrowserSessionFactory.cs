using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System.Reflection;
using System.Management;
using System.Diagnostics;

//from selenium import webdriver;

namespace EtsyRobot.Engine.WebSession
{
	public sealed class BrowserSessionFactory
	{
		public IBrowserSession Create(BrowserSettings settings)
		{
			switch (settings.BrowserType)
			{
				case BrowserType.Firefox:
					return CreateFirefoxAdapter(settings);

				case BrowserType.Chrome:
					return CreateChromeAdapter(settings);

				default:
					throw new ArgumentException("Unknown browser type.");
			}
		}

		public IBrowserSession CreateRemote(RemoteBrowserSettings settings)
		{
			switch (settings.BrowserType)
			{
				case BrowserType.Firefox:
					return CreateRemoteFirefoxAdapter(settings);

				case BrowserType.Chrome:
					return CreateRemoteChromeAdapter(settings);

				default:
					throw new ArgumentException("Unknown browser type.");
			}
		}

		static private IBrowserSession CreateFirefoxAdapter(BrowserSettings settings)
		{
			var profile = new FirefoxProfile();
			profile.SetPreference("dom.max_chrome_script_run_time", settings.CommandTimeout);
			profile.SetPreference("dom.max_script_run_time", settings.CommandTimeout);
			//profile.SetPreference("xpinstall.signatures.required", false);
			if (settings.PluginFileName != null)
			{
				profile.AddExtension(settings.PluginFileName);
			}
			var webDriver = new CustomFirefoxDriver(new FirefoxBinary(), profile, TimeSpan.FromSeconds(settings.CommandTimeout));
			return new DefaultBrowserSession(webDriver).Configure(settings);
		}

		static private IBrowserSession CreateChromeAdapter(BrowserSettings settings)
		{
			var options = new ChromeOptions();
            options.AddArgument("window-size=1280,768");
            //options.AddArgument("disable-popup-blocking");
            options.AddArgument("disable-3d-apis");
            options.AddArgument("disable-gpu");
            if (settings.PluginFileName != null)
			{
                options.AddExtension(settings.PluginFileName);
			}
            options.AddArgument("sidfordelete=" + new Random().Next());

			var webDriver = CustomChromeDriver.Build(options, TimeSpan.FromSeconds(settings.CommandTimeout));
            return new ChromeBrowserSession(webDriver).Configure(settings);
		}

		static private IBrowserSession CreateRemoteFirefoxAdapter(RemoteBrowserSettings settings)
		{
			var profile = new FirefoxProfile();
			profile.SetPreference("dom.max_chrome_script_run_time", settings.CommandTimeout);
			profile.SetPreference("dom.max_script_run_time", settings.CommandTimeout);

			DesiredCapabilities capabilities = DesiredCapabilities.Firefox();
			capabilities.SetCapability(FirefoxDriver.ProfileCapabilityName, profile.ToBase64String());

			RemoteWebDriver webDriver = CreateRemoteDriver(capabilities, settings);
			return new DefaultBrowserSession(webDriver).Configure(settings);
		}

		static private IBrowserSession CreateRemoteChromeAdapter(RemoteBrowserSettings settings)
		{
			RemoteWebDriver webDriver = CreateRemoteDriver(DesiredCapabilities.Chrome(), settings);
			return new ChromeBrowserSession(webDriver).Configure(settings);
		}

		static private RemoteWebDriver CreateRemoteDriver(ICapabilities capabilities, RemoteBrowserSettings settings)
		{
			return new RemoteWebDriver(settings.RemoteAddress, capabilities, TimeSpan.FromSeconds(settings.CommandTimeout));
		}
	}
}