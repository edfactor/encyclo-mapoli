namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayrollDuplicateSSNResponseDto
{
    public required long BadgeNumber { get; set; }
    public required long SSN { get; set; }
    public string? Name { get; set; }
    public required AddressResponseDto Address { get; set; }
    public required DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public DateOnly? RehireDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public required int ProfitSharingRecords { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public decimal HoursLastYear { get; set; }
    public required decimal EarningsCurrentYear { get; set; }
}
