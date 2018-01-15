using System;
using System.Data.Entity;
using System.Linq;

using EtsyRobot.Storage.Model;

namespace EtsyRobot.Storage.Infrastructure
{
	public sealed class JobRepository : Repository<Job>
	{
		public JobRepository(DbContext dbContext) : base(dbContext)
		{}

		public override Job Load(int id)
		{
			return this.CreateQuery().FirstOrDefault(e => e.ID == id);
		}

		public override void Update(Job entity)
		{
			Job persistent = this.Load(entity.ID);
			persistent.RowVersion = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
			this.DbContext.Entry(persistent).CurrentValues.SetValues(entity);
			this.DbContext.Entry(persistent).State = EntityState.Modified;

			foreach (JobStep step in persistent.Steps.Where(s => entity.Steps.All(cs => cs.StepType != s.StepType)).ToArray())
			{
				persistent.Steps.Remove(step);
				DbContext.Entry(step).State = EntityState.Deleted;
			}

			foreach (JobStep step in entity.Steps.Where(s => persistent.Steps.All(cs => cs.StepType != s.StepType)).ToArray())
			{
				persistent.Steps.Add(step);
			}

			foreach (JobStep step in persistent.Steps)
			{
				JobStep currentValues = entity.Steps.Single(s => s.StepType == step.StepType);
				this.DbContext.Entry(step).CurrentValues.SetValues(currentValues);
			}
		}

		protected override IQueryable<Job> CreateQuery()
		{
			return base.CreateQuery().Include(e => e.Steps);
		}
	}
}