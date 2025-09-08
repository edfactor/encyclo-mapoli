using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record DemographicBadgesNotInPayProfitResponse : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; } = string.Empty;
    [MaskSensitive] public required string EmployeeName { get; set; }
    public short Store { get; set; }
    public char Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsExecutive { get; set; }
}
