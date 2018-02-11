using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtsyRobot.Storage.Model;
using System.Threading;
using EtsyRobot.Engine.JobModel;

namespace EtsyRobot.Engine.WebSession.EtsyUtils
{
    internal class EtsyStrategy :  BrowserSessionHolder 
    {
        private Job m_job;
        public Job job
        {
            get { return m_job; }
        }
        public EtsyStrategy(Job job) : base()
        {
            m_job = job;
        }
        public void init(DefaultBrowserSession session)
        {
            base.init(session);
        }
        public virtual GameResult process(DefaultBrowserSession session)
        {
            init(session);
            return null;
        }

    }

    internal class EtsyGameStrategy : EtsyStrategy
    {
        public EtsyGameStrategy(Job job) : base(job)
        {
        }
        public override GameResult process(DefaultBrowserSession session)
        {
            base.init(session);
            GameResult result = new GameResult();

            session.LoadPage(new Uri(job.Url));
            //Thread.Sleep(10000);
            session.InjectHelperScripts();
            
            EtsyAuthentication auth = new EtsyAuthentication(session);
            if (!auth.isLoggedIn())
            {
                if(auth.signIn(job.User, job.Password))
                {
                    if (auth.isLoggedIn())
                    {

                    }
                }
                else
                {

                }

            }

            return result;
        }
    }
}
