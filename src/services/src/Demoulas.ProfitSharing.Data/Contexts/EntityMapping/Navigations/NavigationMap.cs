using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
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
        _ = builder.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(x => x.Url).HasColumnName("URL").HasMaxLength(200);
        _ = builder.Property(x => x.Title).HasColumnName("TITLE").HasMaxLength(100);
        _ = builder.Property(x => x.SubTitle).HasColumnName("SUB_TITLE").HasMaxLength(70);
        _ = builder.Property(x => x.Icon).HasColumnName("ICON").HasMaxLength(200);
        _ = builder.Property(x => x.OrderNumber).HasColumnName("ORDER_NUMBER");
        _ = builder.Property(x => x.StatusId).HasColumnName("STATUS_ID");
        _ = builder.Property(x => x.ParentId).HasColumnName("PARENT_ID");
        _ = builder.Property(x => x.NavigationRoleJSON).HasColumnName("NAVIGATION_ROLE_JSON").HasMaxLength(300);




        builder.HasOne(m => m.NavigationStatus)
            .WithMany(x => x.Navigations)
            .HasForeignKey(x => x.StatusId);

        builder.HasOne(m => m.Parent)
            .WithMany(x=>x.Children)
            .HasForeignKey(x => x.ParentId);
    }
}
