using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Interfaces.Audit; // Add for IDoNotAudit
using Demoulas.ProfitSharing.Data.Configuration;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Demoulas.ProfitSharing.Data.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly DataConfig _config;
    private readonly IAppUser? _appUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(DataConfig config, IAppUser? appUser, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _appUser = appUser;
        _httpContextAccessor = httpContextAccessor;
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

        string sessionId = GetSessionId(_httpContextAccessor.HttpContext);
        List<AuditEvent> events = new List<AuditEvent>();
        foreach (var entry in entries.Where(e => e.Entity is not IDoNotAudit && (e.State is EntityState.Modified or EntityState.Deleted)))
        {
            var primaryKey = GetPrimaryKeyString(entry);

            var changesJson = entry.Properties
                .Where(p => p.IsModified || entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                .Select(p => new AuditChangeEntry
                {
                    ColumnName = p.Metadata.Name,
                    OriginalValue = p.OriginalValue?.ToString(),
                    NewValue = p.CurrentValue?.ToString()
                }).ToList();

            var auditEvent = new AuditEvent
            {
                TableName = entry.Metadata.GetTableName(),
                Operation = entry.State.ToString(),
                UserName = _appUser?.UserName ?? "System",
                PrimaryKey = primaryKey,
                SessionId = sessionId,
                ChangesJson = changesJson,
                ChangesHash = CalculateChangesHash(changesJson)
            };

            events.Add(auditEvent);
        }
        context.Set<AuditEvent>().AddRange(events);
    }

    /// <summary>
    /// Extracts the session ID from HttpContext.Items, checking the current request's Items first (for same-request availability)
    /// then falling back to request cookies (for subsequent requests).
    /// </summary>
    /// <param name="httpContext">The current HTTP context</param>
    /// <returns>The session ID (20-character GUID) or "unknown" if not found</returns>
    private static string GetSessionId(HttpContext? httpContext)
    {
        if (httpContext == null)
        {
            return "unknown";
        }

        // First check HttpContext.Items (session created/retrieved in same request)
        if (httpContext.Items.TryGetValue(Telemetry.SessionIdKey, out var sessionIdObj) && sessionIdObj is string sessionId)
        {
            return sessionId;
        }

        // Fallback to request cookies (for subsequent requests with existing session)
        if (httpContext.Request.Cookies.TryGetValue(Telemetry.SessionIdKey, out var cookieSessionId))
        {
            return cookieSessionId;
        }

        return "unknown";
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

    /// <summary>
    /// Calculates a SHA256 hash of the ChangesJson list for tamper detection.
    /// </summary>
    /// <param name="changesJson">The list of audit change entries to hash</param>
    /// <returns>A hexadecimal string representation of the SHA256 hash, or null if input is null</returns>
    public static string? CalculateChangesHash(List<AuditChangeEntry>? changesJson)
    {
        if (changesJson == null)
        {
            return null;
        }

        var json = JsonSerializer.Serialize(changesJson, JsonSerializerOptions.Web);
        var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Verifies the integrity of an audit event by comparing the stored hash with a recalculated hash.
    /// </summary>
    /// <param name="auditEvent">The audit event to verify</param>
    /// <returns>True if the hash matches (record is intact), false if tampered or hash is missing</returns>
    public static bool VerifyAuditEventIntegrity(AuditEvent auditEvent)
    {
        if (auditEvent == null)
        {
            throw new ArgumentNullException(nameof(auditEvent));
        }

        if (string.IsNullOrEmpty(auditEvent.ChangesHash))
        {
            // No hash stored - cannot verify (legacy records or hash not enabled)
            return false;
        }

        var calculatedHash = CalculateChangesHash(auditEvent.ChangesJson);
        return string.Equals(auditEvent.ChangesHash, calculatedHash, StringComparison.OrdinalIgnoreCase);
    }
}
