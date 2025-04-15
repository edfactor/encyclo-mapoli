using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationRoleMap : IEntityTypeConfiguration<NavigationRole>
{
    public void Configure(EntityTypeBuilder<NavigationRole> builder)
    {
        _ = builder.ToTable("NAVIGATION_ROLE");
        _ = builder.HasKey(m => m.Id);
        _ = builder.Property(m => m.Id).ValueGeneratedNever().HasColumnName("ID").IsRequired();
        _ = builder.Property(x => x.Name).HasColumnName("URL").HasMaxLength(65).IsRequired();


        builder.HasData(
            new NavigationRole() { Id = NavigationRole.Contants.Administrator, Name = "Profit-Sharing-Administrator" },
            new NavigationRole() { Id = NavigationRole.Contants.FinanceManager, Name = "Finance-Manager" },
            new NavigationRole() { Id = NavigationRole.Contants.DistributionClerk, Name = "Distributions-Clerk" },
            new NavigationRole() { Id = NavigationRole.Contants.HardshipAdministrator, Name = "Hardship-Administrator" },
            new NavigationRole() { Id = NavigationRole.Contants.Impersonation, Name = "Impersonation" },
            new NavigationRole() { Id = NavigationRole.Contants.ITOperations, Name = "IT-Operations" }
            );
    }
}
