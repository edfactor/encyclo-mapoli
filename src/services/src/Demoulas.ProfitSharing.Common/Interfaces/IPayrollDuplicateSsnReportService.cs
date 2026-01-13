using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayrollDuplicateSsnReportService
{
    Task<bool> DuplicateSsnExistsAsync(CancellationToken ct);
    Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(SortedPaginationRequestDto req, CancellationToken ct);
}
