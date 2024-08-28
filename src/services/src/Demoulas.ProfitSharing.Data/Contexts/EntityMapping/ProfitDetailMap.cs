using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ProfitDetailMap : IEntityTypeConfiguration<ProfitDetail>
{
    public void Configure(EntityTypeBuilder<ProfitDetail> builder)
    {
        _ = builder.ToTable("PROFIT_DETAIL");

        _ = builder.Property(e => e.Id).ValueGeneratedOnAdd();
        _ = builder.Property(x=>x.ProfitYear).IsRequired().HasColumnName("PROFIT_YEAR");
        _ = builder.Property(x => x.ProfitYearIteration).IsRequired().HasColumnName("PROFIT_YEAR_ITERATION");
        _ = builder.Property(x => x.ProfitCodeId).IsRequired().HasColumnName("PROFIT_CODE_ID");
        _ = builder.HasOne(x => x.ProfitCode).WithMany().HasForeignKey(x => x.ProfitCodeId);
        _ = builder.Property(x=>x.Contribution).IsRequired().HasPrecision(9,2).HasColumnName("CONTRIBUTION");
        _ = builder.Property(x => x.Earnings).IsRequired().HasPrecision(9, 2).HasColumnName("EARNINGS");
        _ = builder.Property(x => x.Forfeiture).IsRequired().HasPrecision(9, 2).HasColumnName("FORFEITURE");
        _ = builder.Property(x => x.MonthToDate).IsRequired().HasPrecision(2,0).HasColumnName("MONTH_TO_DATE");
        _ = builder.Property(x => x.YearToDate).IsRequired().HasPrecision(4,0).HasColumnName("YEAR_TO_DATE");
        _ = builder.Property(x => x.Remark).HasMaxLength(32).HasColumnName("REMARK");
        _ = builder.Property(x => x.ZeroContributionReasonId).HasColumnName("ZERO_CONTRIBUTION_REASON_ID");
        _ = builder.Property(x => x.FederalTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("FEDERAL_TAXES");
        _ = builder.Property(x => x.StateTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("STATE_TAXES");
        _ = builder.HasOne(x=> x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);
        _ = builder.Property(x => x.TaxCodeId).HasColumnName("TAX_CODE_ID");
        _ = builder.Property(x => x.Ssn).HasPrecision(9).IsRequired();
        _ = builder.Property(x => x.DistributionSequence).HasColumnName("DISTRIBUTION_SEQUENCE");

    }
}
