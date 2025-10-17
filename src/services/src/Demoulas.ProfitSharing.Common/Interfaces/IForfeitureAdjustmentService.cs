using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IForfeitureAdjustmentService
{
    public Task<Result<SuggestedForfeitureAdjustmentResponse>> GetSuggestedForfeitureAmount(SuggestedForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default);
    public Task<Result<bool>> UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default);
    public Task<Result<bool>> UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default);
}
