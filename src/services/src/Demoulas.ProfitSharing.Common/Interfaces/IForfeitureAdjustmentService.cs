using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IForfeitureAdjustmentService
{
    public Task<SuggestedForfeitureAdjustmentResponse> GetSuggestedForfeitureAmount(SuggestedForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default);
    public Task UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default);
    public Task UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default);
}
