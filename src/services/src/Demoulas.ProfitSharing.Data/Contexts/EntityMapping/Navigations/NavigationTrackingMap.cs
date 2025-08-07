using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationTrackingMap : IEntityTypeConfiguration<NavigationTracking>
{
    public void Configure(EntityTypeBuilder<NavigationTracking> builder)
    {
        _ = builder.ToTable("NAVIGATION_TRACKING");
        _ = builder.HasKey(t => t.Id);
        _ = builder.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(x => x.StatusId).HasColumnName("STATUS_ID");
        _ = builder.Property(x => x.NavigationId).HasColumnName("NAVIGATION_ID");
        _ = builder.Property(x => x.Username).HasColumnName("USERNAME").HasMaxLength(60);
        _ = builder.Property(x => x.LastModified).HasColumnName("LAST_MODIFIED")
            .HasColumnType("TIMESTAMP WITH TIME ZONE");

        builder.HasOne(m => m.Navigation)
            .WithMany(m => m.NavigationTrackings)
            .HasForeignKey(m => m.NavigationId);

        builder.HasOne(m => m.NavigationStatus)
            .WithMany(m => m.NavigationTrackings)
            .HasForeignKey(m => m.StatusId);
    }
}
