using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// The ProfitShareUpdateTotals of all the YE Update Transactions (aka PROFIT_DETAIL rows)
/// </summary>
public sealed record ProfitShareEditResponse : ReportResponseBase<ProfitShareEditMemberRecordResponse>
{
    [YearEndArchiveProperty]
    public required decimal BeginningBalanceTotal { get; set; }
    [YearEndArchiveProperty]
    public required decimal ContributionGrandTotal { get; set; }
    [YearEndArchiveProperty]
    public required decimal IncomingForfeitureGrandTotal { get; set; }
    [YearEndArchiveProperty]
    public required decimal EarningsGrandTotal { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static ProfitShareEditResponse ResponseExample()
    {
        return new ProfitShareEditResponse
        {
            ReportName = "profit-share-edit",
            ReportDate = DateTimeOffset.UtcNow,
            BeginningBalanceTotal = 2500000.00m,
            ContributionGrandTotal = 350000.00m,
            IncomingForfeitureGrandTotal = 50000.00m,
            EarningsGrandTotal = 200000.00m,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<ProfitShareEditMemberRecordResponse>
            {
                Results = new List<ProfitShareEditMemberRecordResponse>
                {
                    new ProfitShareEditMemberRecordResponse { BadgeNumber = 100001, Psn = 123456L, FullName = "Jane Doe", IsExecutive = false }
                }
            }
        };
    }
}
