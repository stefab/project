using System;
using System.Data.Entity.ModelConfiguration;

using EtsyRobot.Storage.Model;

namespace EtsyRobot.Storage.Infrastructure
{
	internal class JobTypeConfiguration : EntityTypeConfiguration<Job>
	{
		public JobTypeConfiguration()
		{
			this.HasKey(e => e.ID);

			this.Property(e => e.Url).IsRequired().HasMaxLength(2048);
			this.Property(e => e.IssuedBy).IsRequired().HasMaxLength(100);
			this.Property(e => e.IssuedAt).IsRequired();
			this.Property(e => e.HasReferenceScraped).IsRequired();
			this.Property(e => e.HasTestScraped).IsRequired();

			this.Property(e => e.RowVersion).IsConcurrencyToken(true);
		}
	}
}