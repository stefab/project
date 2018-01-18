using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Newtonsoft.Json;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

using EtsyRobot.Engine.PageModel;
using EtsyRobot.Properties;
using EtsyRobot.Engine.WebSession.AdsFinder;
using System.Threading;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using System.IO;

namespace EtsyRobot.Engine.WebSession
{
	internal class DefaultBrowserSession : IBrowserSession
	{
        static int scrapeLayoutRowBlock = 50;
		public DefaultBrowserSession(IWebDriver webDriver)
		{
			this._webDriver = webDriver;
			this._scriptExecutor = (IJavaScriptExecutor) webDriver;
		}

		#region IBrowserSession Members
		public void Dispose()
		{
			this._webDriver.Quit();
		}

        public PageContent Scrape(Uri uri, bool isReferenceScraping = true)
		{
			DateTime startedAt = DateTime.Now;
			try
			{
                Thread.Sleep(100);
                this._webDriver.Url = uri.ToString();
                // wait page fully load
                Thread.Sleep(1000);
			}
			catch (WebDriverTimeoutException)
			{
				// Assuming is was enough time to load a page. Page is scraped "as is".
			}
			_tracer.TraceEvent(TraceEventType.Verbose, 0, "Page {0} opened in {1:f3} seconds.", uri,
			                   (DateTime.Now - startedAt).TotalSeconds);

			this.InjectHelperScripts();

			PageContent content = this.ScrapeContent(isReferenceScraping);
			_tracer.TraceEvent(TraceEventType.Verbose, 0, "Page content has been scaped.");

			Image screenshot = this.GetScreenshot();
			_tracer.TraceEvent(TraceEventType.Verbose, 0, "Screenshot image has taken.");
			content.Screenshot = screenshot;
			return content;
		}
		#endregion

		public DefaultBrowserSession Configure(BrowserSettings settings)
		{
			this.ConfigureWindowSize(settings);
			this.ConfigureTimeout(settings);
			return this;
		}

		protected virtual void ConfigureWindowSize(BrowserSettings settings)
		{
			this._webDriver.Manage().Window.Size = settings.WindowSize;
		}

		protected virtual void ConfigureTimeout(BrowserSettings settings)
		{
			this._webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(settings.PageLoadTimeout);

            //TODO: This setting does not work. Script timeout should be configured via browser-specific settings.
            this._webDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(settings.CommandTimeout); 
            //SetScriptTimeout(TimeSpan.FromSeconds(settings.CommandTimeout)); // old
            Console.WriteLine("Driver settings: page load timeout {0}, ScriptTimeout {1}.",  settings.PageLoadTimeout, settings.CommandTimeout);           
		}

		protected virtual Image GetScreenshot()
		{
			Screenshot screenshot = this._webDriver.TakeScreenshot();
			return ImageUtils.BytesToImage(screenshot.AsByteArray);
		}

