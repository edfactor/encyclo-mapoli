using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BreakdownByStoreRequest : ProfitYearRequest
{
    public bool? StoreManagement { get; set; }
    public short? StoreNumber { get; set; }
    public int? BadgeNumber { get; set; }

    [MaskSensitive]
    public string? EmployeeName { get; set; }

    public static new BreakdownByStoreRequest RequestExample()
    {
        return new BreakdownByStoreRequest
        {
            ProfitYear = 2024,
            StoreManagement = true,
            StoreNumber = 22,
            BadgeNumber = 123456,
            EmployeeName = "Smith"
        };
    }
}
