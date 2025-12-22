using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            .HasMaxLength(84)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasData(
            new Enrollment { Id = Enrollment.Constants.NotEnrolled, Name = "Not Enrolled" },
            new Enrollment { Id = Enrollment.Constants.OldVestingPlanHasContributions, Name = "Old vesting plan has Contributions (7 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.NewVestingPlanHasContributions, Name = "New vesting plan has Contributions (6 years to full vesting)" },
            new Enrollment { Id = Enrollment.Constants.OldVestingPlanHasForfeitureRecords, Name = "Old vesting plan has Forfeiture records" },
            new Enrollment { Id = Enrollment.Constants.NewVestingPlanHasForfeitureRecords, Name = "New vesting plan has Forfeiture records" },
            new Enrollment { Id = Enrollment.Constants.Import_Status_Unknown, Name = "Previous years enrollment is unknown. (History not previously tracked)" }
        );
    }
}
