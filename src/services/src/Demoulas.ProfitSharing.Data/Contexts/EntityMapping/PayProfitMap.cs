using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayProfitMap : ModifiedBaseMap<PayProfit>
{
    public override void Configure(EntityTypeBuilder<PayProfit> builder)
    {
        base.Configure(builder);

        _ = builder.ToTable("PAY_PROFIT");

        _ = builder.HasKey(e => new { e.DemographicId, e.ProfitYear });

        _ = builder.HasIndex(e => e.EnrollmentId, "IX_EnrollmentId");
        _ = builder.HasIndex(e => e.ProfitYear, "IX_ProfitYear");
        // Composite index to support frequent filters by year and joins by demographic
        _ = builder.HasIndex(e => new { e.ProfitYear, e.DemographicId }, "IX_ProfitYear_DemographicId");
        // Composite index optimized for correlated subqueries: WHERE DemographicId = ? AND ProfitYear = ?
        _ = builder.HasIndex(e => new { e.DemographicId, e.ProfitYear }, "IX_DemographicId_ProfitYear");

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(9)
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

        _ = builder.Property(e => e.Etva)
            .HasPrecision(9, 2)
            .HasColumnName("ETVA")
            .IsRequired();

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
