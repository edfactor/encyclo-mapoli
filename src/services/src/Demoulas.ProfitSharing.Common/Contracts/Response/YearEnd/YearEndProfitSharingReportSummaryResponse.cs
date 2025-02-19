namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record YearEndProfitSharingReportSummaryResponse
{
    public required List<YearEndProfitSharingReportSummaryLineItem> LineItems { get; set; }

    public static YearEndProfitSharingReportSummaryResponse SampleResponse()
    {
        return new YearEndProfitSharingReportSummaryResponse()
        {
            LineItems = new List<YearEndProfitSharingReportSummaryLineItem>
            {
                new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "ACTIVE AND INACTIVE",
                    LineItemPrefix = "1",
                    LineItemTitle="AGE 18-20 WITH >= 1000 PS HOURS",
                    NumberOfMembers = 5,
                    TotalWages = 95842.45m,
                    TotalBalance = 0m
                }
            }
        };
    }
}

public sealed record YearEndProfitSharingReportSummaryLineItem
{
    public required string Subgroup { get; set; }
    public required string LineItemPrefix { get; set; }
    public required string LineItemTitle { get; set; }
    public int NumberOfMembers { get; set; }
    public Decimal TotalWages { get; set; }
    public Decimal TotalBalance { get; set; }
}
