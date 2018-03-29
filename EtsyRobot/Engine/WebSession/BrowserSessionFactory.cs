using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System.Reflection;
using System.Management;
using System.Diagnostics;
using System.IO;

//from selenium import webdriver;

namespace EtsyRobot.Engine.WebSession
{
	internal sealed class BrowserSessionFactory
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
            string pathToCurrentUserProfiles = Environment.ExpandEnvironmentVariables("%TEMP%") + @"\ff_selenium_etsy3";
            //string[] pathsToProfiles = Directory.GetDirectories(pathToCurrentUserProfiles, "*.default", SearchOption.TopDirectoryOnly);
            //if (pathsToProfiles.Length != 0)
            //{
            //    FirefoxProfile profile = new FirefoxProfile(pathsToProfiles[0]);
            //    profile.SetPreference("browser.tabs.loadInBackground", false); // set preferences you need
            //    driver = new FirefoxDriver(new FirefoxBinary(), profile, serverTimeout);
            //}
            //FirefoxProfile profile = new FirefoxProfile(@"C:\Users\stefan\AppData\Roaming\Mozilla\Firefox\Profiles\ui906pgj.selenium_etsy", false); //"P:\\var\\ff_profile"
                                                                                                                                                    //var profileManager = new FirefoxProfileManager();
                                                                                                                                                    //FirefoxProfile profile = profileManager.GetProfile(@"ff_etsy");
            FirefoxProfile profile = new FirefoxProfile(); 
            if (profile == null)
            {
                profile = new FirefoxProfile();
            }            
            profile.SetPreference("browser.tabs.loadInBackground", false);
			profile.SetPreference("dom.max_chrome_script_run_time", settings.CommandTimeout);
			profile.SetPreference("dom.max_script_run_time", settings.CommandTimeout);
            //profile.SetPreference("webdriver.firefox.profile", "ff_etsy");
            profile.DeleteAfterUse = false;
            //profile.SetPreference("xpinstall.signatures.required", false);
			if (settings.PluginFileName != null)
			{
				profile.AddExtension(settings.PluginFileName);
			}
            var  cc = TimeSpan.FromSeconds(settings.CommandTimeout);
            
            FirefoxOptions opt = new FirefoxOptions();
            //opt.Profile = profile;
            // To have geckodriver pick up an existing profile on the filesystem, you may pass ["-profile", "/path/to/profile"].
            opt.AddArguments( new string[] { "-profile", pathToCurrentUserProfiles }); // @"P:\var\ff_profile"
            //opt.AddArguments( new string[] { "-P", "ff_etsy" });
            //opt.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true);
            //opt.AddAdditionalCapability(CapabilityType.IsJavaScriptEnabled, true);
            //opt.AddAdditionalCapability(CapabilityType.HasNativeEvents, true);
            //opt.SetPreference("webdriver.firefox.profile", "ff_etsy");
            //var webDriver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(), opt, TimeSpan.FromSeconds(settings.CommandTimeout));
            var webDriver = new FirefoxDriver(opt);
            return new DefaultBrowserSession(webDriver).Configure(settings);
		}

        static int indx = 0;
		static private IBrowserSession CreateChromeAdapter(BrowserSettings settings)
		{
			var options = new ChromeOptions();
            options.AddArgument("window-size=1280,768");
            //options.AddArgument("disable-popup-blocking");
            options.AddArgument("disable-3d-apis");
            options.AddArgument("disable-gpu");
            // chrome_options.add_argument("--profile-directory=Profile1")
            string pathToCurrentUserProfiles = Environment.ExpandEnvironmentVariables("%TEMP%") + @"\chrome_selenium_etsy"; // + indx.ToString();

            indx++;
            options.AddArgument("user-data-dir=" + pathToCurrentUserProfiles);
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