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
            // //*[@id="join_neu_email_field"]
            // //*[@id="username-existing"]  //*[@id="join-neu-form"]
            ReadOnlyCollection<IWebElement> _curSign = session.FindElements(By.XPath("//*[@id=\"sign-in\"]"));
            var elem = _curSign.FirstOrDefault();
            if (elem != null)
            {
                elem.Click();
                // //*[@id="username-existing"] Or //*input[@id="join_neu_email_field"]
                ReadOnlyCollection<IWebElement> _login = session.FindElements(By.XPath("//input[@id=\"username-existing\"]|//input[@id=\"join_neu_email_field\"]"), true, 10, 1);
                if (_login.Count() > 0 )// != null)
                {
                    _login.FirstOrDefault().SendKeys(login);
                }
                else
                {
                    _tracer.TraceEvent(TraceEventType.Warning, 0, "Can not find login element");
                    return false;
                }

                // //*[@id="password-existing"] |  //*[@id="join_neu_password_field"]
                ReadOnlyCollection<IWebElement> _password = session.FindElements(By.XPath("//input[@id=\"password-existing\"]|//input[@id=\"join_neu_password_field\"]"), true, 10, 1);
                if (_password.FirstOrDefault() != null)
                {
                    _password.FirstOrDefault().SendKeys(password);
                }
                else
                {
                    _tracer.TraceEvent(TraceEventType.Warning, 0, "Can not find password element");
                    return false;
                }

                // 1. //*[@id="signin_button"] [@type=\"submit\"] and 
                // 2. //*[@id="signin-button"]
                // 3. //*[@id="join-neu-form"]/div[1]/div/div[5]/div/button
                //     < button type = "submit" name = "submit_attempt" value = "sign-in" class="btn btn-large width-full btn-primary">
                ReadOnlyCollection<IWebElement> _signinButton = session.FindElements(By.XPath("(//*[@id=\"signin_button\"] | //*[@id=\"signin-button\"] | //button[@name=\"submit_attempt\"])"));
                // //*[@id="signin-button"]
                if (_signinButton.FirstOrDefault() != null)
                {
                    _signinButton.FirstOrDefault().Submit();
                }
                else
                    return false;
            }
            return true;
        }

        public bool isLoggedIn(double wait=0)
        {
            //< a href = "https://www.etsy.com/uk/people/Alegraflowers?ref=hdr" class="nav-link" title="Your account" aria-label="Your account" role="button" aria-haspopup="true" aria-expanded="false" aria-owns="sub-nav-user-navigation">
            //       <span class="nav-icon nav-icon-image nav-icon-circle" aria-hidden="true">
            //           <img src = "https://img0.etsystatic.com/026/0/46561140/iusa_75x75.26221922_jwrn.jpg" alt="">
            //       </span>
            //
            //       <span class="text-link" aria-hidden="true">
            //           <span class="text-link-copy">
            //               You
            //           </span>
            //           <span class="etsy-icon etsy-icon-dropdown"><svg xmlns = "http://www.w3.org/2000/svg" viewBox="7 10 10 6" aria-hidden="true" focusable="false"><polygon points = "16.5 10 12 16 7.5 10 16.5 10" ></ polygon ></ svg ></ span >
            //         </ span >
            //         < span id="total-user-count" class="total-user-count count hide" aria-hidden="true">0</span>
            //   </a>
            // //*[@id="gnav-header-inner"]/div/ul/li[4]/a =
            // 
            ReadOnlyCollection<IWebElement> _curSign = session.FindElements(By.XPath("//a[contains(@href,'/people/') and @aria-owns=\"sub-nav-user-navigation\"]"), true, 5, 0.5);
            var elem = _curSign.FirstOrDefault();
            return elem != null ? elem.Displayed : false;
        }
        public bool isNotLoggedIn()
        {
            //< a id = "sign-in" class="inline-overlay-trigger signin-header-action select-signin" rel="nofollow" href="/signin?ref=hdr&amp;from_action=signin-header&amp;from_page=https%3A%2F%2Fwww.etsy.com%2Fteams" title="Войти" role="button">
            // Sign in
            //</a>
            ReadOnlyCollection<IWebElement> _curSign = session.FindElements(By.XPath("//*[@id=\"sign-in\"]"), true, 10, 0.5);
            var elem = _curSign.FirstOrDefault();
            return elem == null ? true : !elem.Displayed;
        }
        private string _login;
        private string _password;
    }
}
