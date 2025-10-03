using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit; // Add for IDoNotAudit
using Demoulas.ProfitSharing.Data.Configuration;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Demoulas.ProfitSharing.Data.Interceptors;
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly DataConfig _config;
    private readonly IAppUser? _appUser;

    public AuditSaveChangesInterceptor(DataConfig config, IAppUser? appUser)
    {
        _config = config;
        _appUser = appUser;
    }

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

        if (!_config.EnableAudit)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        List<AuditEvent> events = new List<AuditEvent>();
        foreach (var entry in entries.Where(e => e.Entity is not IDoNotAudit && (e.State is EntityState.Modified or EntityState.Deleted)))
        {
            var primaryKey = GetPrimaryKeyString(entry);

            var auditEvent = new AuditEvent
            {
                TableName = entry.Metadata.GetTableName(),
                Operation = entry.State.ToString(),
                UserName = _appUser?.UserName ?? "System",
                PrimaryKey = primaryKey,
                ChangesJson = entry.Properties
                    .Where(p => p.IsModified || entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                    .Select(p => new AuditChangeEntry
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

    /// <summary>
    /// Generates a string representation of an entity's primary key values.
    /// </summary>
    /// <param name="entry">The entity entry to extract primary key values from</param>
    /// <returns>A formatted string containing key name-value pairs separated by '+'</returns>
    private static string GetPrimaryKeyString(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey == null || !primaryKey.Properties.Any())
        {
            return string.Empty;
        }

        var keyPairs = new List<string>();

        foreach (var property in primaryKey.Properties)
        {
            var columnName = property.GetColumnName();
            var propertyEntry = entry.Property(property.Name);
            var value = propertyEntry.CurrentValue;

            // Format the key-value pair, handling null values appropriately
            var formattedValue = value?.ToString() ?? "null";
            keyPairs.Add($"{columnName}={formattedValue}");
        }

        // Join key-value pairs with '+' delimiter, maintaining the existing format
        return string.Join("+", keyPairs);
    }
}