        protected int[,] createLayoutTable(int height, int width) {
            DateTime startedAt = DateTime.Now;
            int[,] layout = new int[height, width];
			if (true) {
                //int block = scrapeLayoutRowBlock;
                //int last = 0;
                
                //while (height  > last) {
                //    int start = last;
                //    last += block;
                //    if (height < last) {
                //        last = height;
                //    }
                //    else if (height < last + block) {
                //        last = height;
                //    }
                //    object rawLayout = this._scriptExecutor.ExecuteScript(Resources.ScrapeMetadata + "\n" +
			             //                                              string.Format("return window.privDogAuto.createLayoutTableForDiapason({0}, {1}, {2});", start, last, width));

                //    int[,] layoutPart = null;
                //    try
                //    {
                //        layoutPart = Deserialize<int[,]>(rawLayout.ToString());
                //    }
                //    catch (Exception e)
                //    {
                //        //Debug.WriteLine("Can not Deserialize nodes. Error: " + e.Message);
                //        // sometimes js return array object as string like to JSON.stringify(nodesTable.toString());
                //        layoutPart = Deserialize<int[,]>(rawLayout.ToString().Trim('"'));
                //    }

                //    if (layoutPart != null) {
                //        for (int i = start; i < last; i++ ) {
                //            for (int j = 0; j < width; j++) {
                //                layout[i, j] = layoutPart[i - start, j]; 
                //            }
                //        }                    
                //    }

                //    _tracer.TraceEvent(TraceEventType.Verbose, 0, "createLayoutTable parsed {0} in {1:f3} seconds.", last, (DateTime.Now - startedAt).TotalSeconds);
                //}  
            }
            else {
                object rawLayout; // = this._scriptExecutor.ExecuteScript(Resources.ScrapeMetadata + "\n" +
			                      //                                    "return window.privDogAuto.createLayoutTable();");
			    layout = Deserialize<int[,]>(rawLayout.ToString());
			    _tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}x{1} layout loaded.",
			                       layout.GetUpperBound(1) + 1, layout.GetUpperBound(0) + 1);           
            }
            _tracer.TraceEvent(TraceEventType.Verbose, 0, "createLayoutTable in {0:f3} seconds.", (DateTime.Now - startedAt).TotalSeconds);
            return layout;
        }

        protected virtual PageContent ScrapeContent(bool isReferenceScraping)
		{

   //         dynamic viewport = this._scriptExecutor.ExecuteScript(Resources.ScrapeMetadata + "\n" +
			//                                                      "return window.privDogAuto.getViewport();");
			//_tracer.TraceEvent(TraceEventType.Verbose, 0, "Processing viewport {0}x{1}.",
			//                   (int) viewport["width"], (int) viewport["height"]);

   //         // scrape DomNodes
   //         object rawNodes = this._scriptExecutor.ExecuteScript(Resources.ScrapeMetadata + "\n" +
			//                                                     "return window.privDogAuto.createNodesTable();");
   //         List<NodeInfo> nodes = null;
   //         try
   //         {
   //             nodes = Deserialize<List<NodeInfo>>(rawNodes.ToString());
   //         } 
   //         catch (Exception e) {
   //             //Debug.WriteLine("Can not Deserialize nodes. Error: " + e.Message);
   //             // sometimes js return array object as string like to JSON.stringify(nodesTable.toString());
   //             // "[... \"dt\": 26, \"u\": \"javascript:\\\"<html><body style='background:transparent'></body></html>\\\"\"},  .. ]"
   //             // 1. need \" -> " 2. trim '"' 3. not change real escaped strings \\\" -> \"
   //             string rawNodesCorrected =System.Text.RegularExpressions.Regex.Unescape( rawNodes.ToString());
   //             nodes = Deserialize<List<NodeInfo>>(rawNodesCorrected.Trim('"'));
   //         }
   //         _tracer.TraceEvent(TraceEventType.Verbose, 0, "{0} nodes loaded.", nodes.Count);
   //         // scrape layout (~bitmaps)
   //         var layout = this.createLayoutTable((int) viewport["height"], (int) viewport["width"]);

   //         // ads finder block
   //         List<_ShowContentFrame> _ads = new List<_ShowContentFrame>();
   //         _tracer.TraceEvent(TraceEventType.Verbose, 0, "isReferenceScraping {0}", isReferenceScraping);
            
   //         if (!isReferenceScraping)  {
   //             AdsFinder.AdsFinder finder = new AdsFinder.AdsFinder(this, this._webDriver);
   //             //_ads = finder.findReplacedAds() as List<_ShowContentFrame>;
   //             _ads.AddRange(finder.findReplacedAds());
   //             foreach (var ad in _ads) {
			//       _tracer.TraceEvent(TraceEventType.Verbose, 0, "ads find {0}x{1} advert: {2}.",
			//                  ad.SafeWidth, ad.SafeHeight, ad.Advert); 
   //             }
   //         }

            return new PageContent { };
				//{
				//	Nodes = nodes.ConvertAll(BuildNode).ToArray(),
				//	Layout = layout,
                //  AdZoneNodes = _ads.ConvertAll(BuildAdZoneNode).ToArray() 
				//};
		}


        protected void framesScrapeContent(ISearchContext topWebElement, int lvl = 0)
        {
            // Iterate over iframes tree
            // must not change context (switch_to)

            // switch to current frame context
            if (topWebElement is IWebDriver)
            {
                ((IWebDriver)topWebElement).SwitchTo().DefaultContent();
            }
            else if ((topWebElement is IWebElement) && string.Equals(((IWebElement)topWebElement).TagName, "iframe", StringComparison.OrdinalIgnoreCase))
            {
                this._webDriver.SwitchTo().Frame((IWebElement)topWebElement);
            }
            try
            {
                // iterate over all frames
                ReadOnlyCollection<IWebElement> _curFrames = this._webDriver.FindElements(By.XPath("//iframe"));
                foreach (IWebElement _curFrame in _curFrames)
                {
                    string frmtStr = new string('-', 2 * lvl);
                    //Console.WriteLine(frmtStr + "frame: " + _curFrame.GetAttribute("outerHTML"));
                    _tracer.TraceEvent(TraceEventType.Verbose, 0, frmtStr + "scrape frame: " + _curFrame.GetAttribute("outerHTML"));
                    
                }
            }
            finally
            {
                // restore current parent frame
                this._webDriver.SwitchTo().ParentFrame();
            }

        }



		protected IWebDriver WebDriver
		{
			get { return this._webDriver; }
		}

		private void InjectHelperScripts()
		{}

		static private T Deserialize<T>(string json)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (JsonException)
			{
				_tracer.TraceData(TraceEventType.Error, 0, json.Substring(0, 120));
				throw;
			}
		}
		static private readonly TraceSource _tracer = new TraceSource("Engine.Scraping", SourceLevels.All);

		private readonly IWebDriver _webDriver;
		private readonly IJavaScriptExecutor _scriptExecutor;

	}
}