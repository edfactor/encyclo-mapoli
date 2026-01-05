using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class AnnuityRateConfigMap : ModifiedBaseMap<AnnuityRateConfig>
{
    public override void Configure(EntityTypeBuilder<AnnuityRateConfig> builder)
    {
        base.Configure(builder);
        _ = builder.ToTable("ANNUITY_RATE_CONFIG");
        _ = builder.HasKey(c => c.Year);

        _ = builder.Property(c => c.Year).IsRequired().ValueGeneratedNever().HasColumnName("YEAR").HasPrecision(4);
        _ = builder.Property(c => c.MinimumAge).IsRequired().HasColumnName("MINIMUM_AGE").HasPrecision(3);
        _ = builder.Property(c => c.MaximumAge).IsRequired().HasColumnName("MAXIMUM_AGE").HasPrecision(3);
    }
}
