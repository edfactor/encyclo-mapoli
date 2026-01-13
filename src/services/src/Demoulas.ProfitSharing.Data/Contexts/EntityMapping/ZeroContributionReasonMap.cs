using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ZeroContributionReasonMap : IEntityTypeConfiguration<ZeroContributionReason>
{
    public void Configure(EntityTypeBuilder<ZeroContributionReason> builder)
    {
        builder.ToTable("ZERO_CONTRIBUTION_REASON");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("ID");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100).HasColumnName("NAME");

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
#pragma warning disable CS0618 // Type or member is obsolete
                Id = ZeroContributionReason.Constants.Over64WithLess1000Hours1YearVesting,
#pragma warning restore CS0618 // Type or member is obsolete
                Name = "OVER 64 AND < 1000 HOURS GETS 1 YEAR VESTING (obsolete 11/20)"
            },
            new PayFrequency
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Id = ZeroContributionReason.Constants.Over64WithLess1000Hours2YearsVesting,
#pragma warning restore CS0618 // Type or member is obsolete
                Name = "OVER 64 AND < 1000 HOURS GETS 2 YEARS VESTING (obsolete 11/20)"
            },
            new PayFrequency
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Id = ZeroContributionReason.Constants.Over64WithOver1000Hours3YearsVesting,
#pragma warning restore CS0618 // Type or member is obsolete
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
            },
            new PayFrequency
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Id = ZeroContributionReason.Constants.Unknown,
#pragma warning restore CS0618 // Type or member is obsolete
                Name = "Unknown"
            }
        );
    }
}
