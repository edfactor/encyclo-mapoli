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
        builder.HasOne(m => m.NavigationStatus)
            .WithMany(x => x.Navigations)
            .HasForeignKey(x => x.StatusId);

        builder.HasOne(m => m.NavigationRole)
            .WithMany(m => m.Navigations)
            .HasForeignKey(m => m.NavigationRoleId);
        
        builder.HasOne(m => m.Parent)
            .WithMany(x=>x.Children)
            .HasForeignKey(x => x.ParentId);
    }
}
