using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;

using EtsyRobot.Engine.PageModel;
//using EtsyRobot.Engine.Scraping;
using EtsyRobot.Storage;
using System.Threading;
using EtsyRobot.Engine.WebSession;
using EtsyRobot.Storage.Model;
using EtsyRobot.Engine.WebSession.EtsyUtils;

namespace EtsyRobot.Worker
{
    internal class GameHandler : JobHandler
    {
        public GameHandler(bool isReferenceScraping)
        {
            this._isReferenceScraping = isReferenceScraping;
        }

        static public string getWorkerName()
        {
            return System.Environment.MachineName + ", " + System.Environment.OSVersion;
        }

        public override void Handle(Job job)
        {
            var factory = new BrowserSessionFactory();
            BrowserSettings settings = BrowserSettings.CreateDefault();

            PageContent content = null;
            Exception scrapingException = null;
            try
            {
                IBrowserSession session = null; 
                try
                {
                    session = settings is RemoteBrowserSettings
                                              ? factory.CreateRemote((RemoteBrowserSettings)settings)
                                              : factory.Create(settings);
                    //content = session.Scrape(new Uri(job.Url), this._isReferenceScraping);
                    EtsyStrategy strategy = new EtsyGameStrategy(job);
                    session.ProcessJob(strategy);
                    //strategy.process()

                }
                finally
                {
                    try
                    { 
                        if (session != null) {
                            session.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Sometimes session disposing generates an error.
                        throw new ApplicationException("Session was not closed correctly.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                scrapingException = ex;
                _tracer.TraceData(TraceEventType.Warning, 0,
                                  string.Format("Error when scraping page for (ID = {0}).\r\n", job.ID), ex);
            }

            //while (true)
            //{
            //    // Reload the job. It may be changed since first loading by other worker.
            //    JobWithContent jobWithContent = new JobManager().LoadWithContent(job.ID);
            //    if (this._isReferenceScraping)
            //    {
            //        jobWithContent.ReferenceScrapingHost = getWorkerName();
            //    }
            //    else
            //    {
            //        jobWithContent.TestScrapingHost = getWorkerName();
            //    }

            //    // Job has already been failed by other worker. No actions required.
            //    if (jobWithContent.Status == JobStatus.Failed)
            //    {
            //        return;
            //    }

            //    if (scrapingException == null)
            //    {
            //        Debug.Assert(content != null);
            //        if (this._isReferenceScraping)
            //        {
            //            jobWithContent.SetReferenceContent(content);
            //        }
            //        else
            //        {
            //            jobWithContent.SetTestContent(content);
            //        }
            //    }
            //    else
            //    {
            //        jobWithContent.SetFailed(scrapingException.Message);
            //    }

            //    try
            //    {
            //        new JobManager().SaveJobWithContent(jobWithContent);
            //        _tracer.TraceEvent(TraceEventType.Verbose, 0, "{0} scraping for job (ID = {1}) finished {2}.",
            //                           this._isReferenceScraping ? "Reference" : "Test", job.ID,
            //                           job.Status == JobStatus.Failed ? "with errors" : "successfully.");
            //        return;
            //    }
            //    catch (DbUpdateException)
            //    {
            //        _tracer.TraceInformation("Job (ID = {0}) might been changed by other worker. Retrying the operation...", job.ID);
            //    }
            //}
        }

        static private readonly TraceSource _tracer = new TraceSource("Worker.JobHandlers", SourceLevels.All);

        private readonly bool _isReferenceScraping;
    }
}