using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMilitaryAndRehireService
{
    /// <summary>
    /// Generates a report of employees who are on military leave and have been rehired.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<ReportResponseBase<MilitaryAndRehireReportResponse>> GetMilitaryAndRehireReport(PaginationRequestDto req, CancellationToken cancellationToken);

    
    /// <summary>
    /// Finds rehires who may be entitled to forfeitures taken out in prior years.
    /// </summary>
    /// <param name="req">The request containing the criteria for finding rehires.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a report response with details of rehires and their potential entitlements.</returns>
    Task<ReportResponseBase<MilitaryAndRehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYears(MilitaryAndRehireRequest req,
        CancellationToken cancellationToken);

    /// <summary>
    /// Generates a summary report of profit sharing for employees who are on military leave and have been rehired.
    /// </summary>
    /// <param name="req">The request details including pagination and reporting year.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the summary report response with profit sharing details for rehired military employees.</returns>
    Task<ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>> GetMilitaryAndRehireProfitSummaryReport(MilitaryAndRehireRequest req,
        CancellationToken cancellationToken);
}
