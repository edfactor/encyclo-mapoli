using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ProfitDetailMap : ModifiedBaseMap<ProfitDetail>
{
    public override void Configure(EntityTypeBuilder<ProfitDetail> builder)
    {
        base.Configure(builder);

        _ = builder.ToTable("PROFIT_DETAIL");
        _ = builder.HasKey(p => p.Id);

        _ = builder.HasIndex(p => new { p.Ssn }, "IX_SSN");
        _ = builder.HasIndex(p => new { p.Ssn, p.ProfitYear }, "IX_SSN_YEAR");
        _ = builder.HasIndex(p => new { p.ProfitYear, p.Ssn }, "IX_PROFITYEAR_SSN"); // Optimizes queries filtering by ProfitYear and grouping by Ssn
        _ = builder.HasIndex(p => new { p.ProfitYear, p.ProfitCodeId }, "IX_PROFIT_CODE_ID_PROFIT_YEAR");
        _ = builder.HasIndex(p => new { p.ProfitYear, p.MonthToDate }, "IX_PROFIT_CODE_ID_MONTHTODATE");
        _ = builder.HasIndex(p => new { p.Ssn, p.ProfitYear, p.ProfitCodeId }, "IX_SSN_YEAR_PROFIT_CODE_ID");
        _ = builder.HasIndex(p => new { p.ProfitCodeId, p.Ssn }, "IX_PROFIT_CODE_SSN"); // Optimizes GetFirstContributionYear query (WHERE PROFIT_CODE_ID = 0 GROUP BY SSN)

        _ = builder.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(x => x.ProfitYear).IsRequired().HasColumnName("PROFIT_YEAR");
        _ = builder.Property(x => x.ProfitYearIteration).IsRequired().HasColumnName("PROFIT_YEAR_ITERATION");
        _ = builder.Property(x => x.ProfitCodeId).IsRequired().HasColumnName("PROFIT_CODE_ID");
        _ = builder.Property(x => x.Contribution).IsRequired().HasPrecision(9, 2).HasColumnName("CONTRIBUTION").HasComment("Contribution to plan from DMB");
        _ = builder.Property(x => x.Earnings).IsRequired().HasPrecision(9, 2).HasColumnName("EARNINGS");
        _ = builder.Property(x => x.Forfeiture).IsRequired().HasPrecision(9, 2).HasColumnName("FORFEITURE");
        _ = builder.Property(x => x.MonthToDate).IsRequired().HasPrecision(2, 0).HasColumnName("MONTH_TO_DATE");
        _ = builder.Property(x => x.YearToDate).IsRequired().HasPrecision(4, 0).HasColumnName("YEAR_TO_DATE");
        _ = builder.Property(x => x.Remark).HasMaxLength(32).HasColumnName("REMARK");
        _ = builder.Property(x => x.ZeroContributionReasonId).HasColumnName("ZERO_CONTRIBUTION_REASON_ID");
        _ = builder.Property(x => x.FederalTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("FEDERAL_TAXES");
        _ = builder.Property(x => x.StateTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("STATE_TAXES");
        _ = builder.Property(x => x.TaxCodeId).HasColumnName("TAX_CODE_ID");
        _ = builder.Property(x => x.Ssn).HasColumnName("SSN").HasPrecision(9).IsRequired();
        _ = builder.Property(x => x.DistributionSequence).HasColumnName("DISTRIBUTION_SEQUENCE");
        _ = builder.Property(x => x.CommentTypeId).HasColumnName("COMMENT_TYPE_ID");
        _ = builder.Property(x => x.CommentRelatedOracleHcmId).HasColumnName("COMMENT_RELATED_ORACLE_HCM_ID");
        _ = builder.Property(x => x.CommentRelatedPsnSuffix).HasColumnName("COMMENT_RELATED_PSN_SUFFIX");
        _ = builder.Property(x => x.CommentRelatedState).HasMaxLength(24).HasColumnName("COMMENT_RELATED_STATE");
        _ = builder.Property(x => x.CommentRelatedCheckNumber)
            .HasMaxLength(9)
            .HasColumnName("COMMENT_RELATED_CHECK_NUMBER");
        _ = builder.Property(x => x.CommentIsPartialTransaction).HasColumnName("COMMENT_IS_PARTIAL_TRANSACTION");
        _ = builder.Property(x => x.YearsOfServiceCredit).HasColumnName("YEARS_OF_SERVICE_CREDIT").HasDefaultValue(0).IsRequired();


        _ = builder.HasOne(x => x.ProfitCode).WithMany().HasForeignKey(x => x.ProfitCodeId);
        _ = builder.HasOne(x => x.CommentType).WithMany().HasForeignKey(x => x.CommentTypeId);
        _ = builder.HasOne(x => x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);
        _ = builder.HasOne(x => x.ProfitCode).WithMany().HasForeignKey(x => x.ProfitCodeId);

        _ = builder.HasOne(d => d.ZeroContributionReason)
            .WithMany()
            .HasForeignKey(d => d.ZeroContributionReasonId);
    }
}
