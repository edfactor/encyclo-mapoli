using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class NamesMissingCommaResponse: IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    public required string EmployeeName { get; set; }
    public required bool IsExecutive { get; set; }
}
