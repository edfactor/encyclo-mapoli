namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayrollDuplicateSSNResponseDto
{
    public long BadgeNumber { get; set; }
    public long SSN { get; set; }
    public string? Name { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required DateOnly HireDate { get; set; }
    public required DateOnly? TerminationDate { get; set; }
    public required DateOnly? RehireDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public required int ProfitSharingRecords { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public decimal HoursLastYear { get; set; }
    public required decimal EarningsCurrentYear { get; set; }
}
