using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.OracleHcm;

internal sealed class JobMap : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("Job");

        builder.Property(b => b.RequestedBy).HasMaxLength(30);
        builder.Property(b => b.StartMethod).IsRequired().HasPrecision(2);
        builder.Property(b => b.JobType).IsRequired().HasPrecision(2);
        builder.Property(b => b.JobStatusId).HasPrecision(3);
        builder.Property(b => b.Started).IsRequired().HasColumnType("DATE");
        builder.Property(b => b.Completed).HasColumnType("DATE");

        builder.HasOne(b => b.JobStatus).WithMany(j => j.Jobs).HasForeignKey(j => j.JobStatusId);
    }
}
