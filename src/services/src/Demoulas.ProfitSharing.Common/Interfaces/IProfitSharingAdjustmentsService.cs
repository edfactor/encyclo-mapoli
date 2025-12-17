using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitSharingAdjustmentsService
{
    Task<Result<GetProfitSharingAdjustmentsResponse>> GetAsync(GetProfitSharingAdjustmentsRequest request, CancellationToken ct);

    Task<Result<GetProfitSharingAdjustmentsResponse>> SaveAsync(SaveProfitSharingAdjustmentsRequest request, CancellationToken ct);
}
