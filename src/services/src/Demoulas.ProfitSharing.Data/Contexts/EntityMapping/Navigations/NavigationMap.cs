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

        builder.HasOne(m => m.NavigationStatus)
            .WithMany(x => x.Navigations)
            .HasForeignKey(x => x.StatusId);

        builder.HasOne(m => m.Parent)
            .WithMany(x=>x.Items)
            .HasForeignKey(x => x.ParentId);

        builder.HasMany(x => x.RequiredRoles)
            .WithMany()
            .UsingEntity(x=>x.ToTable(name:"NAVIGATION_ASSIGNED_ROLES"));

        // Seed data based on add-navigation-data.sql
        builder.HasData(
            // Main menu items
            new Navigation 
            { 
                Id = Navigation.Constants.Inquiries, 
                ParentId = null, 
                Title = "INQUIRIES", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 1, 
                Icon = "", 
                Disabled = false 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.Beneficiaries, 
                ParentId = null, 
                Title = "BENEFICIARIES", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 2, 
                Icon = "", 
                Disabled = true 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.Distributions, 
                ParentId = null, 
                Title = "DISTRIBUTIONS", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 3, 
                Icon = "", 
                Disabled = true 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.Reconciliation, 
                ParentId = null, 
                Title = "RECONCILIATION", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 4, 
                Icon = "", 
                Disabled = true 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.YearEnd, 
                ParentId = null, 
                Title = "YEAR END", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 5, 
                Icon = "", 
                Disabled = false 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.ItOperations, 
                ParentId = null, 
                Title = "IT OPERATIONS", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 6, 
                Icon = "", 
                Disabled = false 
            },
            
            // Sub-menu items
            new Navigation 
            { 
                Id = Navigation.Constants.MasterInquiry, 
                ParentId = Navigation.Constants.Inquiries, 
                Title = "MASTER INQUIRY", 
                SubTitle = "", 
                Url = "master-inquiry", 
                StatusId = 1, 
                OrderNumber = 1, 
                Icon = "", 
                Disabled = false 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.DemographicFreeze, 
                ParentId = Navigation.Constants.ItOperations, 
                Title = "Demographic Freeze", 
                SubTitle = "", 
                Url = "demographic-freeze", 
                StatusId = 1, 
                OrderNumber = 1, 
                Icon = "", 
                Disabled = false 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.DecemberActivities, 
                ParentId = Navigation.Constants.YearEnd, 
                Title = "December Activities", 
                SubTitle = "", 
                Url = "december-process-accordion", 
                StatusId = 1, 
                OrderNumber = 1, 
                Icon = "", 
                Disabled = false 
            },
            new Navigation 
            { 
                Id = Navigation.Constants.CleanupReports, 
                ParentId = Navigation.Constants.DecemberActivities, 
                Title = "Clean up Reports", 
                SubTitle = "", 
                Url = "", 
                StatusId = 1, 
                OrderNumber = 1, 
                Icon = "", 
                Disabled = false 
            }
        );
    }
}
