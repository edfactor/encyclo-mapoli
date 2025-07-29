using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.Common.Contracts.Interfaces;
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
        short profitYear, 
        TRequest request,
        TReport report,
        List<KeyValuePair<string, decimal>> kevValues,
        CancellationToken cancellationToken)
        where TReport : class where TRequest : class
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report), "Report cannot be null");
        }

        string requestJson = JsonSerializer.Serialize(request);
        string reportJson = JsonSerializer.Serialize(report);


        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = reportJson } };
        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = _appUser.UserName ?? string.Empty, ChangesJson = entries };


        ReportChecksum checksum = new ReportChecksum { ReportType = reportName, ProfitYear = profitYear, RequestJson = requestJson, ReportJson = reportJson };

        foreach (var kevValue in kevValues)
        {
            var hash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(kevValue.Value));
            var result = new KeyValuePair<string, KeyValuePair<decimal, byte[]>>(kevValue.Key, new KeyValuePair<decimal, byte[]>(kevValue.Value, hash));
            checksum.KeyFieldsChecksumJson.Add(result);
        }

        return _dataContextFactory.UseWritableContext(c =>
        {
            c.AuditEvents.Add(auditEvent);
            c.ReportChecksums.Add(checksum);
            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
