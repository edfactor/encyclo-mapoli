namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ProfitShareUpdateResponse : ReportResponseBase<ProfitShareUpdateMemberResponse>
{
    public required bool HasExceededMaximumContributions { get; set; }
}
