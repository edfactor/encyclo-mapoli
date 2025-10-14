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
    /// <param name="req">Optional master inquiry request with filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queryable of beneficiary inquiry items</returns>
    Task<IQueryable<MasterInquiryItem>> GetBeneficiaryInquiryQueryAsync(
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed beneficiary information by ID.
    /// </summary>
    /// <param name="id">Beneficiary ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple of SSN and member details, or (0, null) if not found</returns>
    Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated beneficiary details for a set of SSNs with sorting support.
    /// </summary>
    /// <param name="req">Pagination and sorting request</param>
    /// <param name="ssns">Set of SSNs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated response of member details</returns>
    Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsnsAsync(
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all (non-paginated) beneficiary details for a set of SSNs.
    /// </summary>
    /// <param name="ssns">Set of SSNs to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of member details</returns>
    Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsnsAsync(
        ISet<int> ssns,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a beneficiary SSN by badge number and suffix.
    /// </summary>
    /// <param name="badgeNumber">Badge number</param>
    /// <param name="psnSuffix">PSN suffix for beneficiaries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SSN if found, 0 otherwise</returns>
    Task<int> FindBeneficiarySsnByBadgeAsync(
        int badgeNumber,
        short psnSuffix,
        CancellationToken cancellationToken = default);
}
