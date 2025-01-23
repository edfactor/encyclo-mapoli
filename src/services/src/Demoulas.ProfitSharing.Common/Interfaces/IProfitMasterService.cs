using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// ProfitMasterUpdate - inserts or deletes PROFIT_DETAIL rows
/// </summary>
public interface IProfitMasterService
{
    public Task<ProfitMasterResponse> Update(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

    public Task<ProfitMasterResponse> Revert(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken);
}
