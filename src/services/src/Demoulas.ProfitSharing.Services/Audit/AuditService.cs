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

    public Task ArchiveCompletedReportAsync<TReport>(string reportName, TReport report, CancellationToken cancellationToken)
        where TReport : class
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report), "Report cannot be null");
        }

        var entries = new List<AuditChangeEntry> { new() { ColumnName = "Report", NewValue = JsonSerializer.Serialize(report) } };

        var auditEvent = new AuditEvent { TableName = reportName, Operation = "Archive", UserName = _appUser.UserName ?? string.Empty, ChangesJson = entries };

        return _dataContextFactory.UseWritableContext(c =>
        {
            c.AuditEvents.Add(auditEvent);
            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
