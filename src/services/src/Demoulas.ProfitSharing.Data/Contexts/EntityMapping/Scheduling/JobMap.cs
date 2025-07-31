using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Scheduling;

internal sealed class JobMap : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("Job");

        builder.Property(b => b.RequestedBy).HasMaxLength(30);
        builder.Property(b => b.JobTypeId).IsRequired().HasPrecision(3);
        builder.Property(b => b.JobStatusId).IsRequired().HasPrecision(3);
        builder.Property(b => b.StartMethodId).IsRequired().HasPrecision(3);
        builder.Property(b => b.Started).IsRequired().HasColumnType("TIMESTAMP WITH TIME ZONE");
        builder.Property(b => b.Completed).HasColumnType("TIMESTAMP WITH TIME ZONE");

        builder.HasOne(b => b.JobStatus).WithMany(j => j.Jobs).HasForeignKey(j => j.JobStatusId);
        builder.HasOne(b => b.StartMethod).WithMany(j => j.Jobs).HasForeignKey(j => j.StartMethodId);
        builder.HasOne(b => b.JobType).WithMany(j => j.Jobs).HasForeignKey(j => j.JobTypeId);
    }
}
