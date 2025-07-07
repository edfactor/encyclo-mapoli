using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IAdhocTerminatedEmployeesService
{
    Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployees(FrozenProfitYearRequest req, CancellationToken cancellationToken);
}
