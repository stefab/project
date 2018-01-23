using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtsyRobot.Engine.WebSession.EtsyUtils
{
    class EtsyAuthentication :  BrowserSessionHolder
    {
        public EtsyAuthentication(DefaultBrowserSession session) : base()
        {
            base.init(session);
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

        public bool signIn()
        {
            return true;
        }
        public bool isLoggedIn()
        {
            return true;
        }

        private string _login;
        private string _password;
    }
}
