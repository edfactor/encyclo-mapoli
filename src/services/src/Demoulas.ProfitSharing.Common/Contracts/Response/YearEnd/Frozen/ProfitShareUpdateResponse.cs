using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record ProfitShareUpdateResponse : ReportResponseBase<ProfitShareUpdateMemberResponse>
{
    public required bool HasExceededMaximumContributions { get; set; }
    public required AdjustmentsSummaryDto AdjustmentsSummary { get; set; }
    public required ProfitShareUpdateTotals ProfitShareUpdateTotals { get; set; }

    /// <summary>
    /// Cross-reference validation results showing which prerequisite report values match/don't match.
    /// Includes validations for PAY443 (Forfeitures and Points), beginning balance continuity, etc.
    /// </summary>
    public MasterUpdateCrossReferenceValidationResponse? CrossReferenceValidation { get; set; }
}
