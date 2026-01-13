using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Scheduling;

internal sealed class JobStatusMap : IEntityTypeConfiguration<JobStatus>
{
    public void Configure(EntityTypeBuilder<JobStatus> builder)
    {
        builder.ToTable("JobStatus");

        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id).HasPrecision(3);
        builder.Property(b => b.Name).HasMaxLength(30);
        builder.HasMany(b => b.Jobs).WithOne(j => j.JobStatus).HasForeignKey(j => j.JobStatusId);


        builder.HasData(
            new JobStatus { Id = JobStatus.Constants.Pending, Name = "Pending" },
            new JobStatus { Id = JobStatus.Constants.Completed, Name = "Completed" },
            new JobStatus { Id = JobStatus.Constants.Failed, Name = "Failed" },
            new JobStatus { Id = JobStatus.Constants.Running, Name = "Running" }
        );
    }
}
