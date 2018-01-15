using System;
using System.Diagnostics;
using System.Transactions;

using EtsyRobot.Engine.PageModel;
using EtsyRobot.Storage.Infrastructure;
using EtsyRobot.Storage.Model;
using EtsyRobot.Properties;
using Newtonsoft.Json;

namespace EtsyRobot.Storage
{
	public sealed class JobManager
	{
		public JobManager() : this(Settings.Default)
		{}

		internal JobManager(Settings settings)
		{
			this._settings = settings;
		}

		public JobWithContent LoadWithContent(int id)
		{
			_tracer.TraceEvent(TraceEventType.Start, 0);
			var contentRepository = new PageContentRepository(this._settings);
			using (contentRepository.AcquireLock(id))
			{
				Job job;
				using (var context = new CoreContext())
				{
					var repository = new JobRepository(context);
					job = repository.Load(id);
				}

				PageContent referenceContent = null;
				if (job.HasReferenceScraped)
				{
					referenceContent = contentRepository.LoadReferenceContent(job.ID);
				}

				PageContent testContent = null;
				if (job.HasTestScraped)
				{
					testContent = contentRepository.LoadTestContent(job.ID);
				}

				_tracer.TraceEvent(TraceEventType.Stop, 0);

				return new JobWithContent(job, referenceContent, testContent);
			}
		}

		public void SaveJobWithContent(JobWithContent job)
		{
			_tracer.TraceEvent(TraceEventType.Start, 0);

			var contentRepository = new PageContentRepository(this._settings);
			using (contentRepository.AcquireLock(job.ID))
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required))
				{
					if (job.HasTestScraped) {
                        string jsonZones = JsonConvert.SerializeObject(job.TestContent.AdZoneNodes);
                        job.AdsInfo = jsonZones;
                    }

                    InternalSaveJob(job);
                    // save content to files
					if (job.HasReferenceScraped)
					{
						contentRepository.SaveReferenceContent(job.ReferenceContent, job.ID);
					}

					if (job.HasTestScraped)
					{
						contentRepository.SaveTestContent(job.TestContent, job.ID);
					}

					scope.Complete();
				}
			}

			_tracer.TraceEvent(TraceEventType.Stop, 0);
		}

		public void SaveJob(Job job)
		{
			_tracer.TraceEvent(TraceEventType.Start, 0);

			// Dispatch if necessary
			var jobWithContent = job as JobWithContent;
			if (jobWithContent != null)
			{
				this.SaveJobWithContent(jobWithContent);
				return;
			}

			InternalSaveJob(job);

			_tracer.TraceEvent(TraceEventType.Stop, 0);
		}

		public void DeleteJobWithContent(int id)
		{
			_tracer.TraceEvent(TraceEventType.Start, 0);

			var contentRepository = new PageContentRepository(this._settings);
			using (contentRepository.AcquireLock(id))
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required))
				{
					Job job;
					using (var context = new CoreContext())
					{
						var repository = new JobRepository(context);
						job = repository.Load(id);
						repository.Delete(job);
						context.SaveChanges();
					}

					if (job.HasReferenceScraped)
					{
						contentRepository.DeleteReferenceContent(id);
					}

					if (job.HasTestScraped)
					{
						contentRepository.DeleteTestContent(id);
					}

					scope.Complete();
				}
			}

			_tracer.TraceEvent(TraceEventType.Stop, 0);
		}

		static private void InternalSaveJob(Job job)
		{
			using (var context = new CoreContext())
			{
				var repository = new JobRepository(context);
				if (job.ID == default(int))
				{
					repository.Add(job);
				}
				else
				{
					repository.Update(job);
				}
				context.SaveChanges();
			}
		}

		static private readonly TraceSource _tracer = new TraceSource("Storage", SourceLevels.All);

		private readonly Settings _settings;
	}
}