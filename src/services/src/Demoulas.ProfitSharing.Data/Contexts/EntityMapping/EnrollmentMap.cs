using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class EnrollmentMap : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollment");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasMany(e => e.Profits).WithOne(p => p.Enrollment);

        builder.HasData(
            new Enrollment { Id = Enrollment.Constants.Not_Enrolled, Description = "Not Enrolled" },
            new Enrollment { Id = Enrollment.Constants.Old_Vesting_Plan_Has_Contributions, Description = "Old vesting plan has Contributions (7 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.New_Vesting_Plan_Has_Contributions, Description = "New vesting plan has Contributions (6 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.Old_Vesting_Plan_Has_Forfeiture_Records, Description = "Old vesting plan has Forfeiture records" },
            new Enrollment { Id = Enrollment.Constants.New_Vesting_Plan_Has_Forfeiture_Records, Description = "New vesting plan has Forfeiture records" }
        );
    }
}
