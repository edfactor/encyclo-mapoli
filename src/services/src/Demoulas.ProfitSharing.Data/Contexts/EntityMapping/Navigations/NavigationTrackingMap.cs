using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationTrackingMap : IEntityTypeConfiguration<NavigationTracking>
{
    public void Configure(EntityTypeBuilder<NavigationTracking> builder)
    {
        builder.HasOne(m => m.Navigation)
            .WithMany(m => m.NavigationTrackings)
            .HasForeignKey(m => m.NavigationId);

        builder.HasOne(m => m.NavigationStatus)
            .WithMany(m => m.NavigationTrackings)
            .HasForeignKey(m => m.StatusId);
    }
}
