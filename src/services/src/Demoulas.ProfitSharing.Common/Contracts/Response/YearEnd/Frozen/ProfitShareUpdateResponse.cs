using Demoulas.Common.Contracts.Contracts.Response;
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

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static ProfitShareUpdateResponse ResponseExample()
    {
        return new ProfitShareUpdateResponse
        {
            ReportName = "profit-share-update",
            ReportDate = DateTimeOffset.UtcNow,
            HasExceededMaximumContributions = false,
            AdjustmentsSummary = AdjustmentsSummaryDto.ResponseExample(),
            ProfitShareUpdateTotals = ProfitShareUpdateTotals.ResponseExample(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<ProfitShareUpdateMemberResponse>
            {
                Results = new List<ProfitShareUpdateMemberResponse> { ProfitShareUpdateMemberResponse.ResponseExample() }
            }
        };
    }
}
