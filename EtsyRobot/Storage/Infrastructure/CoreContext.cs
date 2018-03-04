using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EtsyRobot.Storage.Model;


namespace EtsyRobot.Storage.Infrastructure
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class CoreContext : DbContext
	{
		public CoreContext()
			: base("CoreEntities")
		{
            Database.SetInitializer<CoreContext>(new CoreDatabaseInitializer());
        }
        public DbSet<Game> Games { get; set; }
        public  DbSet<Post> Posts { get; set; }
		public DbSet<Job> Jobs { get; set; }
		public DbSet<JobStep> JobSteps { get; set; }
		public DbSet<Workload> Workloads { get; set; }
		public DbSet<WorkloadItem> WorkloadItems { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Configurations.Add(new JobTypeConfiguration());
			modelBuilder.Ignore<JobWithContent>();
		}

    }
}