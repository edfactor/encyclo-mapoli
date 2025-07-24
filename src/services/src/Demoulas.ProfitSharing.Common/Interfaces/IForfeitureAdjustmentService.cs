using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IForfeitureAdjustmentService
{
    public Task<ForfeitureAdjustmentReportResponse> GetForfeitureAdjustmentReportAsync(ForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default);
    public Task<ForfeitureAdjustmentReportDetail> UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default);
    public Task<List<ForfeitureAdjustmentReportDetail>> UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default);
}
