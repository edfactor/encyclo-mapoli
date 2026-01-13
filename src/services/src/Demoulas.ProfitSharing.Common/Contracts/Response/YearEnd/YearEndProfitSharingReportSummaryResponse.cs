using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public sealed record YearEndProfitSharingReportSummaryResponse
{
    public required List<YearEndProfitSharingReportSummaryLineItem> LineItems { get; set; }

    public static YearEndProfitSharingReportSummaryResponse SampleResponse()
    {
        return new YearEndProfitSharingReportSummaryResponse()
        {
            LineItems =
            [
                new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "ACTIVE AND INACTIVE",
                    LineItemPrefix = "1",
                    LineItemTitle = "AGE 18-20 WITH >= 1000 PS HOURS",
                    NumberOfMembers = 5,
                    TotalWages = 95842.45m,
                    TotalBalance = 0m,
                    TotalHours = 123,
                    TotalPoints = 321
                }
            ]
        };
    }
}
