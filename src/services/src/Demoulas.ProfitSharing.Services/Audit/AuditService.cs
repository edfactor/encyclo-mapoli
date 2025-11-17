using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Audit;

public sealed class AuditService : IAuditService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAppUser? _appUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IProfitSharingDataContextFactory dataContextFactory,
        IAppUser? appUser,
        IHttpContextAccessor httpContextAccessor)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(string reportName,
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
        string reportJson = JsonSerializer.Serialize(response, JsonSerializerOptions.Web);
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
        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = userName, ChangesJson = entries };

        ReportChecksum checksum = new ReportChecksum
        {
            ReportType = reportName,
            ProfitYear = profitYear,
            RequestJson = requestJson,
            ReportJson = reportJson,
            UserName = userName
        };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(response);

        await _dataContextFactory.UseWritableContext(async c =>
        {
            c.AuditEvents.Add(auditEvent);
            c.ReportChecksums.Add(checksum);
            await c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return response;
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

            // Apply LIKE filters for string fields
            if (!string.IsNullOrWhiteSpace(request.TableName))
            {
                query = query.Where(e => EF.Functions.Like(e.TableName ?? "", $"%{request.TableName}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Operation))
            {
                query = query.Where(e => EF.Functions.Like(e.Operation, $"%{request.Operation}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                query = query.Where(e => EF.Functions.Like(e.UserName, $"%{request.UserName}%"));
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

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and get raw entities (without DTO projection)
            var pagedEntities = await query
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 50)
                .ToListAsync(cancellationToken);

            // Project to DTOs in memory after materialization
            var dtos = pagedEntities.Select(e => new AuditEventDto
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
                        OriginalValue = c.OriginalValue,
                        NewValue = c.NewValue
                    }).ToList()
                    : null
            }).ToList();

            // Return paginated response
            return new PaginatedResponseDto<AuditEventDto>
            {
                Results = dtos,
                Total = totalCount
            };
        }, cancellationToken);
    }

    public static IEnumerable<KeyValuePair<string, KeyValuePair<decimal, byte[]>>> ToKeyValuePairs<TReport>(TReport obj)
    where TReport : class
    {
        var result = new List<KeyValuePair<string, decimal>>();
        var type = obj.GetType();

        // Check if the class itself has the YearEndArchivePropertyAttribute
        bool classHasAttribute = Attribute.IsDefined(type, typeof(YearEndArchivePropertyAttribute));

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
            // Convert all numeric types to decimal for consistent hashing
            var rawValue = prop.GetValue(obj);
            var value = ConvertToDecimal(rawValue);
            result.Add(new KeyValuePair<string, decimal>(prop.Name, value));
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
