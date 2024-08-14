using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicSyncAuditMap : IEntityTypeConfiguration<DemographicSyncAudit>
{
    public void Configure(EntityTypeBuilder<DemographicSyncAudit> builder)
    {
        _ = builder.ToTable("DEMOGRAPHIC_SYNC_AUDIT");
        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder.HasIndex(e => e.BadgeNumber, "IX_BADGENUMBER");
        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

        _ = builder.Property(e => e.Message)
            .HasColumnName("MESSAGE");

        _ = builder.Property(e => e.UserName)
            .HasColumnName("USERNAME")
            .HasMaxLength(96);

        _ = builder.Property(e => e.Created)
            .HasColumnType("DATE")
            .HasColumnName("CREATED")
            .ValueGeneratedOnAdd();
    }
}
