using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record ForfeituresAndPointsForYearResponseWithTotals : ReportResponseBase<ForfeituresAndPointsForYearResponse>
{
    [YearEndArchiveProperty]
    public decimal TotalForfeitures { get; set; }

    [MaskSensitive]
    [YearEndArchiveProperty]
    public int TotalForfeitPoints { get; set; }

    [MaskSensitive]
    [YearEndArchiveProperty]
    public int TotalEarningPoints { get; set; }

    [YearEndArchiveProperty]
    public decimal? TotalProfitSharingBalance { get; set; }

    /// <summary>
    /// Total distributions for the year (matches PAY444.DISTRIB, QPAY129.Distributions, QPAY066TA.TotalDisbursements)
    /// </summary>
    [YearEndArchiveProperty]
    public decimal? DistributionTotals { get; set; }

    [YearEndArchiveProperty]
    public decimal? AllocationToTotals { get; set; }

    [YearEndArchiveProperty]
    public decimal? AllocationsFromTotals { get; set; }

    public ValidationResponse? CrossReferenceValidation { get; set; }

    public static ForfeituresAndPointsForYearResponseWithTotals ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponseWithTotals
        {
            ReportName = "Forfeitures and Points for Year",
            ReportDate = DateTimeOffset.Now,
            TotalForfeitures = 1234.56m,
            TotalForfeitPoints = 15000,
            TotalEarningPoints = 85000,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ForfeituresAndPointsForYearResponse>
            {
                Results = new List<ForfeituresAndPointsForYearResponse> { ForfeituresAndPointsForYearResponse.ResponseExample() }
            }
        };
    }
}
