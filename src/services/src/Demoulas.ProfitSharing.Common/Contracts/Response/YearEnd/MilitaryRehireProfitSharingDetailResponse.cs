namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MilitaryRehireProfitSharingDetailResponse
{
    public required short ProfitYear { get; set; }
    public required decimal Forfeiture { get; set; }
    public required string? Remark { get; set; }
}
