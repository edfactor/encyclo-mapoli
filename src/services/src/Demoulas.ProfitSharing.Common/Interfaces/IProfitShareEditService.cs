using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Profit Share Edit - provides the contents of the Transactions (PROFIT_DETAIL rows) that would be generated using the users parameters (contribution %, earnings %, incoming forfeiture %)
/// </summary>
public interface IProfitShareEditService
{
    // external method used by endpoints to return data to browser.   No SSN in this result.
    Task<ProfitShareEditResponse> ProfitShareEdit(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);
}
