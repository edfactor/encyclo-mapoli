using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces
{
    public interface INegativeEtvaReportService
    {
        Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSsNsOnPayProfitResponseAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    }
}
