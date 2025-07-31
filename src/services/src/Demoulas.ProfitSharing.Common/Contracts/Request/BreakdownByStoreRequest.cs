namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BreakdownByStoreRequest : ProfitYearRequest
{
    public bool? StoreManagement { get; set; }
    public short? StoreNumber { get; set; }
    public int? BadgeNumber { get; set; }
    public string? EmployeeName { get; set; }
}
