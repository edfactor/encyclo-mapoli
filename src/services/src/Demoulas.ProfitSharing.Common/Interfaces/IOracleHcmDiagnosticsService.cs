using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for OracleHcm synchronization diagnostics and audit trail management.
/// </summary>
public interface IOracleHcmDiagnosticsService
{
    /// <summary>
    /// Gets the most recent create and modify timestamps from Demographic and PayProfit tables.
    /// </summary>
    Task<Result<OracleHcmSyncMetadata>> GetOracleHcmSyncMetadataAsync(CancellationToken ct);

    /// <summary>
    /// Gets demographic sync audit records with pagination, ordered by Created date (descending).
    /// </summary>
    Task<Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>> GetDemographicSyncAuditAsync(SortedPaginationRequestDto request, CancellationToken ct = default);

    /// <summary>
    /// Gets demographic sync audit records with simple pagination parameters (no validation).
    /// Records are ordered by Created date (descending).
    /// </summary>
    Task<Result<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>> GetDemographicSyncAuditAsync(int skip, int take, CancellationToken ct = default);

    /// <summary>
    /// Clears all records from the DEMOGRAPHIC_SYNC_AUDIT table and returns the count of deleted records.
    /// </summary>
    Task<Result<int>> ClearDemographicSyncAuditAsync(CancellationToken ct);
}
