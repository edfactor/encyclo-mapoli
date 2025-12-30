using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public record ProfitSharingLabelResponse : IIsExecutive
{
    public short StoreNumber { get; set; }
    public string PayClassificationId { get; set; } = string.Empty;
    public string? PayClassificationName { get; set; }
    public byte DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int BadgeNumber { get; set; }
    [MaskSensitive] public required string EmployeeName { get; set; }
    [MaskSensitive] public string? FirstName { get; set; }
    [MaskSensitive] public string? Address1 { get; set; }
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public required bool IsExecutive { get; set; }

    public static ProfitSharingLabelResponse ResponseExample()
    {
        return new ProfitSharingLabelResponse()
        {
            StoreNumber = 22,
            PayClassificationId = "2",
            PayClassificationName = "Assistant Manager",
            DepartmentId = 2,
            DepartmentName = "Grocery",
            BadgeNumber = 710023,
            EmployeeName = "Fred Michaels",
            Address1 = "Main St.",
            City = "Anytown",
            State = "MA",
            PostalCode = "02112",
            IsExecutive = false,
        };
    }
}
