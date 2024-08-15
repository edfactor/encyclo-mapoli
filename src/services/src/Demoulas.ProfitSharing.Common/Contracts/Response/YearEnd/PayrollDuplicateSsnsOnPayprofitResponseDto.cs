namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayrollDuplicateSsnsOnPayprofitResponseDto
{
    public int Count { get; set; }
    public long EmployeeBadge { get; set; }
    public long PayProfitSSN { get; set; }
    public long EmployeeSSN { get; set; }
    public required string Name { get; set; }
    public short Store { get; set; }
    public char Status { get; set; }
    public DateOnly HireDate { get; set; }
    public decimal IncomeCurrentYear { get; set; }
    public DateOnly? RehireDate { get; set; }
    public DateOnly? TermDate { get; set; }

    public required AddressResponseDto Address { get; set; }
    public required ContactInfoResponseDto ContactInfo { get; set; }
}
