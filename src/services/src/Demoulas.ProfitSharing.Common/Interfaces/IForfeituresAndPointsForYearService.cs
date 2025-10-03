using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IForfeituresAndPointsForYearService
{
    Task<ForfeituresAndPointsForYearResponseWithTotals> GetForfeituresAndPointsForYearAsync(FrozenProfitYearRequest req, CancellationToken cancellationToken = default);
}
