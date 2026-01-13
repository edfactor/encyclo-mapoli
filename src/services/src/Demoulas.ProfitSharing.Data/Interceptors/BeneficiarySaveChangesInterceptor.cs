using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Demoulas.ProfitSharing.Data.Interceptors;

public class BeneficiarySaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
    InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AuditDeletes(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AuditDeletes(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var archiveEntries = context.ChangeTracker.Entries<Beneficiary>()
                .Where(e => e.State is EntityState.Deleted)
                .Select(x => new BeneficiaryArchive()
                {
                    Id = x.Entity.Id,
                    PsnSuffix = x.Entity.PsnSuffix,
                    BadgeNumber = x.Entity.BadgeNumber,
                    DemographicId = x.Entity.DemographicId,
                    BeneficiaryContactId = x.Entity.BeneficiaryContactId,
                    Percent = x.Entity.Percent,
                    Relationship = x.Entity.Relationship,
                    DeletedBy = context.GetService<IAppUser>()?.UserName ?? "",
                    DeleteDate = DateTime.UtcNow.ToDateOnly()
                });
        context.Set<BeneficiaryArchive>().AddRange(archiveEntries);


    }
}
