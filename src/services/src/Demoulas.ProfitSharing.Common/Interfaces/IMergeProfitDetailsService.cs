using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMergeProfitDetailsService
{
    Task<Result<bool>> MergeProfitDetailsToDemographic(int sourceDemographic, int targetDemographic, CancellationToken cancellationToken = default);
}
