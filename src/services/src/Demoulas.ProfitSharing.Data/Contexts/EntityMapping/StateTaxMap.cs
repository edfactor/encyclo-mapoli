using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class StateTaxMap : ModifiedBaseMap<StateTax>
{
    public override void Configure(EntityTypeBuilder<StateTax> builder)
    {
        base.Configure(builder);

        builder.ToTable("STATE_TAX");
        builder.HasKey(x => x.Abbreviation);
        builder.Property(x => x.Abbreviation).HasMaxLength(2);
        builder.Property(x => x.Rate).HasPrecision(9, 2);

    }
}
