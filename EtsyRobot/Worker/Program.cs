using System;
using System.Linq;
using System.Threading.Tasks;


using EtsyRobot.Storage.Infrastructure;
using EtsyRobot.Properties;

namespace EtsyRobot.Worker
{
	internal class Program
	{
		static private void Run(string[] args) //Main(string[] args)
		{
            using (var db = new CoreContext())
			{
				db.Database.Initialize(false);
			}

			bool isTestScraping = args.Any(a => a.Equals("-testScraping", StringComparison.InvariantCultureIgnoreCase));
            // Create scrapingWorker threads
			var scrapingWorker = new Worker(
				isTestScraping ? JobProducer.CreateTestScrapingProducer() : JobProducer.CreateReferenceScrapingProducer(),
				isTestScraping ? JobHandler.CreateTestScrapingHandler() : JobHandler.CreateGameHandler(),
				Environment.ProcessorCount);
            
			Task.Run(() => scrapingWorker.Run()).ContinueWith(
				t => onWorkerFail(t.Exception),
				TaskContinuationOptions.OnlyOnFaulted);
            // Create comparisonWorker threads
            var comparisonWorker = new Worker(JobProducer.CreateComparisonProducer(), JobHandler.CreateComparisonHandler(),
                                              Environment.ProcessorCount);

            Task.Run(() => comparisonWorker.Run()).ContinueWith(
                t => { Console.WriteLine(@"Comparison worker exited with exception: {0}", t.Exception); 
                    Environment.Exit(2);    
                },
                TaskContinuationOptions.OnlyOnFaulted);

			Console.WriteLine(@"Worker started as a console application.");
			Console.WriteLine(@"Press any key to quit...");
			Console.ReadKey();
		}

        private static void onWorkerFail(AggregateException e)
        {
            Console.WriteLine(@"Critical error. Scraping worker exited with exception: {0}", e);
            Environment.Exit(1);
        }
	}
}