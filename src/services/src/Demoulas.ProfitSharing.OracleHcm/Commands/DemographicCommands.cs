using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.OracleHcm.Commands;

/// <summary>
/// Command to add a demographic sync audit record.
/// </summary>
public sealed class AddAuditCommand : IDemographicCommand
{
    private readonly DemographicSyncAudit _auditRecord;

    public AddAuditCommand(DemographicSyncAudit auditRecord)
    {
        _auditRecord = auditRecord;
    }

    public async Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        await context.DemographicSyncAudit.AddAsync(_auditRecord, ct);
    }
}

/// <summary>
/// Command to add a demographic history record.
/// </summary>
public sealed class AddHistoryCommand : IDemographicCommand
{
    private readonly DemographicHistory _historyRecord;

    public AddHistoryCommand(DemographicHistory historyRecord)
    {
        _historyRecord = historyRecord;
    }

    public async Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        await context.DemographicHistories.AddAsync(_historyRecord, ct);
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

/// <summary>
/// Command to update SSN in BeneficiaryContacts using old SSN.
/// </summary>
public sealed class UpdateBeneficiaryContactsSsnCommand : IDemographicCommand
{
    private readonly int _oldSsn;
    private readonly int _newSsn;

    public UpdateBeneficiaryContactsSsnCommand(int oldSsn, int newSsn)
    {
        _oldSsn = oldSsn;
        _newSsn = newSsn;
    }

    public Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        return context.BeneficiaryContacts
            .Where(bc => bc.Ssn == _oldSsn)
            .ExecuteUpdateAsync(s => s.SetProperty(bc => bc.Ssn, _newSsn), ct);
    }
}

/// <summary>
/// Command to update SSN in ProfitDetails using old SSN.
/// </summary>
public sealed class UpdateProfitDetailsSsnCommand : IDemographicCommand
{
    private readonly int _oldSsn;
    private readonly int _newSsn;

    public UpdateProfitDetailsSsnCommand(int oldSsn, int newSsn)
    {
        _oldSsn = oldSsn;
        _newSsn = newSsn;
    }

    public Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct)
    {
        return context.ProfitDetails
            .Where(pd => pd.Ssn == _oldSsn)
            .ExecuteUpdateAsync(s => s.SetProperty(pd => pd.Ssn, _newSsn), ct);
    }
}
