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
        public virtual void process() { }
    }
}
