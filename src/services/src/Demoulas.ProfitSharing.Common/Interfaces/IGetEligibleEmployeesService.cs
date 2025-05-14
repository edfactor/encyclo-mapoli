using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IGetEligibleEmployeesService
{
    /// <summary>
    /// Generates a report of eligible employees
    /// </summary>
    /// <param name="request">The profit year and pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of eligible employees</returns>
    Task<GetEligibleEmployeesResponse> GetEligibleEmployeesAsync(ProfitYearRequest request, CancellationToken cancellationToken);

}
