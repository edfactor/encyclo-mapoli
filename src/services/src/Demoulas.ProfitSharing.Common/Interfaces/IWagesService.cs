using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IWagesService
{
    Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesCurrentYearReport(PaginationRequestDto request, CancellationToken cancellationToken);
    Task<ReportResponseBase<WagesPreviousYearResponse>> GetWagesPreviousYearReport(PaginationRequestDto request, CancellationToken cancellationToken);
}
