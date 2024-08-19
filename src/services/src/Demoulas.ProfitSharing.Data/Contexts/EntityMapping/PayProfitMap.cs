using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class PayProfitMap : IEntityTypeConfiguration<PayProfit>
{
    public void Configure(EntityTypeBuilder<PayProfit> builder)
    {
        _ = builder.ToTable("PAY_PROFIT");

        _ = builder.HasKey(e => e.BadgeNumber);

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnName("BADGE_NUMBER");

        _ = builder.Property(e => e.SSN)
            .HasPrecision(9)
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

        _ = builder.Property(e => e.IncomeCurrentYear)
            .HasColumnName("INCOME_CURRENT_YEAR")
            .HasPrecision(9, 2)
            .IsRequired();

        _ = builder.Property(e => e.IncomeLastYear)
            .HasColumnName("INCOME_LAST_YEAR")
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

        _ = builder.Property(d => d.CertificateIssuedLastYear)
            .HasColumnType("NUMBER(1)")
            .HasDefaultValue(0)
            .HasColumnName("CERTIFICATE_ISSUED_LAST_YEAR");

        _ = builder.Property(e => e.PSCertificateIssuedDate)
            .HasColumnName("PS_CERTIFICATE_ISSUED_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.InitialContributionYear)
            .HasColumnName("INITIAL_CONTRIBUTION_YEAR")
            .HasPrecision(4);

        _ = builder.Property(e => e.NetBalanceLastYear)
            .HasColumnName("NET_BALANCE_LAST_YEAR")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.EarningLastYear)
            .HasColumnName("EARNINGS_LAST_YEAR")
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
        
        _ = builder.Property(e => e.IncomeExecutive)
            .HasColumnName("INCOME_EXECUTIVE")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.HoursExective)
            .HasColumnName("HOURS_EXECUTIVE")
            .HasPrecision(6, 2);

        _ = builder.Property(e => e.ZeroContributionReasonId)
            .HasColumnName("ZERO_CONTRIBUTION_REASON_ID");

        _ = builder.Property(e => e.EnrollmentId)
            .HasColumnName("ENROLLMENT_ID");

        _ = builder.Property(e => e.BeneficiaryTypeId)
            .HasColumnName("BENEFICIARY_ID");

        _ = builder.Property(e => e.EmployeeTypeId)
            .HasColumnName("EMPLOYEE_TYPE_ID");


        _ = builder.HasOne(e => e.Enrollment)
            .WithMany(p => p.Profits)
            .HasForeignKey(p => p.EnrollmentId);

        _ = builder.HasOne(e => e.BeneficiaryType)
            .WithMany(p => p.Profits)
            .HasForeignKey(p => p.BeneficiaryTypeId);

        _ = builder.HasOne(e => e.EmployeeType)
            .WithMany(p => p.Profits)
            .HasForeignKey(p => p.EmployeeTypeId);
            
       _ = builder.HasOne(d => d.ZeroContributionReason)
            .WithMany(p => p.Profits)
            .HasForeignKey(d => d.ZeroContributionReasonId);
    }
}
