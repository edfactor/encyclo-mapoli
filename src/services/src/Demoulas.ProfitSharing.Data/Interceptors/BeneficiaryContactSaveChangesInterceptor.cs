using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Demoulas.ProfitSharing.Data.Interceptors;

public sealed class BeneficiaryContactSaveChangesInterceptor : SaveChangesInterceptor
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
        var archiveEntries = context.ChangeTracker.Entries<BeneficiaryContact>()
            .Where(e => e.State is EntityState.Deleted)
            .Select(x => new BeneficiaryContactArchive()
            {
                Id = x.Entity.Id,
                Ssn = x.Entity.Ssn,
                DateOfBirth = x.Entity.DateOfBirth,
                Address = x.Entity.Address,
                ContactInfo = x.Entity.ContactInfo,
                CreatedDate = x.Entity.CreatedDate,
                DeletedBy = context.GetService<IAppUser>()?.UserName ?? "",
                DeleteDate = DateTime.UtcNow.ToDateOnly()
            });
        context.Set<BeneficiaryContactArchive>().AddRange(archiveEntries);
    }
}
