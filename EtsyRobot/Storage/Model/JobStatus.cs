using System;
using System.ComponentModel.DataAnnotations;

namespace EtsyRobot.Storage.Model
{
	public enum JobStatus
	{
		[Display(Name = "Not completed")]
		Incomplete = 0,

		[Display(Name = "Completed without issues")]
		CompletedNoIssues = 1,

		[Display(Name = "Completed with issues")]
		CompletedWithIssues = 2,

		[Display(Name = "Failed")]
		Failed = 3
	}
}