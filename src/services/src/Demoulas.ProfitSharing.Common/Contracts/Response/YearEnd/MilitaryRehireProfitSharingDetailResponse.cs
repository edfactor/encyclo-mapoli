namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record MilitaryRehireProfitSharingDetailResponse
{
    public required short ProfitYear { get; set; }
    public required decimal Forfeiture { get; set; }
    public required string? Remark { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public required byte EnrollmentId { get; set; }
    public required string EnrollmentName { get; set; }
    public byte ProfitCodeId { get; set; }
    public decimal Wages { get; set; }
    public decimal? SuggestedForfeiture { get; set; }

    // Need a property for the profit detail id from the PROFIT_DETAIL database table
    public int ProfitDetailId { get; set; }

    
}
