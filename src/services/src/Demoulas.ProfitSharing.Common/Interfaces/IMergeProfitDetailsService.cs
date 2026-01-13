using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMergeProfitDetailsService
{
    Task<Result<bool>> MergeProfitDetailsToDemographic(int sourceSsn, int destinationSsn, CancellationToken cancellationToken = default);
}
