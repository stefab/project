using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtsyRobot.Storage.Model
{
	public class JobStep
	{
		[Key]
		public int ID { get; set; }

		[Required, ForeignKey("Job")]
		public int JobID { get; set; }

		[Required]
		public StepType StepType { get; set; }

		public virtual Job Job { get; set; }
	}
}