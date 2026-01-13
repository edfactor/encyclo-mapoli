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

        _ = builder.Property(e => e.OracleHcmId)
            .HasPrecision(15)
            .ValueGeneratedNever()
            .HasColumnName("ORACLE_HCM_ID");

        _ = builder.Property(e => e.InvalidValue)
            .HasColumnName("INVALID_VALUE")
            .HasMaxLength(256);

        _ = builder.Property(e => e.PropertyName)
            .HasColumnName("PROPERTY_NAME")
            .HasMaxLength(128);

        _ = builder.Property(e => e.Message)
            .HasColumnName("MESSAGE")
            .HasMaxLength(512);

        _ = builder.Property(e => e.UserName)
            .HasColumnName("USERNAME")
            .HasMaxLength(96);

        _ = builder.Property(e => e.Created)
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasColumnName("CREATED")
            .ValueGeneratedOnAdd();
    }
}
