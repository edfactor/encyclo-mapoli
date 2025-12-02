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
}
