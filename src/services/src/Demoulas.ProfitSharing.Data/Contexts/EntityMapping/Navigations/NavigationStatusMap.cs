using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationStatusMap : IEntityTypeConfiguration<NavigationStatus>
{
    public void Configure(EntityTypeBuilder<NavigationStatus> builder)
    {
        _ = builder.ToTable("NAVIGATION_STATUS");
        _ = builder.HasKey(m => m.Id);

        _ = builder.Property(m => m.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        _ = builder.Property(m => m.Name)
            .HasMaxLength(64)
            .HasColumnName("NAME")
            .IsRequired();



        builder.HasData(
            new NavigationStatus {Id =NavigationStatus.Constants.NotStarted, Name = "Not Started"},
            new NavigationStatus {Id =NavigationStatus.Constants.InProgress, Name = "In Progress"},
            new NavigationStatus {Id =NavigationStatus.Constants.OnHold, Name = "On Hold"},
            new NavigationStatus {Id =NavigationStatus.Constants.Complete, Name = "Complete"}
        );
    }
}
