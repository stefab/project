using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace EtsyRobot.Storage.Model
{
	[DataContract]
	public class Workload
	{
		public Workload()
		{
			this.Items = new HashSet<WorkloadItem>();
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[DataMember(Name = "id")]
		public int ID { get; set; }

		[Required]
		[MaxLength(50)]
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "items")]
		public virtual ICollection<WorkloadItem> Items { get; private set; }

		public void AddItem(WorkloadItem item)
		{
			if (this.Items.Count > 0)
			{
				item.Order = this.Items.Max(it => it.Order) + 1;
			}

			this.Items.Add(item);
		}

		public void AddItem(string url)
		{
			this.AddItem(new WorkloadItem {Url = url});
		}
	}
}