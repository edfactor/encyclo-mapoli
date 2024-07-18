using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class EmploymentStatusMap : IEntityTypeConfiguration<EmploymentStatus>
{
    public void Configure(EntityTypeBuilder<EmploymentStatus> builder)
    {
        builder.ToTable("EmploymentStatus");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(20);

        builder.HasMany(b => b.Demographics).WithOne(p => p.EmploymentStatus);

        builder.HasData(
            new EmploymentStatus { Id = EmploymentStatus.Constants.Active, Name = "Active" },
            new EmploymentStatus { Id = EmploymentStatus.Constants.Delete, Name = "Delete" },
            new EmploymentStatus { Id = EmploymentStatus.Constants.Inactive, Name = "Inactive" },
            new EmploymentStatus { Id = EmploymentStatus.Constants.Terminated, Name = "Terminated" }
        );
    }
}
