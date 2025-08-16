using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ITerminationAndRehireService
{
    /// <summary>
    /// Generates a report of employees who are on military leave and have been rehired.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<ReportResponseBase<EmployeesOnMilitaryLeaveResponse>> GetEmployeesOnMilitaryLeaveAsync(PaginationRequestDto req, CancellationToken cancellationToken);

}
