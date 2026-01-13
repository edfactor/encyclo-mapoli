using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;

internal sealed class NavigationMap : IEntityTypeConfiguration<Navigation>
{
    public void Configure(EntityTypeBuilder<Navigation> builder)
    {
        _ = builder.ToTable("NAVIGATION");
        _ = builder.HasKey(t => t.Id);
        _ = builder.Property(x => x.Id).HasColumnName("ID").ValueGeneratedNever();
        _ = builder.Property(x => x.Url).HasColumnName("URL").HasMaxLength(200);
        _ = builder.Property(x => x.Title).HasColumnName("TITLE").HasMaxLength(100);
        _ = builder.Property(x => x.SubTitle).HasColumnName("SUB_TITLE").HasMaxLength(70);
        _ = builder.Property(x => x.Icon).HasColumnName("ICON").HasMaxLength(200);
        _ = builder.Property(x => x.OrderNumber).HasColumnName("ORDER_NUMBER");
        _ = builder.Property(x => x.StatusId).HasColumnName("STATUS_ID");
        _ = builder.Property(x => x.ParentId).HasColumnName("PARENT_ID");
        _ = builder.Property(x => x.Disabled).HasColumnName("DISABLED");
        _ = builder.Property(x => x.IsNavigable).HasColumnName("IS_NAVIGABLE");

        builder.HasOne(m => m.NavigationStatus)
            .WithMany(x => x.Navigations)
            .HasForeignKey(x => x.StatusId);

        builder.HasOne(m => m.Parent)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ParentId);

        builder.HasMany(x => x.RequiredRoles)
            .WithMany()
            .UsingEntity(x => x.ToTable(name: "NAVIGATION_ASSIGNED_ROLES"));

        // Self-referencing many-to-many for prerequisites
        builder
            .HasMany(n => n.PrerequisiteNavigations!)
            .WithMany(n => n.DependentNavigations!)
            .UsingEntity<Dictionary<short, short>>(
                "NAVIGATION_PREREQUISITES",
                j => j
                    .HasOne<Navigation>()
                    .WithMany()
                    .HasForeignKey("PREREQUISITE_ID")
                    .HasConstraintName("FK_NAV_PREREQ_PREREQUISITE")
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne<Navigation>()
                    .WithMany()
                    .HasForeignKey("NAVIGATION_ID")
                    .HasConstraintName("FK_NAV_PREREQ_DEPENDENT")
                    .OnDelete(DeleteBehavior.NoAction)
            )
            .ToTable("NAVIGATION_PREREQUISITES");
    }
}
