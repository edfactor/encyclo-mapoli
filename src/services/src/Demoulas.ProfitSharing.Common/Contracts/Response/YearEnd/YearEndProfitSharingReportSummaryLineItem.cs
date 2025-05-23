namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record YearEndProfitSharingReportSummaryLineItem
{
    public required string Subgroup { get; set; }
    public required string LineItemPrefix { get; set; }
    public required string LineItemTitle { get; set; }
    public int NumberOfMembers { get; set; }
    public Decimal TotalWages { get; set; }
    public Decimal TotalBalance { get; set; }
}