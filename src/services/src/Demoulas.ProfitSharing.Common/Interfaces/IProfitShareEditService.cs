using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Profit Share Edit - provides the contents of the Transactions (PROFIT_DETAIL rows) that would be generated using the users parameters (contribution %, earnings %, incoming forfeiture %) 
/// </summary>
public interface IProfitShareEditService
{
    public Task<ProfitShareEditResponse> ProfitShareEdit(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);
}
