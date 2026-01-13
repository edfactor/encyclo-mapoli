using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DemographicBadgesNotInPayProfitResponse : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; } = string.Empty;
    [MaskSensitive] public required string FullName { get; set; }
    public short Store { get; set; }
    public char Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsExecutive { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static DemographicBadgesNotInPayProfitResponse ResponseExample()
    {
        return new DemographicBadgesNotInPayProfitResponse
        {
            BadgeNumber = 456789,
            Ssn = "***-**-5678",
            FullName = "Smith, Jennifer M",
            Store = 42,
            Status = 'A',
            StatusName = "Active",
            IsExecutive = false
        };
    }
}
