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
        _ = builder.Property(x => x.IsReadOnly).HasColumnName("IS_READ_ONLY").IsRequired().HasDefaultValue(false);


        builder.HasData(
            new NavigationRole { Id = NavigationRole.Contants.Administrator, Name = Role.ADMINISTRATOR, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.BeneficiaryAdministrator, Name = Role.BENEFICIARY_ADMINISTRATOR, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.FinanceManager, Name = Role.FINANCEMANAGER, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.DistributionClerk, Name = Role.DISTRIBUTIONSCLERK, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.HardshipAdministrator, Name = Role.HARDSHIPADMINISTRATOR, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.Impersonation, Name = Role.IMPERSONATION, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.ItDevOps, Name = Role.ITDEVOPS, IsReadOnly = true },
            new NavigationRole { Id = NavigationRole.Contants.ItOperations, Name = Role.ITOPERATIONS, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.ExecutiveAdministrator, Name = Role.EXECUTIVEADMIN, IsReadOnly = false },
            new NavigationRole { Id = NavigationRole.Contants.Auditor, Name = Role.AUDITOR, IsReadOnly = true }
        );
    }
}
