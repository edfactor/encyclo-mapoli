using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayBenReportService
{
    Task<PaginatedResponseDto<PayBenReportResponse>> GetPayBenReport(PayBenReportRequest request, CancellationToken cancellationToken);
}
