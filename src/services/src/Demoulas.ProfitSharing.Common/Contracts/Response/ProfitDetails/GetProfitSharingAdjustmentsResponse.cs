using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsResponse : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int DemographicId { get; init; }

    public required int BadgeNumber { get; init; }

    public required IReadOnlyList<ProfitSharingAdjustmentRowResponse> Rows { get; init; }

    public static GetProfitSharingAdjustmentsResponse ResponseExample()
    {
        return new GetProfitSharingAdjustmentsResponse
        {
            ProfitYear = 2024,
            DemographicId = 123,
            BadgeNumber = 1001,
            Rows = new List<ProfitSharingAdjustmentRowResponse>
            {
                ProfitSharingAdjustmentRowResponse.ResponseExample()
            }
        };
    }
}
