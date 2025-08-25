using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
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
        _ = builder.Property(x => x.Name).HasColumnName("NAME").HasMaxLength(65).IsRequired();


        builder.HasData(
            new NavigationRole { Id = NavigationRole.Contants.Administrator, Name = Role.ADMINISTRATOR },
            new NavigationRole { Id = NavigationRole.Contants.FinanceManager, Name = Role.FINANCEMANAGER },
            new NavigationRole { Id = NavigationRole.Contants.DistributionClerk, Name = Role.DISTRIBUTIONSCLERK },
            new NavigationRole { Id = NavigationRole.Contants.HardshipAdministrator, Name = Role.HARDSHIPADMINISTRATOR },
            new NavigationRole { Id = NavigationRole.Contants.Impersonation, Name = Role.IMPERSONATION },
            new NavigationRole { Id = NavigationRole.Contants.ItDevOps, Name = Role.ITDEVOPS }
            );
    }
}
