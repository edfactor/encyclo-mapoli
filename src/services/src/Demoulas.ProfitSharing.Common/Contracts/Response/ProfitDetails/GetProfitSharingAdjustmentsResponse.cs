using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsResponse : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int DemographicId { get; init; }

    public required bool IsOver21AtInitialHire { get; init; }

    public required decimal CurrentBalance { get; init; }

    public required decimal VestedBalance { get; init; }

    public required int BadgeNumber { get; init; }

    public required IReadOnlyList<ProfitSharingAdjustmentRowResponse> Rows { get; init; }
}

public sealed record ProfitSharingAdjustmentRowResponse
{
    public int? ProfitDetailId { get; init; }

    public required int RowNumber { get; init; }

    public required short ProfitYear { get; init; }

    public required byte ProfitCodeId { get; init; }

    public required decimal Contribution { get; init; }

    public required decimal Earnings { get; init; }

    public required decimal Forfeiture { get; init; }

    public required decimal Payment { get; init; }

    public required byte? MonthToDate { get; init; }

    public required short? YearToDate { get; init; }

    public required decimal FederalTaxes { get; init; }

    public required decimal StateTaxes { get; init; }

    public required char TaxCodeId { get; init; }

    public required string TaxCodeName { get; init; }

    public required byte? CommentTypeId { get; init; }

    public required string CommentTypeName { get; init; }

    public required string? CommentRelatedCheckNumber { get; init; }

    public required string? CommentRelatedState { get; init; }

    public required bool CommentIsPartialTransaction { get; init; }

    public required decimal CurrentHoursYear { get; init; }

    public required decimal CurrentIncomeYear { get; init; }

    public required DateOnly? ActivityDate { get; init; }

    public required string Comment { get; init; }

    public required bool IsEditable { get; init; }
}
