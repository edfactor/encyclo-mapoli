using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ITerminatedEmployeeService
{
    /// <summary>
    /// Generates a report of employees who are terminated (but not retired) and all beneficiaries.
    /// </summary>
    /// <param name="req">The request details including pagination and reporting year.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(StartAndEndDateRequest req, CancellationToken ct);

}
