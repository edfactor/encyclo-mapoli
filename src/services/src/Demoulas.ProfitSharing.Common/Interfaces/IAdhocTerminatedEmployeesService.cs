using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IAdhocTerminatedEmployeesService
{
    Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployees(FrozenProfitYearRequest req, CancellationToken cancellationToken);
}
