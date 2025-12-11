using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ITerminatedEmployeeService
{
    /// <summary>
    /// Generates a report of employees who are terminated (but not retired) and all beneficiaries.
    /// Optionally filters by vested balance using VestedBalanceValue and VestedBalanceOperator.
    /// </summary>
    /// <param name="req">The request details including pagination, reporting year, and optional vested balance filter.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of terminated employees and beneficiaries.</returns>
    Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(FilterableStartAndEndDateRequest req, CancellationToken ct);

}
