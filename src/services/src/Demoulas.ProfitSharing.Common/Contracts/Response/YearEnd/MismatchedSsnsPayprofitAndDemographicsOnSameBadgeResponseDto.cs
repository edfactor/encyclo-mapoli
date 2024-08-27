namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto
{
    public long EmployeeBadge { get; set; }
    public long PayProfitSsn { get; set; }
    public long EmployeeSsn { get; set; }
    public required string Name { get; set; }
    public short Store { get; set; }
    public char Status { get; set; }
}
