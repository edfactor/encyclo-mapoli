using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMissiveService
{
    Task<Dictionary<int, List<int>>> DetermineMissivesForSsns(IEnumerable<int> ssns, short profitYear, CancellationToken cancellation);
    Task<List<MissiveResponse>> GetAllMissives(CancellationToken token);
}
