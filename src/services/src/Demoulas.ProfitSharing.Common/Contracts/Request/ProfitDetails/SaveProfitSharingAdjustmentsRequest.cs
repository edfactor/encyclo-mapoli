using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;

public sealed record SaveProfitSharingAdjustmentsRequest : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int BadgeNumber { get; init; }

    public required IReadOnlyList<ProfitSharingAdjustmentRowRequest> Rows { get; init; }

    public static SaveProfitSharingAdjustmentsRequest RequestExample()
    {
        return new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = 2024,
            BadgeNumber = 1001,
            Rows = new List<ProfitSharingAdjustmentRowRequest>
            {
                ProfitSharingAdjustmentRowRequest.RequestExample()
            }
        };
    }
}
