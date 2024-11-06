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

        _ = builder.HasKey(e => new { e.DemographicId, e.ProfitYear});

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(11)
            .ValueGeneratedNever()
            .HasColumnName("DEMOGRAPHIC_ID");

        _ = builder.Property(e => e.ProfitYear)
            .HasPrecision(4)
            .ValueGeneratedNever()
            .HasColumnName("PROFIT_YEAR");

        _ = builder.Property(e => e.CurrentHoursYear)
            .HasPrecision(6, 2)
            .HasColumnName("CURRENT_HOURS_YEAR")
            .IsRequired();

        _ = builder.Property(e => e.CurrentIncomeYear)
            .HasPrecision(9, 2)
            .HasColumnName("CURRENT_INCOME_YEAR")
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

        _ = builder.Property(e => e.WeeksWorkedYear)
            .HasPrecision(2)
            .HasColumnName("WEEKS_WORKED_YEAR")
            .IsRequired();
   
        _ = builder.Property(e => e.PsCertificateIssuedDate)
            .HasColumnName("PS_CERTIFICATE_ISSUED_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.IncomeExecutive)
            .HasColumnName("INCOME_EXECUTIVE")
            .HasPrecision(9, 2);

        _ = builder.Property(e => e.HoursExecutive)
            .HasColumnName("HOURS_EXECUTIVE")
            .HasPrecision(6, 2);

        _ = builder.Property(e => e.LastUpdate)
            .HasColumnName("LAST_UPDATE")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();


        _ = builder.Property(e => e.ZeroContributionReasonId)
            .HasColumnName("ZERO_CONTRIBUTION_REASON_ID");

        _ = builder.Property(e => e.EnrollmentId)
            .HasColumnName("ENROLLMENT_ID");

        _ = builder.Property(e => e.BeneficiaryTypeId)
            .HasColumnName("BENEFICIARY_TYPE_ID");

        _ = builder.Property(e => e.EmployeeTypeId)
            .HasColumnName("EMPLOYEE_TYPE_ID");
        

        _ = builder.Property(e => e.PointsEarned)
            .HasColumnName("POINTS_EARNED")
            .HasPrecision(9, 2);

        _ = builder.HasOne(e => e.Enrollment)
            .WithMany()
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = builder.HasOne(e => e.BeneficiaryType)
            .WithMany()
            .HasForeignKey(p => p.BeneficiaryTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = builder.HasOne(e => e.EmployeeType)
            .WithMany()
            .HasForeignKey(p => p.EmployeeTypeId)
            .OnDelete(DeleteBehavior.NoAction);
            
       _ = builder.HasOne(d => d.ZeroContributionReason)
            .WithMany()
            .HasForeignKey(d => d.ZeroContributionReasonId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
