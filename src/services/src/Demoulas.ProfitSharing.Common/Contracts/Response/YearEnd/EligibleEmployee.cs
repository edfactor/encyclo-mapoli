
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record EligibleEmployee : IFullNameProperty, IIsExecutive
{
    public required long OracleHcmId { get; set; }
    public required int BadgeNumber { get; set; }
    [MaskSensitive] public required string FullName { get; set; }
    public byte DepartmentId { get; set; }
    public string? Department { get; set; }
    public short StoreNumber { get; set; }
    public required bool IsExecutive { get; set; }

    public static EligibleEmployee Example()
    {
        return new EligibleEmployee
        {
            OracleHcmId = 42,
            BadgeNumber = 721,
            FullName = "John, Null E",
            Department = "Grocery",
            DepartmentId = 1,
            IsExecutive = false,
        };
    }
}
