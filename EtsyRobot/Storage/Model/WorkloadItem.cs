using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EtsyRobot.Storage.Model
{
	[DataContract]
	public class WorkloadItem
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[DataMember(Name = "id")]
		public int ID { get; set; }

		[DataMember(Name = "workloadId")]
		public int WorkloadID { get; set; }

		[Required]
		[MaxLength(2048)]
		[DataMember(Name = "url")]
		public string Url { get; set; }

		[Required]
		[DataMember(Name = "order")]
		public int Order { get; set; }

		public virtual Workload Workload { get; set; }
	}
}