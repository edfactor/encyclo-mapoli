using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitDetailReversalsService
{
    Task<Result<bool>> ReverseProfitDetailsAsync(int[] profitDetailIds, CancellationToken cancellationToken);
}
