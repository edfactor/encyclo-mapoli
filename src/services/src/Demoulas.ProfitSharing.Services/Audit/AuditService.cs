using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Audit;

public sealed class AuditService : IAuditService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;
    private readonly IAppUser? _appUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _maskingOptions;

    public AuditService(IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride,
        IAppUser? appUser,
        IHttpContextAccessor httpContextAccessor)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
        _appUser = appUser;
        _httpContextAccessor = httpContextAccessor;

        // Initialize masking serializer options
        _maskingOptions = new JsonSerializerOptions(JsonSerializerOptions.Web);
        _maskingOptions.Converters.Add(new MaskingJsonConverterFactory());
    }

    public Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
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

        return ArchiveCompletedReportAsync(reportName, profitYear, request, isArchiveRequest, reportFunction, cancellationToken);
    }

    public async Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        bool isArchiveRequest,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken) where TRequest : PaginationRequestDto where TResponse : class
    {
        TRequest archiveRequest = request;
        if (isArchiveRequest)
        {
            // Create archive request with full data retrieval
            archiveRequest = request with { Skip = 0, Take = ushort.MaxValue };
        }

        TResponse response = await reportFunction(archiveRequest, isArchiveRequest, cancellationToken);

        if (!isArchiveRequest)
        {
            return response;
        }

        string requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
        string reportJson = JsonSerializer.Serialize(response, _maskingOptions);
        string userName = _appUser?.UserName ?? "Unknown";

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
        var auditEvent = new AuditEvent { TableName = reportName, Operation = AuditEvent.AuditOperations.Archive, UserName = userName, ChangesJson = entries };

        ReportChecksum checksum = new ReportChecksum
        {
            ReportType = reportName,
            ProfitYear = profitYear,
            RequestJson = requestJson,
            ReportJson = reportJson,
            UserName = userName
        };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(response);

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

    public async Task LogSensitiveDataAccessAsync(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        CancellationToken cancellationToken = default)
    {
        string userName = _appUser?.UserName ?? "Unknown";

        var entries = new List<AuditChangeEntry>
        {
            new() { ColumnName = "Operation", NewValue = operationName },
            new() { ColumnName = "Details", NewValue = details ?? "N/A" }
        };

        var auditEvent = new AuditEvent
        {
            TableName = tableName,
            Operation = AuditEvent.AuditOperations.SensitiveAccess,
            PrimaryKey = primaryKey,
            UserName = userName,
            ChangesJson = entries,
            CreatedAt = DateTimeOffset.UtcNow
        };

        using (_guardOverride.AllowFor(Role.ITDEVOPS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING))
        {
            await _dataContextFactory.UseWritableContext(async c =>
            {
                c.AuditEvents.Add(auditEvent);
                await c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }
    }

    public async Task<TResult> LogSensitiveDataAccessAsync<TResult>(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var result = await operation(cancellationToken);

        // Log access after successful operation
        await LogSensitiveDataAccessAsync(
            operationName,
            tableName,
            primaryKey,
            details,
            cancellationToken);

        return result;
    }

    public Task<PaginatedResponseDto<AuditEventDto>> SearchAuditEventsAsync(
        AuditSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = context.AuditEvents
                .TagWith($"AuditSearch-GetEvents-Filters-Table:{request.TableName}-Operation:{request.Operation}-User:{request.UserName}")
                .AsQueryable();

            // Apply LIKE filters for string fields (using Contains for testability)
            if (!string.IsNullOrWhiteSpace(request.TableName))
            {
                query = query.Where(e => e.TableName != null && e.TableName.Contains(request.TableName));
            }

            if (!string.IsNullOrWhiteSpace(request.Operation))
            {
                query = query.Where(e => e.Operation.Contains(request.Operation));
            }

            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                query = query.Where(e => e.UserName.Contains(request.UserName));
            }

            // Apply date range filters
            if (request.StartTime.HasValue)
            {
                query = query.Where(e => e.CreatedAt >= request.StartTime.Value);
            }

            if (request.EndTime.HasValue)
            {
                query = query.Where(e => e.CreatedAt <= request.EndTime.Value);
            }

            var sortReq = request with
            {
                SortBy = request.SortBy switch
                {
                    "auditEventId" => "Id",
                    "" => "Id",
                    null => "Id",
                    _ => request.SortBy
                }
            };
            var paginatedResults = await query.ToPaginationResultsAsync(sortReq, cancellationToken);

            // Project to DTOs with ChangesJson handling after materialization
            var dtos = paginatedResults.Results.Select(e => new AuditEventDto
            {
                AuditEventId = e.Id,
                TableName = e.TableName,
                Operation = e.Operation,
                PrimaryKey = e.PrimaryKey,
                UserName = e.UserName,
                CreatedAt = e.CreatedAt,
                // Only include ChangesJson when TableName is "NAVIGATION"
                ChangesJson = e.TableName == "NAVIGATION"
                    ? e.ChangesJson?.Select(c => new AuditChangeEntryDto
                    {
                        Id = c.Id,
                        ColumnName = c.ColumnName,
                        OriginalValue = SerializeJsonValue(c.OriginalValue),
                        NewValue = SerializeJsonValue(c.NewValue)
                    }).ToList()
                    : null
            }).ToList();

            return new PaginatedResponseDto<AuditEventDto>
            {
                Results = dtos,
                Total = paginatedResults.Total
            };
        }, cancellationToken);
    }

    public Task<List<AuditChangeEntryDto>> GetAuditChangeEntriesAsync(
        int auditEventId,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var auditEvent = await context.AuditEvents
                .TagWith($"AuditSearch-GetChangeEntries-EventId:{auditEventId}")
                .FirstOrDefaultAsync(e => e.Id == auditEventId, cancellationToken);

            if (auditEvent == null || auditEvent.ChangesJson == null)
            {
                return new List<AuditChangeEntryDto>();
            }

            return auditEvent.ChangesJson.Select(c => new AuditChangeEntryDto
            {
                Id = c.Id,
                ColumnName = c.ColumnName,
                OriginalValue = DeserializeArchivedPayload(c.OriginalValue),
                NewValue = DeserializeArchivedPayload(c.NewValue)
            }).ToList();
        }, cancellationToken);
    }

    private string? DeserializeArchivedPayload(string? jsonValue)
    {
        if (string.IsNullOrWhiteSpace(jsonValue))
        {
            return null;
        }

        try
        {
            // Try to deserialize as ArchivedDataPayload
            var payload = JsonSerializer.Deserialize<ArchivedDataPayload>(jsonValue, JsonSerializerOptions.Web);

            if (payload == null)
            {
                return jsonValue;
            }

            // Get the Type from TypeName
            var type = Type.GetType(payload.TypeName);

            if (type == null)
            {
                // If type can't be resolved, return the RawData as formatted JSON with masking
                return JsonSerializer.Serialize(payload.RawData, _maskingOptions);
            }

            // Deserialize RawData as the actual type
            var deserializedObject = JsonSerializer.Deserialize(payload.RawData.GetRawText(), type, JsonSerializerOptions.Web);

            // Serialize back with masking options for proper role-based masking
            return JsonSerializer.Serialize(deserializedObject, _maskingOptions);
        }
        catch (JsonException)
        {
            // If it's not a valid ArchivedDataPayload, try to format as generic JSON
            return SerializeJsonValue(jsonValue);
        }
    }

    private static string? SerializeJsonValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(value);
            // Serialize with indentation for better readability
            return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            // If it's not valid JSON, return as-is (it's a plain string value)
            return value;
        }
    }

    public static IEnumerable<KeyValuePair<string, KeyValuePair<decimal, byte[]>>> ToKeyValuePairs<TReport>(TReport obj)
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
            var rawValue = prop.GetValue(obj);
            var value = ConvertToDecimal(rawValue);
            result.Add(new KeyValuePair<string, decimal>(keyName, value));
        }

        // Materialize the result list to avoid lazy evaluation issues with yield return
        var materializedResult = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>();

        foreach (var kevValue in result)
        {
            var hash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(kevValue.Value));
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
}
