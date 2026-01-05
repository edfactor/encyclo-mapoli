using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BreakdownTotalsByStoreRequest : ProfitYearRequest
{
    public bool? StoreManagement { get; set; }

    /// <summary>
    /// Store number (required). Bound from route.
    /// </summary>
    public short StoreNumber { get; set; }

    public int? BadgeNumber { get; set; }

    [MaskSensitive]
    public string? EmployeeName { get; set; }

    public static new BreakdownTotalsByStoreRequest RequestExample()
    {
        return new BreakdownTotalsByStoreRequest
        {
            ProfitYear = 2024,
            StoreManagement = true,
            StoreNumber = 22,
            BadgeNumber = 123456,
            EmployeeName = "Smith"
        };
    }
}
