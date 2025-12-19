using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsRequest : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int BadgeNumber { get; init; }

    public bool GetAllRows { get; init; }

    public static GetProfitSharingAdjustmentsRequest RequestExample()
    {
        return new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = 2024,
            BadgeNumber = 1001,
            GetAllRows = false
        };
    }
}
