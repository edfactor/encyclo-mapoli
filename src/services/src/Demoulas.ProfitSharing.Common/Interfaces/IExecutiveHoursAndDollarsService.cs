using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IExecutiveHoursAndDollarsService
{
    /// <summary>
    /// Generates a report of executives with hours and dollar values
    /// </summary>
    /// <param name="request">The profit year and pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetExecutiveHoursAndDollarsReportAsync(ExecutiveHoursAndDollarsRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Sets Executive hours and dollars.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    /// <param name="profitYear">The profit year </param>
    /// <param name="executiveHoursAndDollarsDtos">The list of executive badges, hours, and dollars updates to apply to the system</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>if the data is correctly updated, nothing is returned.   If a validation or transaction problem occurs, appropriate exceptions are thrown</returns>
    Task SetExecutiveHoursAndDollarsAsync(short profitYear, List<SetExecutiveHoursAndDollarsDto> executiveHoursAndDollarsDtos, CancellationToken cancellationToken);

}
