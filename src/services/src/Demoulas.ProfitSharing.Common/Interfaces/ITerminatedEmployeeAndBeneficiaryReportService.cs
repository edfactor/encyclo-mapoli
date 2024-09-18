using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ITerminatedEmployeeAndBeneficiaryReportService
{
    /// <summary>
    /// Generates a report of employees who are terminated (but not retired) and all beneficiaries.
    /// </summary>
    /// <param name="startDate">Fiscal year start date
    /// <param name="endDate">Fiscal year end date
    /// <param name="profitSharingYear">Specific years transactions to report on.  Using 9999.9 defaults to prior year before april, and current year otherwise.
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave and rehired.</returns>
    Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct);
}
