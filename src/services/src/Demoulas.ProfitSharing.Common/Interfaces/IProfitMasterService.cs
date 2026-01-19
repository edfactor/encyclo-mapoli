using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// ProfitMasterUpdate - inserts or deletes PROFIT_DETAIL rows
/// </summary>
public interface IProfitMasterService
{
    Task<ProfitMasterUpdateResponse?> StatusAsync(ProfitYearRequest profitShareUpdateRequest, CancellationToken cancellationToken);

    Task<ProfitMasterUpdateResponse> UpdateAsync(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken ct);

    Task<ProfitMasterRevertResponse> RevertAsync(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken);
}
