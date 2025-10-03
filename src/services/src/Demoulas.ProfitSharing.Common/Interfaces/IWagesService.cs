using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IWagesService
{
    /// <summary>
    /// Retrieves the wages report for the current year.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response for the current year's wages.</returns>
    Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesReportAsync(ProfitYearRequest request, CancellationToken cancellationToken);


}
