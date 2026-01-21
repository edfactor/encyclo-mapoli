using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Services.Services.InquiriesAndAdjustments.MasterInquiry;

/// <summary>
/// Service for employee/demographic master inquiry operations.
/// Handles all employee-specific queries, filtering, and detail retrieval for the Master Inquiry feature.
/// </summary>
public interface IEmployeeMasterInquiryService
{
    /// <summary>
    /// Gets a queryable collection of MasterInquiryItem for employees/demographics
    /// with profit details and related entities.
    /// </summary>
    /// <param name="req">Optional request containing pre-filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A queryable collection of employee inquiry items.</returns>
    Task<IQueryable<MasterInquiryItem>> GetEmployeeInquiryQueryAsync(
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable collection of MasterInquiryItem for employees/demographics using an existing context.
    /// Use this overload when calling from within an existing UseReadOnlyContext scope to avoid nested context creation.
    /// </summary>
    /// <param name="ctx">The existing database context to use.</param>
    /// <param name="req">Optional request containing pre-filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A queryable collection of employee inquiry items.</returns>
    Task<IQueryable<MasterInquiryItem>> GetEmployeeInquiryQueryAsync(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed employee information for a specific demographic ID.
    /// Includes current/previous year data, missives, and duplicate detection.
    /// </summary>
    /// <param name="id">The demographic ID.</param>
    /// <param name="currentYear">The current profit year.</param>
    /// <param name="previousYear">The previous profit year.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the SSN and detailed member information.</returns>
    Task<(int ssn, MemberDetails? memberDetails)> GetEmployeeDetailsAsync(
        int id,
        short currentYear,
        short previousYear,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated employee details for a collection of SSNs.
    /// Optimized for batch retrieval with duplicate detection.
    /// </summary>
    /// <param name="req">Pagination and sorting request.</param>
    /// <param name="ssns">Collection of SSNs to retrieve.</param>
    /// <param name="currentYear">The current profit year.</param>
    /// <param name="previousYear">The previous profit year.</param>
    /// <param name="duplicateSsns">Set of SSNs known to have duplicates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated response containing employee details.</returns>
    Task<PaginatedResponseDto<MemberDetails>> GetEmployeeDetailsForSsnsAsync(
        MasterInquiryRequest req,
        ISet<int> ssns,
        short currentYear,
        short previousYear,
        ISet<int> duplicateSsns,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employee details for a collection of SSNs without pagination.
    /// Used for combined employee/beneficiary queries.
    /// </summary>
    /// <param name="ssns">Collection of SSNs to retrieve.</param>
    /// <param name="currentYear">The current profit year.</param>
    /// <param name="previousYear">The previous profit year.</param>
    /// <param name="duplicateSsns">Set of SSNs known to have duplicates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all employee details.</returns>
    Task<List<MemberDetails>> GetAllEmployeeDetailsForSsnsAsync(
        ISet<int> ssns,
        short currentYear,
        short previousYear,
        ISet<int> duplicateSsns,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to find an employee SSN by badge number when no profit details exist.
    /// Used for handling exact badge/SSN lookups for new employees.
    /// </summary>
    /// <param name="badgeNumber">The badge number to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The SSN if found, otherwise 0.</returns>
    Task<int> FindEmployeeSsnByBadgeAsync(
        int badgeNumber,
        CancellationToken cancellationToken = default);
}
