using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ProfitDetailMap : IEntityTypeConfiguration<ProfitDetail>
{
    public void Configure(EntityTypeBuilder<ProfitDetail> builder)
    {
        _ = builder.ToTable("ProfitDetail");
        _ = builder.HasKey(x => new { x.ProfitYear, x.ProfitYearIteration });

        _ = builder.Property(x=>x.ProfitYear).IsRequired().HasColumnName("PROFIT_YEAR");
        _ = builder.Property(x => x.ProfitYearIteration).IsRequired().HasColumnName("PROFIT_YEAR_ITERATION");
        _ = builder.Property(x => x.ProfitCodeId).IsRequired().HasColumnName("PROFIT_CODE_ID");
        _ = builder.Property(x => x.ProfitClient).HasColumnName("PROFIT_CLIENT");
        _ = builder.HasOne(x => x.ProfitCode).WithMany().HasForeignKey(x => x.ProfitCodeId);
        _ = builder.Property(x=>x.Contribution).IsRequired().HasPrecision(9,2).HasColumnName("CONTRIBUTION");
        _ = builder.Property(x => x.Earnings).IsRequired().HasPrecision(9, 2).HasColumnName("EARNINGS");
        _ = builder.Property(x => x.Forfeiture).IsRequired().HasPrecision(9, 2).HasColumnName("FORFEITURE");
        _ = builder.Property(x => x.Month).IsRequired().HasColumnName("MONTH");
        _ = builder.Property(x => x.Year).IsRequired().HasColumnName("YEAR");
        _ = builder.Property(x => x.Comment).HasMaxLength(16).HasColumnName("COMMENT");
        _ = builder.Property(x => x.FederalTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("FEDERAL_TAXES");
        _ = builder.Property(x => x.StateTaxes).IsRequired().HasPrecision(9, 2).HasColumnName("STATE_TAXES");
        _ = builder.HasOne(x=> x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);
        _ = builder.Property(x => x.TaxCodeId).IsRequired().HasColumnName("TAX_CODE_ID");
        _ = builder.Property(x => x.SSN).IsRequired();
        _ = builder.Property(x => x.ProfDistId).HasColumnName("PROF_DIST_ID");
        
    }
}
