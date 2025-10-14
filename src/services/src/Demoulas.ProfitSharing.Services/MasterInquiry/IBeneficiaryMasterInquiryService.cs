using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Service interface for beneficiary-specific master inquiry operations.
/// Handles lookup, filtering, and retrieval of beneficiary profit sharing data.
/// </summary>
public interface IBeneficiaryMasterInquiryService
{
    /// <summary>
    /// Builds the query for beneficiary inquiry with optional filtering.
    /// </summary>
    /// <param name="ctx">Read-only database context</param>
    /// <param name="req">Optional master inquiry request with filter criteria</param>
    /// <returns>Queryable of beneficiary inquiry items</returns>
    IQueryable<MasterInquiryItem> GetBeneficiaryInquiryQuery(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null);

    /// <summary>
    /// Gets detailed beneficiary information by ID.
    /// </summary>
    /// <param name="ctx">Read-only database context</param>
    /// <param name="id">Beneficiary ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple of SSN and member details, or (0, null) if not found</returns>
    Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetailsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        int id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets paginated beneficiary details for a set of SSNs with sorting support.
    /// </summary>
    /// <param name="ctx">Read-only database context</param>
    /// <param name="req">Pagination and sorting request</param>
    /// <param name="ssns">Set of SSNs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated response of member details</returns>
    Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsnsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all (non-paginated) beneficiary details for a set of SSNs.
    /// </summary>
    /// <param name="ctx">Read-only database context</param>
    /// <param name="ssns">Set of SSNs to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of member details</returns>
    Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsnsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        ISet<int> ssns,
        CancellationToken cancellationToken);

    /// <summary>
    /// Finds a beneficiary SSN by badge number and suffix.
    /// </summary>
    /// <param name="ctx">Read-only database context</param>
    /// <param name="badgeNumber">Badge number</param>
    /// <param name="psnSuffix">PSN suffix for beneficiaries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SSN if found, 0 otherwise</returns>
    Task<int> FindBeneficiarySsnByBadgeAsync(
        ProfitSharingReadOnlyDbContext ctx,
        int badgeNumber,
        short psnSuffix,
        CancellationToken cancellationToken);
}
