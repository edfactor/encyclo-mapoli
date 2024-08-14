using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class EnrollmentMap : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("ENROLLMENT");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasMany(e => e.Profits).WithOne(p => p.Enrollment);

        builder.HasData(
            new Enrollment { Id = Enrollment.Constants.Not_Enrolled, Name = "Not Enrolled" },
            new Enrollment { Id = Enrollment.Constants.Old_Vesting_Plan_Has_Contributions, Name = "Old vesting plan has Contributions (7 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.New_Vesting_Plan_Has_Contributions, Name = "New vesting plan has Contributions (6 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.Old_Vesting_Plan_Has_Forfeiture_Records, Name = "Old vesting plan has Forfeiture records" },
            new Enrollment { Id = Enrollment.Constants.New_Vesting_Plan_Has_Forfeiture_Records, Name = "New vesting plan has Forfeiture records" }
        );
    }
}
