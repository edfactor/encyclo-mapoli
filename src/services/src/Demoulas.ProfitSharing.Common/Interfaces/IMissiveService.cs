using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMissiveService
{
    Task<List<int>> DetermineMissivesForSsn(int ssn, short profitYear, CancellationToken cancellation);
    Task<List<MissiveResponse>> GetAllMissives(CancellationToken token);
}
