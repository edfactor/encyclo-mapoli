using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IExecutiveHoursAndDollarsService
{
    /// <summary>
    /// Generates a report of executives with hours and dollar values
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetExecutiveHoursAndDollarsReport(ProfitYearRequest request, CancellationToken cancellationToken);

}
