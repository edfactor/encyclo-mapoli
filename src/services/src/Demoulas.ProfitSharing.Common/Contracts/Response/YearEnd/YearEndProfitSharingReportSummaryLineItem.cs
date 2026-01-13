using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record YearEndProfitSharingReportSummaryLineItem
{
    public required string Subgroup { get; set; }
    public required string LineItemPrefix { get; set; }
    public required string LineItemTitle { get; set; }
    public int NumberOfMembers { get; set; }
    public decimal TotalWages { get; set; }
    public decimal? TotalHours { get; set; }
    [MaskSensitive] public int? TotalPoints { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalPriorBalance { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static YearEndProfitSharingReportSummaryLineItem ResponseExample()
    {
        return new YearEndProfitSharingReportSummaryLineItem
        {
            Subgroup = "ACTIVE AND INACTIVE",
            LineItemPrefix = "2",
            LineItemTitle = "AGE 21-30 WITH >= 1000 PS HOURS",
            NumberOfMembers = 45,
            TotalWages = 1850000.00m,
            TotalHours = 850,
            TotalPoints = 4200,
            TotalBalance = 185000.00m,
            TotalPriorBalance = 155000.00m
        };
    }
}
