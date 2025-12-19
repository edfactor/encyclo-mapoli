using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsResponse : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int DemographicId { get; init; }

    public required int BadgeNumber { get; init; }

    public required IReadOnlyList<ProfitSharingAdjustmentRowResponse> Rows { get; init; }
}

public sealed record ProfitSharingAdjustmentRowResponse
{
    public int? ProfitDetailId { get; init; }

    /// <summary>
    /// Indicates whether this profit detail has already been reversed (another row references it).
    /// When true, the user should not be allowed to reverse this row again.
    /// </summary>
    public required bool HasBeenReversed { get; init; }

    public required int RowNumber { get; init; }

    public required short ProfitYear { get; init; }

    public required byte ProfitYearIteration { get; init; }

    public required byte ProfitCodeId { get; init; }

    public required string ProfitCodeName { get; init; }

    public required decimal Contribution { get; init; }

    public required decimal Earnings { get; init; }

    public required decimal Forfeiture { get; init; }

    public required decimal Payment { get; init; }

    public required decimal FederalTaxes { get; init; }

    public required decimal StateTaxes { get; init; }

    public required char TaxCodeId { get; init; }

    public required DateOnly? ActivityDate { get; init; }

    public required string Comment { get; init; }

    public required bool IsEditable { get; init; }

    public static ProfitSharingAdjustmentRowResponse ResponseExample()
    {
        return new ProfitSharingAdjustmentRowResponse
        {
            ProfitDetailId = 12345,
            HasBeenReversed = false,
            RowNumber = 1,
            ProfitYear = 2024,
            ProfitYearIteration = 1,
            ProfitCodeId = 1,
            ProfitCodeName = "Regular Contribution",
            Contribution = 5000.00m,
            Earnings = 2500.00m,
            Forfeiture = 0.00m,
            Payment = 7500.00m,
            FederalTaxes = 1875.00m,
            StateTaxes = 375.00m,
            TaxCodeId = 'S',
            ActivityDate = new DateOnly(2024, 12, 15),
            Comment = "Regular profit sharing contribution",
            IsEditable = true
        };
    }
}

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
