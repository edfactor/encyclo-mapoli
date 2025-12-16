using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;

public sealed record SaveProfitSharingAdjustmentsRequest : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int BadgeNumber { get; init; }

    public required int SequenceNumber { get; init; }

    public required IReadOnlyList<ProfitSharingAdjustmentRowRequest> Rows { get; init; }
}

public sealed record ProfitSharingAdjustmentRowRequest
{
    public int? ProfitDetailId { get; init; }

    public required int RowNumber { get; init; }

    public required byte ProfitYearIteration { get; init; }

    public required byte ProfitCodeId { get; init; }

    public required decimal Contribution { get; init; }

    public required decimal Earnings { get; init; }

    public required decimal Forfeiture { get; init; }

    public required DateOnly? ActivityDate { get; init; }

    public required string Comment { get; init; }
}
