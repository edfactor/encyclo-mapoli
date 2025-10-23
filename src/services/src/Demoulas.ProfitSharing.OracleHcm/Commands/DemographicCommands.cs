using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Commands;

/// <summary>
/// Command to add a demographics audit record.
/// </summary>
public sealed class AddAuditCommand : IDemographicCommand
{
    private readonly DemographicsAudit _auditRecord;

    public AddAuditCommand(DemographicsAudit auditRecord)
    {
        _auditRecord = auditRecord;
    }

    public async Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        await context.DemographicsAudits.AddAsync(_auditRecord, ct);
    }
}

/// <summary>
/// Command to add a demographics history record.
/// </summary>
public sealed class AddHistoryCommand : IDemographicCommand
{
    private readonly DemographicsHistory _historyRecord;

    public AddHistoryCommand(DemographicsHistory historyRecord)
    {
        _historyRecord = historyRecord;
    }

    public async Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        await context.DemographicsHistories.AddAsync(_historyRecord, ct);
    }
}

/// <summary>
/// Command to add a new demographic record.
/// </summary>
public sealed class AddDemographicCommand : IDemographicCommand
{
    private readonly Demographic _demographic;

    public AddDemographicCommand(Demographic demographic)
    {
        _demographic = demographic;
    }

    public async Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        await context.Demographics.AddAsync(_demographic, ct);
    }
}

/// <summary>
/// Command to update demographic values (entity is already tracked).
/// </summary>
public sealed class UpdateDemographicCommand : IDemographicCommand
{
    private readonly Demographic _demographic;

    public UpdateDemographicCommand(Demographic demographic)
    {
        _demographic = demographic;
    }

    public Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        context.Demographics.Update(_demographic);
        return Task.CompletedTask;
    }
}
