using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ZeroContributionReasonMap : IEntityTypeConfiguration<ZeroContributionReason>
{
    public void Configure(EntityTypeBuilder<ZeroContributionReason> builder)
    {
        builder.ToTable("ZeroContributionReason");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("ID");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100).HasColumnName("NAME");

        builder.HasMany(b => b.Profits).WithOne(p => p.ZeroContributionReason);

        builder.HasData(
            new PayFrequency { Id = ZeroContributionReason.Constants.Normal, Name = "Normal" },
            new PayFrequency { Id = ZeroContributionReason.Constants.Under21WithOver1Khours, Name = "18, 19, OR 20 WITH > 1000 HOURS" },
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested,
                Name = "TERMINATED EMPLOYEE > 1000 HOURS WORKED GETS YEAR VESTED"
            },
#pragma warning disable CS0612 // Type or member is obsolete
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.Over64WithLess1000Hours1YearVesting,
                Name = "OVER 64 AND < 1000 HOURS GETS 1 YEAR VESTING (obsolete 11/20)"
            },
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.Over64WithLess1000Hours2YearsVesting,
                Name = "OVER 64 AND < 1000 HOURS GETS 2 YEARS VESTING (obsolete 11/20)"
            },
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.Over64WithOver1000Hours3YearsVesting,
                Name = "OVER 64 AND > 1000 HOURS GETS 3 YEARS VESTING (obsolete 11/20)"
            },
#pragma warning restore CS0612 // Type or member is obsolete
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                Name = ">=65 AND 1st CONTRIBUTION >= 5 YEARS AGO GETS 100% VESTED"
            },
            new PayFrequency
            {
                Id = ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay,
                Name = "=64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY"
            }
        );
    }
}
