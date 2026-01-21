using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

/// <summary>
/// Service for OracleHcm synchronization diagnostics and audit trail management.
/// </summary>
public class OracleHcmDiagnosticsService : IOracleHcmDiagnosticsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;
    private readonly ILogger<OracleHcmDiagnosticsService> _logger;

    public OracleHcmDiagnosticsService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride,
        ILogger<OracleHcmDiagnosticsService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
        _logger = logger;
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
#pragma warning disable DSMPS001
                var demographicTimestamps = await ctx.Demographics
#pragma warning restore DSMPS001
                    .TagWith("GetOracleHcmSyncMetadata-Demographic")
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        LatestCreated = g.Min(d => d.CreatedAtUtc),
                        LatestModified = g.Max(d => d.ModifiedAtUtc != null ? d.ModifiedAtUtc : d.CreatedAtUtc)
                    })
                    .FirstOrDefaultAsync(ct);

                // Get earliest created and latest modified timestamps from PayProfit table
                var payProfitTimestamps = await ctx.PayProfits
                    .TagWith("GetOracleHcmSyncMetadata-PayProfit")
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        LatestCreated = g.Min(p => p.CreatedAtUtc),
                        LatestModified = g.Max(p => p.ModifiedAtUtc != null ? p.ModifiedAtUtc : p.CreatedAtUtc)
                    })
                    .FirstOrDefaultAsync(ct);

                return new OracleHcmSyncMetadata
                {
                    DemographicCreatedAtUtc = demographicTimestamps?.LatestCreated,
                    DemographicModifiedAtUtc = demographicTimestamps?.LatestModified,
                    PayProfitCreatedAtUtc = payProfitTimestamps?.LatestCreated,
                    PayProfitModifiedAtUtc = payProfitTimestamps?.LatestModified
                };
            }, ct);

            return Result<OracleHcmSyncMetadata>.Success(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get OracleHcm sync metadata");
            return Result<OracleHcmSyncMetadata>.Failure(Error.Unexpected("Unexpected error"));
        }
    }

    /// <summary>
    /// Gets demographic sync audit records, ordered by Created date (descending).
    /// Supports pagination using SortedPaginationRequestDto.
    /// </summary>
    public async Task<Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>> GetDemographicSyncAuditAsync(SortedPaginationRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.DemographicSyncAudit
                    .TagWith("GetDemographicSyncAudit")
                    .OrderByDescending(a => a.Created)
                    .Select(a => new DemographicSyncAuditRecordResponse
                    {
                        Id = a.Id,
                        BadgeNumber = a.BadgeNumber,
                        OracleHcmId = a.OracleHcmId,
                        Message = a.Message,
                        PropertyName = a.PropertyName,
                        InvalidValue = a.InvalidValue,
                        UserName = a.UserName,
                        Created = a.Created
                    });

                return await query.ToPaginationResultsAsync(request, ct);
            }, ct);

            return Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get demographic sync audit records (sorted pagination)");
            return Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>.Failure(Error.Unexpected("Unexpected error"));
        }
    }

    /// <summary>
    /// Gets demographic sync audit records with simple pagination parameters (no validation).
    /// Records are ordered by Created date (descending).
    /// </summary>
    public async Task<Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>> GetDemographicSyncAuditAsync(int skip, int take, CancellationToken ct = default)
    {
        try
        {
            var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Get total count
                var total = await ctx.DemographicSyncAudit
                    .TagWith("GetDemographicSyncAudit-Count")
                    .CountAsync(ct);

                // Get paginated results
                var items = await ctx.DemographicSyncAudit
                    .TagWith("GetDemographicSyncAudit-Paginated")
                    .OrderByDescending(a => a.Created)
                    .Skip(skip)
                    .Take(take)
                    .Select(a => new DemographicSyncAuditRecordResponse
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

                return new PaginatedResponseDto<DemographicSyncAuditRecordResponse>
                {
                    Results = items,
                    Total = total
                };
            }, ct);

            return Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get demographic sync audit records (skip/take pagination)");
            return Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>.Failure(Error.Unexpected("Unexpected error"));
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
            using (_guardOverride.AllowFor(Role.ITDEVOPS, Role.ADMINISTRATOR))
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
            _logger.LogError(ex, "Failed to clear demographic sync audit records");
            return Result<int>.Failure(Error.Unexpected("Unexpected error"));
        }
    }
}
