using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class PayProfitMap : IEntityTypeConfiguration<PayProfit>
{
    public void Configure(EntityTypeBuilder<PayProfit> builder)
    {
        _ = builder.ToTable("PayProfit");

        _ = builder.HasKey(e => e.PSN);

        _ = builder.Property(e => e.PSN)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnName("PSN");

        _ = builder.Property(e => e.SSN)
            .HasMaxLength(9)
            .IsRequired();

        _ = builder.Property(e => e.HoursCurrentYear)
            .HasPrecision(6, 2)
            .HasColumnName("HOURS_CURRENT_YEAR")
            .IsRequired();

        _ = builder.Property(e => e.HoursLastYear)
            .HasColumnName("HOURS_LAST_YEAR")
            .HasPrecision(6, 2);

        _ = builder.Property(e => e.WeeksWorkedYear)
            .HasPrecision(2)
            .HasColumnName("WEEKS_WORKED_YEAR")
            .IsRequired();

        _ = builder.Property(e => e.WeeksWorkedLastYear)
            .HasColumnName("WEEKS_WORKED_LAST_YEAR")
            .HasPrecision(2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsCurrentYear)
            .HasColumnName("EARNINGS_CURRENT_YEAR")
            .HasPrecision(9, 2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsLastYear)
            .HasColumnName("EARNINGS_LAST_YEAR")
            .HasPrecision(9, 2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsAfterApplyingVestingRules)
            .HasPrecision(9, 2)
            .HasColumnName("EARNINGS_AFTER_APPLYING_VESTING_RULES")
            .IsRequired();

        _ = builder.Property(e => e.EarningsEtvaValue)
            .HasPrecision(9, 2)
            .HasColumnName("EARNINGS_ETVA_VALUE")
            .IsRequired();

        _ = builder.Property(e => e.SecondaryEarnings)
            .HasColumnName("SECONDARY_EARNINGS")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.SecondaryEtvaEarnings)
            .HasColumnName("SECONDARY_ETVA_EARNINGS")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.EarningsPriorEtvaValue)
            .HasColumnName("EARNINGS_PRIOR_ETVA_VALUE")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.CompanyContributionYears)
            .HasColumnName("COMPANY_CONTRIBUTION_YEARS")
            .IsRequired();

        _ = builder.Property(e => e.PSCertificateIssuedDate)
            .HasColumnName("PS_CERTIFICATE_ISSUED_DATE");

        _ = builder.Property(e => e.InitialContributionYear)
            .HasColumnName("INITIAL_CONTRIBUTION_YEAR")
            .HasPrecision(4);

        _ = builder.Property(e => e.NetBalanceLastYear)
            .HasColumnName("NET_BALANCE_LAST_YEAR")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.NumberOfDollarsEarningLastYear)
            .HasColumnName("NUMBER_OF_DOLLARS_EARNING_LAST_YEAR")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.PointsEarnedLastYear)
            .HasColumnName("POINTS_EARNED_LAST_YEAR")
            .HasPrecision(5);

        _ = builder.Property(e => e.VestedBalanceLastYear)
            .HasColumnName("VESTED_BALANCE_LAST_YEAR")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.ContributionAmountLastYear)
            .HasColumnName("CONTRIBUTION_AMOUNT_LAST_YEAR")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.ForfeitureAmountLastYear)
            .HasColumnName("FORFEITURE_AMOUNT_LAST_YEAR")
            .HasPrecision(9, 2);
        
        _ = builder.Property(e => e.ExecutiveEarnings)
            .HasColumnName("EXECUTIVE_EARNINGS")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.ExecutiveHours)
            .HasColumnName("EXECUTIVE_HOURS")
            .HasPrecision(6, 2);
        
        _ = builder.HasOne(e => e.Enrollment)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.BeneficiaryType)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.EmployeeType)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.ZeroContributionReason)
            .WithMany(p => p.Profits);
    }
}
