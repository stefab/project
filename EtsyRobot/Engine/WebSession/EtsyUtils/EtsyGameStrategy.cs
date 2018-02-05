using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtsyRobot.Engine.WebSession.EtsyUtils
{
    internal class EtsyStrategy :  BrowserSessionHolder 
    {
        public EtsyStrategy() : base()
        {
        }
        public void init(DefaultBrowserSession session)
        {
            base.init(session);
        }
        public virtual void process(DefaultBrowserSession session) { init(session); }
    }

    internal class EtsyGameStrategy : EtsyStrategy
    {
        public EtsyGameStrategy() : base()
        {
        }
        public virtual void process(DefaultBrowserSession session)
        {
            init(session);
            EtsyAuthentication auth = new EtsyAuthentication(session);
            if (auth.isLoggedIn())
            {
                auth.signIn("alla", "password");
            }
        }
    }
}
