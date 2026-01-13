using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class EmploymentTypeMap : IEntityTypeConfiguration<EmploymentType>
{
    public void Configure(EntityTypeBuilder<EmploymentType> builder)
    {
        builder.ToTable("EMPLOYMENT_TYPE");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasData(
            new EmploymentType { Id = EmploymentType.Constants.PartTime, Name = "PartTime" },
            new EmploymentType { Id = EmploymentType.Constants.FullTimeStraightSalary, Name = "FullTimeStraightSalary" },
            new EmploymentType { Id = EmploymentType.Constants.FullTimeAccruedPaidHolidays, Name = "FullTimeAccruedPaidHolidays" },
            new EmploymentType { Id = EmploymentType.Constants.FullTimeEightPaidHolidays, Name = "FullTimeEightPaidHolidays" }
        );
    }
}
