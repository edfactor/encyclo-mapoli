namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public class MemberYearSummaryDto
{
    public short StoreNumber { get; init; }
    public int EnrollmentId { get; set; }
    public int BadgeNumber { get; set; }
    public string? Ssn { get; init; }
    public string? FullName { get; set; }
    public byte PayFrequencyId { get; init; }
    public byte DepartmentId { get; set; }
    public byte PayClassificationId { get; set; }
    public decimal BeginningBalance { get; set; }
    public decimal Earnings { get; set; }

    public decimal Contributions { get; set; }
    public decimal Forfeiture { get; set; }
    public decimal Distributions { get; set; }

    public decimal EndingBalance { get; set; }

    public decimal VestedAmount { get; set; }

    public decimal VestedPercentage { get; set; }
    public char EmploymentStatusId { get; set; }
}

