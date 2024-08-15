using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class TaxCodeMap : IEntityTypeConfiguration<TaxCode>
{
    public void Configure(EntityTypeBuilder<TaxCode> builder)
    {
        builder.HasKey(x=> x.Code);
        builder.ToTable("TAX_CODE");

        builder.Property(x => x.Code).IsRequired().ValueGeneratedNever().HasColumnName("CODE");
        builder.Property(x => x.Description).IsRequired().HasMaxLength(128).HasColumnName("DESCRIPTION");
        builder.HasData(GetPredefinedTaxCodes());
    }

    private List<TaxCode> GetPredefinedTaxCodes()
    {
        return
        [
            new TaxCode{Code='1', Description="Early (Premature) dist no known exception" },
            new TaxCode{Code='2', Description="Early (Premature) dist exception applies" },
            new TaxCode{Code='3', Description="Disability" },
            new TaxCode{Code='4', Description="Death"},
            new TaxCode{Code='5', Description="Prohibited transaction"},
            new TaxCode{Code='6', Description="Section 1035 exchange"},
            new TaxCode{Code='7', Description="Normal distribution"},
            new TaxCode{Code='8', Description="Excess contributions + earnings/deferrals"},
            new TaxCode{Code='9', Description="PS 58 cost"},
            new TaxCode{Code='A', Description="Qualifies for 5- or 10-year averaging"},
            new TaxCode{Code='B', Description="Qualifies for death benefit exclusion"},
            new TaxCode{Code='C', Description="Qualifies for both A and B"},
            new TaxCode{Code='D', Description="Excess contributions + earnings deferrals"},
            new TaxCode{Code='E', Description="Excess annual additions under section 415"},
            new TaxCode{Code='F', Description="Charitable gift annuity"},
            new TaxCode{Code='G', Description="Direct rollover to IRA"},
            new TaxCode{Code='H', Description="Direct rollover to plan/tax sheltered annuity"},
            new TaxCode{Code='P', Description="Excess contributions + earnings/deferrals "},

        ];
    }
}
