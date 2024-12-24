using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

/// <summary>
///     Invokes the Profit Share Update Service to compute and return the transactions (PROFIT_DETAIL rows) based on user input.
///     Modeled after PAY447
///
///     This class follows the name of the step in the Ready YE flow.    It could instead be named "View Transactions for YE Update"
/// </summary>
public class ProfitShareEditService : IProfitShareEditService
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;

    public ProfitShareEditService(IProfitShareUpdateService profitShareUpdateService)
    {
        _profitShareUpdateService = profitShareUpdateService;
    }

    public async Task<ProfitShareEditResponse> ProfitShareEdit(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        ProfitShareUpdateResponse psur = await _profitShareUpdateService.ProfitShareUpdate(profitShareUpdateRequest, cancellationToken);
        
        // THIS WORK IS NOT COMPLETE, It is enough to provide data to the endpoint.
        // Completing this service is part of PS-573
        
        return new ProfitShareEditResponse
        {
            ReportName = "Profit Sharing Edit",
            ReportDate = DateTimeOffset.Now,
            BeginningBalance = 1,
            ContributionGrandTotal = 2,
            IncomingForfeitureGrandTotal = 3,
            EarningsGrandTotal = 4,
            Response = new PaginatedResponseDto<ProfitShareEditMemberRecordResponse>
            {
                Results = psur.Response.Results.Select(t => new ProfitShareEditMemberRecordResponse
                {
                    Badge = t.Badge,
                    Psn = t.Psn,
                    Name = t.Name,
                    Code = 0,
                    ContributionAmount = t.Contributions,
                    EarningsAmount = t.Earnings,
                    IncomingForfeitures = t.IncomingForfeitures,
                    Reason = "TBD" // We need to do the work to compute this, in PS-573
                })
            }
        };
    }
}
