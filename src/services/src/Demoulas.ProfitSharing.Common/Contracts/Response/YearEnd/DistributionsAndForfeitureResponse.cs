namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record DistributionsAndForfeitureResponse
{
    public required long BadgeNumber { get; set; }
    public required string EmployeeName { get; set; }
    public required string EmployeeSsn { get; set; }
    public DateOnly? LoanDate { get; set; }
    public required decimal DistributionAmount { get; set; }
    public required decimal StateTax { get; set; }
    public required decimal FederalTax { get; set; }
    public required decimal ForfeitAmount { get; set; }
    public byte? Age { get; set; }
    public char? TaxCode { get; set; }
    public string? OtherName { get; set; }
    public string? OtherSsn { get; set; }
    public byte? Enrolled { get; set; }

    public static DistributionsAndForfeitureResponse ResponseExample()
    {
        return new DistributionsAndForfeitureResponse() {
            BadgeNumber = 123,
            EmployeeName = "Doe, John",
            EmployeeSsn = "124",
            DistributionAmount = 1250.25m,
            StateTax = 25.12m,
            FederalTax = 51.52m,
            ForfeitAmount = 0m,
            Age = 33,
            TaxCode = '9',
            Enrolled = 1,
        };
    }
}
