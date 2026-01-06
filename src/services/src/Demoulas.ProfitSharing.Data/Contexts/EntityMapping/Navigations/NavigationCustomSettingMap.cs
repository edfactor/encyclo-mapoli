using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;

internal sealed class NavigationCustomSettingMap : IEntityTypeConfiguration<NavigationCustomSetting>
{
    public void Configure(EntityTypeBuilder<NavigationCustomSetting> builder)
    {
        _ = builder.ToTable("NAVIGATION_CUSTOM_SETTING");
        _ = builder.HasKey(x => new { x.NavigationId, x.Key });

        _ = builder.Property(x => x.NavigationId)
            .HasColumnName("NAVIGATION_ID")
            .IsRequired();

        _ = builder.Property(x => x.Key)
            .HasColumnName("KEY")
            .HasMaxLength(128)
            .IsRequired();

        _ = builder.Property(x => x.ValueJson)
            .HasColumnName("VALUE_JSON")
            .HasColumnType("CLOB")
            .IsRequired();

        _ = builder.HasOne(x => x.Navigation)
            .WithMany(n => n.CustomSettings)
            .HasForeignKey(x => x.NavigationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data moved to SQL scripts - per-navigation settings inserted via add-navigation-data.sql
    }
}
