using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IAdhocTerminatedEmployeesService
{
    Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployees(StartAndEndDateRequest req, CancellationToken cancellationToken);
    Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployeesNeedingFormLetter(FilterableStartAndEndDateRequest req, CancellationToken cancellationToken);
    Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployeesNeedingFormLetter(TerminatedLettersRequest req, CancellationToken cancellationToken);
    Task<string> GetFormLetterForTerminatedEmployees(StartAndEndDateRequest startAndEndDateRequest, CancellationToken cancellationToken);
    Task<string> GetFormLetterForTerminatedEmployees(TerminatedLettersRequest terminatedLettersRequest, CancellationToken cancellationToken);
}
