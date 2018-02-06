using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace EtsyRobot.Engine.WebSession
{
	internal class ChromeBrowserSession : DefaultBrowserSession
	{
		public ChromeBrowserSession(IWebDriver webDriver) : base(webDriver)
		{}

		protected override void ConfigureWindowSize(BrowserSettings settings)
		{
			// Window size is configured in Chrome options.
            base.ConfigureWindowSize(settings);
		}

		public override Image GetScreenshot()
		{
			//Thread.Sleep(2000);
			//return this.GetFullScreenshot();
            return base.GetScreenshot();
		}

		/// <summary>
		/// There is open issue for Chrome - screenshot is truncated by browser winsow size.
		/// So we should use a custom workaround.
		/// </summary>
		/// <returns></returns>
		private Bitmap GetFullScreenshot()
		{
			Bitmap stitchedImage = null;
			try
			{
				var totalwidth1 = this.WebDriver.ExecuteJavaScript<long>("return document.body.offsetWidth");

				var totalHeight1 = this.WebDriver.ExecuteJavaScript<long>("return  document.body.parentNode.scrollHeight");

				var totalWidth = (int) totalwidth1;
				var totalHeight = (int) totalHeight1;

				// Get the Size of the Viewport
				var viewportWidth1 = this.WebDriver.ExecuteJavaScript<long>("return document.body.clientWidth");
				//documentElement.scrollWidth");
				var viewportHeight1 = this.WebDriver.ExecuteJavaScript<long>("return window.innerHeight");
				//documentElement.scrollWidth");

				var viewportWidth = (int) viewportWidth1;
				var viewportHeight = (int) viewportHeight1;


				// Split the Screen in multiple Rectangles
				var rectangles = new List<Rectangle>();
				// Loop until the Total Height is reached
				for (int i = 0; i < totalHeight; i += viewportHeight)
				{
					int newHeight = viewportHeight;
					// Fix if the Height of the Element is too big
					if (i + viewportHeight > totalHeight)
					{
						newHeight = totalHeight - i;
					}
					// Loop until the Total Width is reached
					for (int ii = 0; ii < totalWidth; ii += viewportWidth)
					{
						int newWidth = viewportWidth;
						// Fix if the Width of the Element is too big
						if (ii + viewportWidth > totalWidth)
						{
							newWidth = totalWidth - ii;
						}

						// Create and add the Rectangle
						var currRect = new Rectangle(ii, i, newWidth, newHeight);
						rectangles.Add(currRect);
					}
				}

				// Build the Image
				stitchedImage = new Bitmap(totalWidth, totalHeight);
				// Get all Screenshots and stitch them together
				Rectangle previous = Rectangle.Empty;
				foreach (Rectangle rectangle in rectangles)
				{
					// Calculate the Scrolling (if needed)
					if (previous != Rectangle.Empty)
					{
						int xDiff = rectangle.Right - previous.Right;
						int yDiff = rectangle.Bottom - previous.Bottom;
						this.WebDriver.ExecuteJavaScript<long>(String.Format("window.scrollBy({0}, {1}); return 0;", xDiff, yDiff));
						Thread.Sleep(200);
					}

					Screenshot screenshot = this.WebDriver.TakeScreenshot();

					// Build an Image out of the Screenshot
					Image screenshotImage;
					using (var memStream = new MemoryStream(screenshot.AsByteArray))
					{
						screenshotImage = Image.FromStream(memStream);
					}

					// Calculate the Source Rectangle
					var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height,
					                                    rectangle.Width, rectangle.Height);

					// Copy the Image
					using (Graphics g = Graphics.FromImage(stitchedImage))
					{
						g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
					}

					// Set the Previous Rectangle
					previous = rectangle;
				}
			}
			catch (Exception)
			{
				// handle
			}
			return stitchedImage;
		}
	}
}