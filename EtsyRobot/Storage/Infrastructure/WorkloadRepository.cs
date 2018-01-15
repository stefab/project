using System;
using System.Data.Entity;
using System.Linq;

using EtsyRobot.Storage.Model;

namespace EtsyRobot.Storage.Infrastructure
{
	public sealed class WorkloadRepository : Repository<Workload>
	{
		public WorkloadRepository(DbContext dbContext) : base(dbContext)
		{}

		public override Workload Load(int id)
		{
			return this.CreateQuery().FirstOrDefault(e => e.ID == id);
		}

		public override void Update(Workload entity)
		{
			Workload persistent = this.Load(entity.ID);
			this.DbContext.Entry(persistent).CurrentValues.SetValues(entity);
			this.DbContext.Entry(persistent).State = EntityState.Modified;

			foreach (WorkloadItem item in persistent.Items.Where(s => entity.Items.All(cs => cs.Url != s.Url)).ToArray())
			{
				persistent.Items.Remove(item);
				this.DbContext.Entry(item).State = EntityState.Deleted;
			}

			foreach (WorkloadItem item in entity.Items.Where(s => persistent.Items.All(cs => cs.Url != s.Url)).ToArray())
			{
				persistent.Items.Add(item);
			}

			foreach (WorkloadItem item in persistent.Items)
			{
				WorkloadItem currentValues = entity.Items.Single(s => s.Url == item.Url);
				this.DbContext.Entry(item).CurrentValues.SetValues(currentValues);
			}
		}

		protected override IQueryable<Workload> CreateQuery()
		{
			return base.CreateQuery().Include(e => e.Items);
		}
	}
}