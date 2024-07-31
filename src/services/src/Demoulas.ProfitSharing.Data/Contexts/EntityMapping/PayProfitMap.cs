using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class PayProfitMap : IEntityTypeConfiguration<PayProfit>
{
    public void Configure(EntityTypeBuilder<PayProfit> builder)
    {
        _ = builder.ToTable("PayProfit");

        _ = builder.HasKey(e => e.EmployeeBadge);

        _ = builder.Property(e => e.EmployeeBadge)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .IsRequired();

        _ = builder.Property(e => e.EmployeeSSN)
            .HasMaxLength(9)
            .IsRequired();

        _ = builder.Property(e => e.HoursCurrentYear)
            .HasPrecision(4, 2)
            .IsRequired();

        _ = builder.Property(e => e.HoursLastYear)
            .HasPrecision(4, 2);

        _ = builder.Property(e => e.WeeksWorkedYear)
            .HasPrecision(2)
            .IsRequired();

        _ = builder.Property(e => e.WeeksWorkedLastYear)
            .HasPrecision(2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsCurrentYear)
            .HasPrecision(8, 2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsLastYear)
            .HasPrecision(8, 2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsAfterApplyingVestingRules)
            .HasPrecision(9, 2)
            .IsRequired();

        _ = builder.Property(e => e.EarningsEtvaValue)
            .HasPrecision(9, 2)
            .IsRequired();

        _ = builder.Property(e => e.SecondaryEarnings)
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.SecondaryEtvaEarnings)
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.EarningsPriorEtvaValue)
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.CompanyContributionYears)
            .IsRequired();

        _ = builder.Property(e => e.PSCertificateIssuedDate);

        _ = builder.Property(e => e.InitialContributionYear)
            .HasPrecision(4);

        _ = builder.Property(e => e.NetBalanceLastYear)
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.NumberOfDollarsEarningLastYear)
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.PointsEarnedLastYear)
            .HasPrecision(5);

        _ = builder.Property(e => e.VestedBalanceLastYear).HasPrecision(9, 2);

        _ = builder.Property(e => e.ContributionAmountLastYear).HasPrecision(9, 2);

        _ = builder.Property(e => e.ForfeitureAmountLastYear).HasPrecision(9, 2);

        _ = builder.Property(e => e.ForfeitureAmountLastYear).HasPrecision(9, 2);
        
        _ = builder.Property(e => e.ExecutiveEarnings)
            .HasPrecision(8, 2);

        _ = builder.Property(e => e.ExecutiveHours)
            .HasPrecision(4, 2);
        
        _ = builder.HasOne(e => e.Enrollment)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.BeneficiaryType)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.EmployeeType)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(e => e.ZeroContributionReason)
            .WithMany(p => p.Profits);

        _ = builder.HasOne(d => d.Demographic)
            .WithMany(p => p.PayProfit)
            .HasForeignKey(p => p.EmployeeSSN);
    }
}
