using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        _ = builder.Property(x=>x.ProfitYear).IsRequired();
        _ = builder.Property(x => x.ProfitYearIteration).IsRequired();
        _ = builder.Property(x => x.ProfitCodeId).IsRequired();
        _ = builder.HasOne(x => x.ProfitCode).WithMany().HasForeignKey(x => x.ProfitCodeId);
        _ = builder.Property(x=>x.Contribution).IsRequired().HasPrecision(9,2);
        _ = builder.Property(x => x.Earnings).IsRequired().HasPrecision(9, 2);
        _ = builder.Property(x => x.Forfeiture).IsRequired().HasPrecision(9, 2);
        _ = builder.Property(x => x.Month).IsRequired();
        _ = builder.Property(x => x.Year).IsRequired();
        _ = builder.Property(x => x.Comment).HasMaxLength(16);
        _ = builder.Property(x => x.FederalTaxes).IsRequired().HasPrecision(9, 2);
        _ = builder.Property(x => x.StateTaxes).IsRequired().HasPrecision(9, 2);
        _ = builder.HasOne(x=> x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);
        _ = builder.Property(x => x.TaxCodeId).IsRequired();
        _ = builder.Property(x => x.SSN).IsRequired();
    }
}
