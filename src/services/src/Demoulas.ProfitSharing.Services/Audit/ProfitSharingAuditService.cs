using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Request.Audit;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Entities.Audit;
using Demoulas.Common.Data.Services.Service.Audit;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Audit;

public sealed class ProfitSharingAuditService : AuditService<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>, IProfitSharingAuditService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;
    private readonly IAppUser? _appUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _maskingOptions;

    public ProfitSharingAuditService(IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride,
        IAppUser? appUser,
        IHttpContextAccessor httpContextAccessor,
        JsonSerializerOptions jsonSerializerOptions) : base(dataContextFactory, appUser, httpContextAccessor, jsonSerializerOptions)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
        _appUser = appUser;
        _httpContextAccessor = httpContextAccessor;

        // Initialize masking serializer options from configured JSON settings
        _maskingOptions = jsonSerializerOptions;
    }

    // Move all ArchiveCompletedReportAsync overloads so they are adjacent, per S4136

    // Place all ArchiveCompletedReportAsync overloads together, after the constructor and before other public methods

    public Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto
    {

        return ArchiveCompletedReportAsync(reportName, profitYear, request, reportFunction, new List<Func<TResponse, (string, object)>>(), cancellationToken);
    }

    public Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        List<Func<TResponse, (string, object)>> additionalChecksums,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto
    {
        if (reportFunction == null)
        {
            throw new ArgumentNullException(nameof(reportFunction), "Report function cannot be null.");
        }

        bool isArchiveRequest = false;
        _ = (_httpContextAccessor.HttpContext?.Request?.Query?.TryGetValue("archive", out var archiveValue) ?? false) &&
            bool.TryParse(archiveValue, out isArchiveRequest) && isArchiveRequest;

        return ArchiveCompletedReportAsync(reportName, profitYear, request, isArchiveRequest, reportFunction, additionalChecksums, cancellationToken);
    }

    public Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        bool isArchiveRequest,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto
    {
        return ArchiveCompletedReportAsync(reportName, profitYear, request, isArchiveRequest, reportFunction, new List<Func<TResponse, (string, object)>>(), cancellationToken);
    }

    public async Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        bool isArchiveRequest,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        List<Func<TResponse, (string, object)>> additionalChecksums,
        CancellationToken cancellationToken) where TRequest : PaginationRequestDto where TResponse : class
    {
        TRequest archiveRequest = request;
        if (isArchiveRequest)
        {
            // Create archive request with full data retrieval
            archiveRequest = request with { Skip = 0, Take = int.MaxValue };
        }

        TResponse response = await reportFunction(archiveRequest, isArchiveRequest, cancellationToken);

        if (!isArchiveRequest)
        {
            return response;
        }

        string requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
        string reportJson = JsonSerializer.Serialize(response, _maskingOptions);
        string userName = _appUser?.UserName ?? "Unknown";
        string sessionId = GetSessionId(_httpContextAccessor.HttpContext);

        // Create archived data payload with type metadata
        // Parse the JSON to get a JsonElement so RawData is an object, not an escaped string
        var reportJsonElement = JsonSerializer.Deserialize<JsonElement>(reportJson);
        var archivedPayload = new ArchivedDataPayload
        {
            TypeName = typeof(TResponse).AssemblyQualifiedName ?? typeof(TResponse).FullName ?? typeof(TResponse).Name,
            RawData = reportJsonElement
        };
        string archivedPayloadJson = JsonSerializer.Serialize(archivedPayload, JsonSerializerOptions.Web);

        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = archivedPayloadJson } };
        var auditEvent = new AuditEvent
        {
            TableName = reportName,
            Operation = AuditEvent.AuditOperations.Archive,
            UserName = userName,
            SessionId = sessionId,
            ChangesJson = entries
        };

        ReportChecksum checksum = new ReportChecksum
        {
            ReportType = reportName,
            ProfitYear = profitYear,
            RequestJson = requestJson,
            ReportJson = reportJson,
            UserName = userName
        };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(response, additionalChecksums);

        using (_guardOverride.AllowFor(Role.ITDEVOPS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING))
        {
            await _dataContextFactory.UseWritableContext(async c =>
            {
                c.AuditEvents.Add(auditEvent);
                c.ReportChecksums.Add(checksum);
                await c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }

        return response;
    }

    public override async Task<TResult> LogSensitiveDataAccessAsync<TResult>(string operationName, string tableName, string? primaryKey, string? details,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        using (_guardOverride.AllowFor(Role.ITDEVOPS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING))
        {
            return await base.LogSensitiveDataAccessAsync(operationName, tableName, primaryKey, details, operation, cancellationToken);
        }
    }


    public override async Task LogDataChangeAsync(string operationName, string tableName, string auditOperation, string? primaryKey,
        IReadOnlyList<AuditChangeEntryInputRequest> changes,
        CancellationToken cancellationToken = default)
    {
        using (_guardOverride.AllowFor(Role.ITDEVOPS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING))
        {
            await base.LogDataChangeAsync(operationName, tableName, auditOperation, primaryKey, changes, cancellationToken);
        }
    }

    public static IEnumerable<KeyValuePair<string, KeyValuePair<decimal, byte[]>>> ToKeyValuePairs<TReport>(TReport obj, List<Func<TReport, (string, object)>> additionalChecksums)
        where TReport : class
    {
        var result = new List<KeyValuePair<string, decimal>>();
        var type = obj.GetType();

        // Check if the class itself has the YearEndArchivePropertyAttribute
        var classAttribute = type.GetCustomAttribute<YearEndArchivePropertyAttribute>();
        bool classHasAttribute = classAttribute != null;

        IEnumerable<PropertyInfo> properties;
        if (classHasAttribute)
        {
            // If class has the attribute, include all numeric properties (including nullable)
            properties = type.GetProperties()
                .Where(p => IsNumericType(p.PropertyType));
        }
        else
        {
            // Otherwise, only include properties that explicitly have the attribute
            properties = type.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(YearEndArchivePropertyAttribute))
                            && IsNumericType(p.PropertyType));
        }

        foreach (var prop in properties)
        {
            // Get the attribute to retrieve the KeyName
            var propertyAttribute = prop.GetCustomAttribute<YearEndArchivePropertyAttribute>();
            string keyName;

            if (propertyAttribute != null)
            {
                // Use the KeyName from the property attribute
                keyName = propertyAttribute.KeyName ?? prop.Name;
            }
            else if (classHasAttribute)
            {
                // Property doesn't have its own attribute, but class does
                // Use the property name as the key
                keyName = prop.Name;
            }
            else
            {
                // Shouldn't happen, but fallback to property name
                keyName = prop.Name;
            }

            // Convert all numeric types to decimal for consistent hashing
            object? rawValue = prop.GetValue(obj);
            decimal value = ConvertToDecimal(rawValue);
            result.Add(new KeyValuePair<string, decimal>(keyName, value));
        }

        foreach (var additionalChecksum in additionalChecksums)
        {
            (string key, object valueObj) = additionalChecksum(obj);
            decimal value = ConvertToDecimal(valueObj);
            result.Add(new KeyValuePair<string, decimal>(key, value));
        }

        // Materialize the result list to avoid lazy evaluation issues with yield return
        var materializedResult = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>();

        foreach (var kevValue in result)
        {
            byte[] hash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(kevValue.Value));
            var kvp = new KeyValuePair<string, KeyValuePair<decimal, byte[]>>(kevValue.Key, new KeyValuePair<decimal, byte[]>(kevValue.Value, hash));
            materializedResult.Add(kvp);
        }

        return materializedResult;
    }

    /// <summary>
    /// Determines if a type is a numeric type (including nullable variants).
    /// Supports all C# primitive numeric types: byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal.
    /// </summary>
    private static bool IsNumericType(Type type)
    {
        // Get the underlying type if nullable
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType == typeof(byte) ||
               underlyingType == typeof(sbyte) ||
               underlyingType == typeof(short) ||
               underlyingType == typeof(ushort) ||
               underlyingType == typeof(int) ||
               underlyingType == typeof(uint) ||
               underlyingType == typeof(long) ||
               underlyingType == typeof(ulong) ||
               underlyingType == typeof(float) ||
               underlyingType == typeof(double) ||
               underlyingType == typeof(decimal);
    }

    /// <summary>
    /// Converts any numeric type (or null) to decimal for consistent hashing.
    /// Returns 0m for null values.
    /// </summary>
    private static decimal ConvertToDecimal(object? value)
    {
        return value switch
        {
            byte b => b,
            sbyte sb => sb,
            short s => s,
            ushort us => us,
            int i => i,
            uint ui => ui,
            long l => l,
            ulong ul => ul,
            float f => (decimal)f,
            double d => (decimal)d,
            decimal dec => dec,
            null => 0m,
            _ => 0m
        };
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
        if (httpContext.Items.TryGetValue(Demoulas.ProfitSharing.Common.Constants.Telemetry.SessionIdKey, out object? sessionIdObj) && sessionIdObj is string sessionId)
        {
            return sessionId;
        }

        // Fallback to request cookies (for subsequent requests with existing session)
        if (httpContext.Request.Cookies.TryGetValue(Demoulas.ProfitSharing.Common.Constants.Telemetry.SessionIdKey, out string? cookieSessionId))
        {
            return cookieSessionId;
        }

        return "unknown";
    }
}
