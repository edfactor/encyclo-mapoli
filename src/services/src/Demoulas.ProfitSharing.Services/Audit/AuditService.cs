using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Audit;

public sealed class AuditService : IAuditService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAppUser? _appUser;

    public AuditService(IProfitSharingDataContextFactory dataContextFactory, IAppUser? appUser)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
    }

    public async Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse, TResult>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : ReportResponseBase<TResult>
        where TRequest : PaginationRequestDto
        where TResult : class
    {
        if (reportFunction == null)
        {
            throw new ArgumentNullException(nameof(reportFunction), "Report function cannot be null.");
        }

        // Create archive request with full data retrieval (Skip=0, Take=max)
        TRequest archiveRequest = CreateArchiveRequest(request);

        TResponse response = await reportFunction(archiveRequest, cancellationToken);

        string requestJson = JsonSerializer.Serialize(request);
        string reportJson = JsonSerializer.Serialize(response);

        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = reportJson } };
        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = _appUser?.UserName ?? string.Empty, ChangesJson = entries };

        ReportChecksum checksum = new ReportChecksum { ReportType = reportName, ProfitYear = profitYear, RequestJson = requestJson, ReportJson = reportJson };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(response);

        await _dataContextFactory.UseWritableContext(async c =>
        {
            c.AuditEvents.Add(auditEvent);
            c.ReportChecksums.Add(checksum);
            await c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return response;
    }

    private static TRequest CreateArchiveRequest<TRequest>(TRequest originalRequest) 
        where TRequest : PaginationRequestDto
    {
        // For record types (like PaginationRequestDto), use 'with' expression
        // This works because PaginationRequestDto is a record type
        return originalRequest with { Skip = 0, Take = short.MaxValue };
    }

    public async Task ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        TRequest request,
        TResponse response,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto
    {
        string requestJson = JsonSerializer.Serialize(request);
        string reportJson = JsonSerializer.Serialize(response);

        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = reportJson } };
        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = _appUser?.UserName ?? string.Empty, ChangesJson = entries };

        // For this overload, we don't have a profit year from the request, so we'll set it to 0 or extract it if possible
        short profitYear = 0;
        var profitYearProp = request.GetType().GetProperty("ProfitYear");
        if (profitYearProp != null && profitYearProp.PropertyType == typeof(short))
        {
            profitYear = (short)(profitYearProp.GetValue(request) ?? 0);
        }

        ReportChecksum checksum = new ReportChecksum { ReportType = reportName, ProfitYear = profitYear, RequestJson = requestJson, ReportJson = reportJson };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(response);

        await _dataContextFactory.UseWritableContext(async c =>
        {
            c.AuditEvents.Add(auditEvent);
            c.ReportChecksums.Add(checksum);
            await c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    public static IEnumerable<KeyValuePair<string, KeyValuePair<decimal, byte[]>>> ToKeyValuePairs<TReport>(TReport obj)
    where TReport : class
    {
        var result = new List<KeyValuePair<string, decimal>>();
        var type = obj.GetType();
        var properties = type.GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(YearEndArchivePropertyAttribute)) && p.PropertyType == typeof(decimal));
        
        foreach (var prop in properties)
        {
            var value = (decimal)(prop.GetValue(obj) ?? 0);
            result.Add(new KeyValuePair<string, decimal>(prop.Name, value));
        }

        foreach (var kevValue in result)
        {
            var hash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(kevValue.Value));
            var kvp = new KeyValuePair<string, KeyValuePair<decimal, byte[]>>(kevValue.Key, new KeyValuePair<decimal, byte[]>(kevValue.Value, hash));
            yield return kvp;
        }
    }
}
