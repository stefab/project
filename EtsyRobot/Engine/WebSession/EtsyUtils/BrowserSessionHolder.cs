using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;

namespace EtsyRobot.Engine.WebSession.EtsyUtils
{
    internal class BrowserSessionHolder
    {
        private DefaultBrowserSession _session;
        private IWebDriver _webDriver;
        public BrowserSessionHolder()
        {
        }
        public BrowserSessionHolder(DefaultBrowserSession session)
        {
            init(session);
        }
        public virtual void init(DefaultBrowserSession session)
        {
            _session = session;
            _webDriver = session.WebDriver;
        }
    }
}
