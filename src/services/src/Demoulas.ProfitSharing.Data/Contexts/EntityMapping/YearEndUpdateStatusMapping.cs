using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class YearEndUpdateStatusMapping : ModifiedBaseMap<YearEndUpdateStatus>
{
    public override void Configure(EntityTypeBuilder<YearEndUpdateStatus> builder)
    {
        _ = builder.ToTable("YE_UPDATE_STATUS");
        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasPrecision(9)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(e => e.ProfitYear)
            .HasPrecision(4)
            .ValueGeneratedNever()
            .HasColumnName("PROFIT_YEAR");

        _ = builder
            .HasIndex(e => e.ProfitYear, "IX_YE_UPDATE_STATUS_PROFIT_YEAR")
            .IsUnique();


        _ = builder.Property(e => e.BeneficiariesEffected)
            .HasColumnName("BENEFICIARIES_EFFECTED")
            .HasPrecision(6) // 1 million members
            .IsRequired();

        _ = builder.Property(e => e.EmployeesEffected)
            .HasColumnName("EMPLOYEES_EFFECTED")
            .HasPrecision(6) // 1 million members
            .IsRequired();

        _ = builder.Property(e => e.EtvasEffected)
            .HasColumnName("ETVAS_EFFECTED")
            .HasPrecision(6) // 1 million members
            .IsRequired();

        _ = builder.Property(e => e.ContributionPercent)
            .HasColumnName("CONTRIBUTION_PERCENT")
            .HasPrecision(9, 6) // PIC S999V999999   7.448821 percent 
            .IsRequired();

        _ = builder.Property(e => e.IncomingForfeitPercent)
            .HasColumnName("INCOMING_FORFEIT_PERCENT")
            .HasPrecision(9, 6) // PIC S999V999999   
            .IsRequired();

        _ = builder.Property(e => e.EarningsPercent)
            .HasColumnName("EARNINGS_PERCENT")
            .HasPrecision(9, 6) // PIC S999V999999   
            .IsRequired();

        _ = builder.Property(e => e.SecondaryEarningsPercent)
            .HasColumnName("SECONDARY_EARNINGS_PERCENT")
            .HasPrecision(9, 6) // PIC S999V999999   
            .IsRequired();

        _ = builder.Property(e => e.MaxAllowedContributions)
            .HasColumnName("MAX_ALLOWED_CONTRIBUTIONS")
            .HasPrecision(6) // 1 million - roughly $75,000 is the max allowed for the 2025 tax year.
            .IsRequired();

        _ = builder.Property(e => e.BadgeAdjusted)
            .HasColumnName("BADGE_ADJUSTED")
            .HasPrecision(7);

        _ = builder.Property(e => e.BadgeAdjusted2)
            .HasColumnName("BADGE_ADJUSTED2")
            .HasPrecision(7);

        _ = builder.Property(e => e.AdjustContributionAmount)
            .HasColumnName("ADJUST_CONTRIBUTION_AMOUNT")
            .HasPrecision(5, 2); // Usually only pennies


        _ = builder.Property(e => e.AdjustEarningsAmount)
            .HasColumnName("ADJUST_EARNINGS_AMOUNT")
            .HasPrecision(5, 2); // Usually only pennies

        _ = builder.Property(e => e.AdjustIncomingForfeitAmount)
            .HasColumnName("ADJUST_INCOMING_FORFEIT_AMOUNT")
            .HasPrecision(5, 2); // Usually only pennies

        _ = builder.Property(e => e.AdjustEarningsSecondaryAmount)
            .HasColumnName("ADJUST_EARNINGS_SECONDARY_AMOUNT")
            .HasPrecision(5, 2); // Usually only pennies

        _ = builder.Property(e => e.IsYearEndCompleted)
            .HasColumnName("IS_YEAR_END_COMPLETED");

        base.Configure(builder);
    }
}
