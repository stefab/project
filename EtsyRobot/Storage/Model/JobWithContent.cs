using System;

using EtsyRobot.Engine.PageModel;
using EtsyRobot.Storage.Framework.Utils;

namespace EtsyRobot.Storage.Model
{
	public sealed class JobWithContent : Job
	{
		public JobWithContent(Job job, PageContent referenceContent, PageContent testContent)
		{
			this.ID = job.ID;
			this.Url = job.Url;
			this.IssuedBy = job.IssuedBy;
			this.IssuedAt = job.IssuedAt;
			this.CompletedAt = job.CompletedAt;
			this.Status = job.Status;

			this.HasReferenceScraped = job.HasReferenceScraped;
			this.HasTestScraped = job.HasTestScraped;

			this.Issue = job.Issue;
			this.ErrorMessage = job.ErrorMessage;
			this.RowVersion = job.RowVersion;
            this.ReferenceScrapingHost = job.ReferenceScrapingHost;
            this.TestScrapingHost = job.TestScrapingHost;
            this.AdsInfo = job.AdsInfo;
            this.Priority = job.Priority;

			foreach (JobStep step in job.Steps)
			{
				this.Steps.Add(step);
			}

			if (this.HasReferenceScraped && referenceContent == null)
			{
				throw new ArgumentNullException("referenceContent", @"Reference content must be provided.");
			}
			this.ReferenceContent = referenceContent;

			if (this.HasTestScraped && testContent == null)
			{
				throw new ArgumentNullException("testContent", @"Test content must be provided.");
			}
			this.TestContent = testContent;
		}

		public PageContent ReferenceContent { get; private set; }
		public PageContent TestContent { get; private set; }

        
		public void SetReferenceContent(PageContent content)
		{
			Guard.AgainstNull(content, "content");

			if (Status != JobStatus.Incomplete)
			{
				throw new InvalidOperationException("Cannot change a completed job.");
			}

			this.HasReferenceScraped = true;
			this.ReferenceContent = content;
		}

		public void SetTestContent(PageContent content)
		{
			Guard.AgainstNull(content, "content");

			if (Status != JobStatus.Incomplete)
			{
				throw new InvalidOperationException("Cannot change a completed job.");
			}

			this.HasTestScraped = true;
			this.TestContent = content;
		}
	}
}