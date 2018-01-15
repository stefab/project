using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using NLog;

using EtsyRobot.Storage.Model;

namespace EtsyRobot.Worker
{
	internal class Worker
	{
		public Worker(JobProducer producer, JobHandler handler, int maxConcurrentJobs)
		{
			this._producer = producer;
			this._handler = handler;
			this._maxConcurrentJobs = maxConcurrentJobs;
            this._restartAfterJobsCount = -1;
            this._JobsProcessedCount = 0;
		}

		public void Run()
		{
			using (var semaphore = new SemaphoreSlim(this._maxConcurrentJobs))
			{
				foreach (Job job in this._producer.Produce())
				{
					SemaphoreSlim semaphore1 = semaphore;
					Job job1 = job;

					_tracer.TraceInformation("Waiting for a semaphore...");
					semaphore1.Wait();
					Task.Factory.StartNew(() => this.Consume(job1))
						.ContinueWith(t =>
							{
								if (t.Exception != null)
								{
									_tracer.TraceData(TraceEventType.Error, 0, t.Exception);
								}
								semaphore1.Release();
							});
                    this._JobsProcessedCount++;
				}
			}
		}

		private void Consume(Job job)
		{
			try
			{
				_tracer.TraceEvent(TraceEventType.Verbose, 0, "Handler {0} consumed the job (ID = {1}).",
				                   this._handler.GetType().Name, job.ID);

				this._handler.Handle(job);
				_tracer.TraceEvent(TraceEventType.Verbose, 0, "Handler {0} processed the job (ID = {1}).",
				                   this._handler.GetType().Name, job.ID);
			}
			catch (Exception ex)
			{
				string message = string.Format("Handler {0} encountered an error: {1}.", this._handler.GetType().Name, ex.Message);
				_tracer.TraceEvent(TraceEventType.Error, 0, message);
				_log.Error(message);
			}
		}

		static private readonly TraceSource _tracer = new TraceSource("Worker", SourceLevels.All);
		static private readonly Logger _log = LogManager.GetLogger("Worker");

		private readonly JobProducer _producer;
		private readonly JobHandler _handler;
		private readonly int _maxConcurrentJobs;
        private readonly int _restartAfterJobsCount;
        private int _JobsProcessedCount;
	}
}