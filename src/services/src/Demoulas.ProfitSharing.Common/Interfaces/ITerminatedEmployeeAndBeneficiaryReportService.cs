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
    Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct);
}
