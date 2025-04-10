using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// ProfitMasterUpdate - inserts or deletes PROFIT_DETAIL rows
/// </summary>
public interface IProfitMasterService
{
    Task<ProfitMasterUpdateResponse?> Status(ProfitYearRequest profitShareUpdateRequest, CancellationToken cancellationToken);

    Task<ProfitMasterUpdateResponse> Update(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

    Task<ProfitMasterRevertResponse> Revert(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken);
}
