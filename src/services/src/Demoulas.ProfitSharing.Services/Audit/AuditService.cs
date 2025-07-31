using System.Security.Cryptography;
using System.Text.Json;
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
    private readonly IAppUser _appUser;

    public AuditService(IProfitSharingDataContextFactory dataContextFactory, IAppUser appUser)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
    }

   
    public Task ArchiveCompletedReportAsync<TRequest, TReport>(string reportName,
        TRequest request,
        TReport report,
        CancellationToken cancellationToken)
        where TReport : class where TRequest : IProfitYearRequest
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report), "Report cannot be null");
        }

        string requestJson = JsonSerializer.Serialize(request);
        string reportJson = JsonSerializer.Serialize(report);


        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = reportJson } };
        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = _appUser.UserName ?? string.Empty, ChangesJson = entries };


        ReportChecksum checksum = new ReportChecksum { ReportType = reportName, ProfitYear = request.ProfitYear, RequestJson = requestJson, ReportJson = reportJson };
        checksum.KeyFieldsChecksumJson = ToKeyValuePairs(report);


        return _dataContextFactory.UseWritableContext(c =>
        {
            c.AuditEvents.Add(auditEvent);
            c.ReportChecksums.Add(checksum);
            return c.SaveChangesAsync(cancellationToken);
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
