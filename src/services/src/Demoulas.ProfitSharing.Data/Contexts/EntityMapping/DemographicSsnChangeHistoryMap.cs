using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicSsnChangeHistoryMap : ModifiedBaseMap<DemographicSsnChangeHistory>
{
    public override void Configure(EntityTypeBuilder<DemographicSsnChangeHistory> builder)
    {
        _ = builder.ToTable("DEMOGRAPHIC_SSN_CHANGE_HISTORY");
        _ = builder.HasKey(x => x.Id);

        _ = builder.HasIndex(x => x.DemographicId, "IX_DEMOGRAPHIC");

        _ = builder.Property(x => x.Id)
            .HasPrecision(18)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(x => x.DemographicId)
            .HasPrecision(9)
            .HasColumnName("DEMOGRAPHIC_ID")
            .IsRequired();

        _ = builder.Property(x => x.OldSsn)
            .HasPrecision(9)
            .HasColumnName("OLD_SSN")
            .IsRequired();

        _ = builder.Property(x => x.NewSsn)
            .HasPrecision(9)
            .HasColumnName("NEW_SSN")
            .IsRequired();

       base.Configure(builder);
    }
}
