namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class DuplicateNamesAndBirthdaysResponse
{
    public required int BadgeNumber { get; set; }
    public required long SSN { get; set; }
    public string? Name { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required AddressResponseDto Address { get; set; }
    public required short Years { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public required int Count { get; set; }
    public required Decimal NetBalance { get; set; }
    public required decimal? HoursCurrentYear { get; set; }
    public required decimal? EarningsCurrentYear { get; set; }
}
