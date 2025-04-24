using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Interceptors;
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Audit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Audit(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        List<AuditEvent> events = new List<AuditEvent>();
        foreach (var entry in entries)
        {
            var auditEvent = new AuditEvent
            {
                TableName = entry.Metadata.GetTableName(),
                Operation = entry.State.ToString(),
                PrimaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                Changes = entry.Properties
                    .Where(p => p.IsModified || entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                    .Select(p => new AuditChange
                    {
                        ColumnName = p.Metadata.Name,
                        OriginalValue = p.OriginalValue?.ToString(),
                        NewValue = p.CurrentValue?.ToString()
                    }).ToList()
            };

            events.Add(auditEvent);
        }
        context.Set<AuditEvent>().AddRange(events);
    }
}
