using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Scheduling;

internal sealed class JobTypeMap : IEntityTypeConfiguration<JobType>
{
    public void Configure(EntityTypeBuilder<JobType> builder)
    {
        builder.ToTable("JobType");

        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id).HasPrecision(3);
        builder.Property(b => b.Name).HasMaxLength(30);
        builder.HasMany(b => b.Jobs).WithOne(j => j.JobType).HasForeignKey(j => j.JobTypeId);


        builder.HasData(
            new JobType { Id = JobType.Constants.EmployeeSyncFull, Name = "Employee Sync Full" },
            new JobType { Id = JobType.Constants.PayrollSyncFull, Name = "Payroll Sync Full" },
            new JobType { Id = JobType.Constants.EmployeeSyncDelta, Name = "Employee Sync Delta" }
        );
    }
}
