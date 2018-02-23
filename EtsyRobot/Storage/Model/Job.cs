using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using EtsyRobot.Engine.Comparing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EtsyRobot.Engine.Comparing
{

    public enum ComparisonIssue
    {
        [Display(Name = "No differences")]
        NoDifferences = 0,

        [Display(Name = "Blocking site's static content")]
        StaticContentBlocked = 1,

        [Display(Name = "Video blocked")]
        VideoBlocked = 2,

        [Display(Name = "PrivDog ad of different dimension compare to original ad")]
        DifferentAdDimensions = 3,

        [Display(Name = "Appearance of ad where no ad was present originally")]
        WrongAdPosition = 4,

        [Display(Name = "Overlapping ads")]
        AdsOverlapped = 5,

        [Display(Name = "Breaking site loading")]
        PageLoadingBroken = 6,

        [Display(Name = "Breaking site structure")]
        PageStructureBroken = 7
    }
    public sealed class ComparisonResult
    {
        public ComparisonResult(ComparisonIssue issue, string details)
        {
            this.Issue = issue;
            this.Details = details;
        }

        public bool IsSuccess
        {
            get { return this.Issue == ComparisonIssue.NoDifferences; }
        }

        public ComparisonIssue Issue { get; private set; }
        public string Details { get; private set; }

        static public readonly ComparisonResult Success = new ComparisonResult(ComparisonIssue.NoDifferences, null);
    }
}

namespace EtsyRobot.Storage.Model
{
	public class Job
	{
		public Job()
		{
			this.Steps = new HashSet<JobStep>();
		}
        public int igor { get; set; }
        public int igor2 { get; set; }
        public string EtsyUser2 { get; set; }
        public string EtsyUser { get; set; }
        public string Password  { get; set; }

		public int ID { get; set; }
		public string Url { get; set; }
		public string IssuedBy { get; set; }
		public DateTime IssuedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
		public JobStatus Status { get; set; }

		public bool HasReferenceScraped { get; set; }
		public bool HasTestScraped { get; set; }

		public ComparisonIssue? Issue { get; set; }
		public string IssueDetails { get; set; }
		public string ErrorMessage { get; set; }
		public byte[] RowVersion { get; set; }
		public double? ExecutionTime { get; set; }

        [StringLength(256)]
        public string TestScrapingHost { get; set; }

        [StringLength(256)]
        public string ReferenceScrapingHost { get; set; }
        
        public string AdsInfo { get; set; }
        
        public int Priority { get; set; } 
		public virtual ICollection<JobStep> Steps { get; private set; }

		static public Job Create(string url)
		{
			return new Job
				{
					Url = url,
					IssuedBy = Thread.CurrentPrincipal.Identity.Name,
					IssuedAt = DateTime.UtcNow,
					Status = JobStatus.Incomplete,
					RowVersion = BitConverter.GetBytes(DateTime.UtcNow.ToBinary()),
					Steps =
						{
							new JobStep {StepType = StepType.ReferenceScraping},
							new JobStep {StepType = StepType.TestScraping},
							new JobStep {StepType = StepType.Comparison}
						}
				};
		}

		public void SetCompleted(ComparisonResult comparisonResult)
		{
			if (this.Status != JobStatus.Incomplete)
			{
				throw new InvalidOperationException("This operation can be performed for an incomplete job only.");
			}

			this.Status = comparisonResult.Issue == ComparisonIssue.NoDifferences
				              ? JobStatus.CompletedNoIssues
				              : JobStatus.CompletedWithIssues;
			this.Issue = comparisonResult.Issue;
			this.IssueDetails = comparisonResult.Details;
			this.CompletedAt = DateTime.UtcNow;
		}

		public void SetFailed(string errorMessage)
		{
			if (this.Status != JobStatus.Incomplete)
			{
				throw new InvalidOperationException("This operation can be performed for an incomplete job only.");
			}

			this.Status = JobStatus.Failed;
			this.ErrorMessage = errorMessage;

			this.Steps.Clear();
		}

		public void RemoveStep(StepType stepType)
		{
			JobStep step = this.Steps.FirstOrDefault(s => s.StepType == stepType);
			if (step == null)
			{
				throw new InvalidOperationException("No step found");
			}

			this.Steps.Remove(step);
			step.Job = null;
		}
	}
}