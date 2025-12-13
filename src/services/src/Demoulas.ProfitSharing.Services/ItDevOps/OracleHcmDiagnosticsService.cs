using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

/// <summary>
/// Service for OracleHcm synchronization diagnostics and audit trail management.
/// </summary>
public class OracleHcmDiagnosticsService : IOracleHcmDiagnosticsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;

    public OracleHcmDiagnosticsService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
    }

    /// <summary>
    /// Gets the most recent create and modify timestamps from Demographic and PayProfit tables.
    /// Returns four timestamp values: Demographic Created, Demographic Modified, PayProfit Created, PayProfit Modified.
    /// </summary>
    public async Task<Result<OracleHcmSyncMetadata>> GetOracleHcmSyncMetadataAsync(CancellationToken ct)
    {
        try
        {
            var metadata = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Get latest timestamps from PayProfit table for both table syncs
                var payProfitTimestamps = await ctx.PayProfits
                    .TagWith("GetOracleHcmSyncMetadata-PayProfit")
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        LatestCreated = g.Max(p => p.CreatedAtUtc),
                        LatestModified = g.Max(p => p.ModifiedAtUtc)
                    })
                    .FirstOrDefaultAsync(ct);

                // For Demographic timestamps, we use PayProfit as proxy since direct Demographics access is restricted
                // Both tables are synced together from OracleHcm
                return new OracleHcmSyncMetadata
                {
                    DemographicCreatedAtUtc = payProfitTimestamps?.LatestCreated,
                    DemographicModifiedAtUtc = payProfitTimestamps?.LatestModified,
                    PayProfitCreatedAtUtc = payProfitTimestamps?.LatestCreated,
                    PayProfitModifiedAtUtc = payProfitTimestamps?.LatestModified
                };
            }, ct);

            return Result<OracleHcmSyncMetadata>.Success(metadata);
        }
        catch (Exception ex)
        {
            return Result<OracleHcmSyncMetadata>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <summary>
    /// Gets demographic sync audit records, grouped by BadgeNumber and ordered by Created date (descending).
    /// Supports pagination.
    /// </summary>
    public async Task<Result<DemographicSyncAuditPage>> GetDemographicSyncAuditAsync(int pageNumber = 1, int pageSize = 50, CancellationToken ct = default)
    {
        try
        {
            // Validate pagination parameters
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1 || pageSize > 1000)
            {
                pageSize = 50;
            }

            var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Get total count
                var totalCount = await ctx.DemographicSyncAudit
                    .TagWith("GetDemographicSyncAudit-Count")
                    .CountAsync(ct);

                // Get paginated records, ordered by Created descending
                var records = await ctx.DemographicSyncAudit
                    .TagWith("GetDemographicSyncAudit-Records")
                    .OrderByDescending(a => a.Created)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new DemographicSyncAuditDto
                    {
                        Id = a.Id,
                        BadgeNumber = a.BadgeNumber,
                        OracleHcmId = a.OracleHcmId,
                        Message = a.Message,
                        PropertyName = a.PropertyName,
                        InvalidValue = a.InvalidValue,
                        UserName = a.UserName,
                        Created = a.Created
                    })
                    .ToListAsync(ct);

                return new DemographicSyncAuditPage
                {
                    Records = records,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }, ct);

            return Result<DemographicSyncAuditPage>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<DemographicSyncAuditPage>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <summary>
    /// Clears all records from the DEMOGRAPHIC_SYNC_AUDIT table.
    /// Returns the count of records deleted.
    /// </summary>
    public async Task<Result<int>> ClearDemographicSyncAuditAsync(CancellationToken ct)
    {
        try
        {
            using (_guardOverride.AllowFor(roles: Role.ITDEVOPS))
            {
                // Get count and delete in same writable context
                var count = await _dataContextFactory.UseWritableContext(async ctx =>
                {
                    // Get count before deleting
                    var recordCount = await ctx.DemographicSyncAudit
                        .TagWith("ClearDemographicSyncAudit-Count")
                        .CountAsync(ct);

                    // Delete all records
                    await ctx.DemographicSyncAudit
                        .TagWith("ClearDemographicSyncAudit-Delete")
                        .ExecuteDeleteAsync(ct);

                    return recordCount;
                }, ct);

                return Result<int>.Success(count);
            }
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(Error.Unexpected(ex.Message));
        }
    }
}
