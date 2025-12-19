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

public sealed record ProfitSharingAdjustmentRowRequest
{
    public int? ProfitDetailId { get; init; }

    /// <summary>
    /// The ID of the profit detail that this adjustment reverses (for tracking purposes).
    /// When provided, the system will validate that the source has not already been reversed.
    /// </summary>
    public int? ReversedFromProfitDetailId { get; init; }

    public required int RowNumber { get; init; }

    public required byte ProfitCodeId { get; init; }

    public required decimal Contribution { get; init; }

    public required decimal Earnings { get; init; }

    public required decimal Forfeiture { get; init; }

    public required DateOnly? ActivityDate { get; init; }

    public required string Comment { get; init; }

    public static ProfitSharingAdjustmentRowRequest RequestExample()
    {
        return new ProfitSharingAdjustmentRowRequest
        {
            ProfitDetailId = null,
            ReversedFromProfitDetailId = null,
            RowNumber = 1,
            ProfitCodeId = 1,
            Contribution = 5000.00m,
            Earnings = 2500.00m,
            Forfeiture = 0.00m,
            ActivityDate = new DateOnly(2024, 12, 15),
            Comment = "New profit sharing contribution"
        };
    }
}
