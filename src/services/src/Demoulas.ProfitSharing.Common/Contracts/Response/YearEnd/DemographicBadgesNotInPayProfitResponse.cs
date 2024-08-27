namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class DemographicBadgesNotInPayProfitResponse
{
    public required long EmployeeBadge { get; set; }
    public required long EmployeeSsn { get; set; }
    public required string EmployeeName { get; set; }
    public short Store { get; set; }
    public char Status { get; set; }
}
