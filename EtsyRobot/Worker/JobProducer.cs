using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using EtsyRobot.Storage;
using EtsyRobot.Storage.Infrastructure;
using EtsyRobot.Storage.Model;

namespace EtsyRobot.Worker
{
	internal class JobProducer
	{
		private JobProducer(StepType stepType, Expression<Func<Job, bool>> filterExpression)
		{
			this._stepType = stepType;
			this._filterExpression = filterExpression;
		}

		public IEnumerable<Job> Produce()
		{
			while (true)
			{
				Job job;
				try
				{
					_tracer.TraceEvent(TraceEventType.Verbose, 0, "Trying to find a next {0} job...", this._stepType);
					job = this.FindNext();
				}
				catch (Exception ex)
				{
					_tracer.TraceEvent(TraceEventType.Error, 0, "Error occurred when retrieving a job. {0}",
					                   ex.GetBaseException().Message);

					job = null;
				}

				if (job != null)
				{
					_tracer.TraceEvent(TraceEventType.Verbose, 0, "Found a {0} job (ID = {1}).", this._stepType, job.ID);

					yield return job;
				}
				else
				{
					_tracer.TraceEvent(TraceEventType.Verbose, 0, "No awaiting {0} jobs. Sleeping for {1} seconds...",
					                   this._stepType, this._sleepDuration.TotalSeconds);

					Thread.Sleep(this._sleepDuration);
				}
			}
// ReSharper disable FunctionNeverReturns
		}

// ReSharper restore FunctionNeverReturns

		static public JobProducer CreateReferenceScrapingProducer()
		{
			return new JobProducer(StepType.ReferenceScraping,
			                       e => e.Status == JobStatus.Incomplete &&
			                            e.Steps.Any(s => s.StepType == StepType.ReferenceScraping));
		}

		static public JobProducer CreateTestScrapingProducer()
		{
			return new JobProducer(StepType.TestScraping,
			                       e => e.Status == JobStatus.Incomplete &&
			                            e.Steps.Any(s => s.StepType == StepType.TestScraping));
		}

		static public JobProducer CreateComparisonProducer()
		{
			return new JobProducer(StepType.Comparison,
			                       e => e.Status == JobStatus.Incomplete &&
			                            e.HasReferenceScraped && e.HasTestScraped &&
			                            e.Steps.Any(s => s.StepType == StepType.Comparison));
		}

		private Job FindNext()
		{
			Job job;
			bool needRetry;

			do
			{
				needRetry = false;
				using (var context = new CoreContext())
				{
					var repository = new JobRepository(context);
					try
					{
						_tracer.TraceEvent(TraceEventType.Verbose, 0, "Looking for a {0} job...", this._stepType);

                        //job = repository.FindMatching(this._filterExpression).FirstOrDefault();
                        job = repository.FilteredQuery(this._filterExpression).OrderByDescending(e => e.Priority).FirstOrDefault();
					}
					catch (DataException ex) // The job may be deadlocked
					{
						_tracer.TraceInformation("An exception occurred when loading a {0} job.", _stepType);
						_tracer.TraceData(TraceEventType.Information, 0, ex);
						job = null;
					}
				}

				if (job != null)
				{
					_tracer.TraceEvent(TraceEventType.Verbose, 0, "{0} job (ID = {1}) found.", this._stepType, job.ID);

					job.RemoveStep(this._stepType);
					try
					{
						new JobManager().SaveJob(job);
					}
					catch (DbUpdateException ex)
					{
						_tracer.TraceInformation("{0} job (ID = {1}) has been changed by other worker. Retrying the find operation...",
						                         this._stepType, job.ID);
						_tracer.TraceData(TraceEventType.Information, 0, ex);
						needRetry = true;
					}
				}
			} while (needRetry);

			return job;
		}

		static private readonly TraceSource _tracer = new TraceSource("Worker.JobProducer", SourceLevels.All);

		private readonly StepType _stepType;
		private readonly Expression<Func<Job, bool>> _filterExpression;

		private readonly TimeSpan _sleepDuration = TimeSpan.FromSeconds(10);
	}
}