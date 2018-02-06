using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EtsyRobot.Engine.WebSession.EtsyUtils
{
    class EtsyAuthentication :  BrowserSessionHolder
    {
        static private readonly TraceSource _tracer = new TraceSource("EtsyRobot", SourceLevels.All);
        public EtsyAuthentication(DefaultBrowserSession session) : base(session)
        {
        }

        string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public bool signIn(string login, string password)
        {
            Login = login;
            Password = password;
            ReadOnlyCollection<IWebElement> _curSign = session.FindElements(By.XPath("//*[@id=\"sign-in\"]"));
            var elem = _curSign.FirstOrDefault();
            if (elem != null)
            {
                elem.Click();
                // //*[@id="username-existing"]
                ReadOnlyCollection<IWebElement> _login = session.FindElements(By.XPath("//*[@id=\"username-existing\"]"), true, 10, 1);
                if (_login.FirstOrDefault() != null)
                {
                    _login.FirstOrDefault().SendKeys(login);

                }
                else
                {
                    _tracer.TraceEvent(TraceEventType.Warning, 0, "Can not find login element");
                    return false;
                }

                // //*[@id="password-existing"]
                ReadOnlyCollection<IWebElement> _password = session.FindElements(By.XPath("//*[@id=\"password-existing\"]"), true,10, 1);
                if (_password.FirstOrDefault() != null)
                {
                    _password.FirstOrDefault().SendKeys(password);
                }
                else
                {
                    _tracer.TraceEvent(TraceEventType.Warning, 0, "Can not find password element");
                    return false;
                }

                // //*[@id="signin_button"]
                ReadOnlyCollection<IWebElement> _signinButton = session.FindElements(By.XPath("//*[@id=\"signin-button\"]"));
                if (_signinButton.FirstOrDefault() != null)
                {
                    _signinButton.FirstOrDefault().Submit();
                }
                else
                    return false;
            }
            return true;
        }

        public bool isLoggedIn()
        {
            //< a id = "sign-in" class="inline-overlay-trigger signin-header-action select-signin" rel="nofollow" href="/signin?ref=hdr&amp;from_action=signin-header&amp;from_page=https%3A%2F%2Fwww.etsy.com%2Fteams" title="Войти" role="button">
            // Sign in
            //</a>
            ReadOnlyCollection<IWebElement> _curSign = session.WebDriver.FindElements(By.XPath("//*[@id=\"sign-in\"]"));
            var elem = _curSign.FirstOrDefault();
            return elem == null ? true : !elem.Displayed;
        }

        private string _login;
        private string _password;
    }
}
