using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record MemberYearSummaryDto : IIsExecutive
{
    public required short StoreNumber { get; init; }
    public required int BadgeNumber { get; init; }
    [MaskSensitive] public required string FullName { get; init; } = string.Empty;

    public required string PayClassificationId { get; init; }
    public required string PayClassificationName { get; init; } = string.Empty;

    public decimal BeginningBalance { get; init; }
    public decimal Earnings { get; init; }
    public decimal Contributions { get; init; }
    public decimal Forfeitures { get; init; }
    public decimal Distributions { get; init; }
    public decimal EndingBalance { get; init; }
    public decimal BeneficiaryAllocation { get; init; }
    public decimal VestedAmount { get; init; }
    public byte VestedPercent { get; init; }
    [MaskSensitive] public DateOnly DateOfBirth { get; init; }
    public DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; }

    /// <summary>
    /// Calculated current age. Returns 0 if DateOfBirth is default.
    /// </summary>
    [MaskSensitive]
    public short Age
    {
        get
        {
            if (DateOfBirth == default)
            {
                return 0;
            }

            return DateOfBirth.Age();
        }
    }

    /// <summary>
    /// Calculated age at termination. Returns 0 if DateOfBirth is default or TerminationDate is null.
    /// </summary>
    [MaskSensitive]
    public short AgeAtTermination
    {
        get
        {
            if (DateOfBirth == default || TerminationDate is null)
            {
                return 0;
            }

            return DateOfBirth.Age(TerminationDate.Value.ToDateTime(TimeOnly.MinValue));
        }
    }
    public byte? EnrollmentId { get; init; }
    public decimal ProfitShareHours { get; init; }
    [MaskSensitive] public string Street1 { get; set; } = string.Empty;
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public int CertificateSort { get; set; }
    public bool IsExecutive { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static MemberYearSummaryDto ResponseExample()
    {
        return new MemberYearSummaryDto
        {
            StoreNumber = 51,
            BadgeNumber = 111222,
            FullName = "Williams, Patricia A",
            PayClassificationId = "FT",
            PayClassificationName = "Full Time",
            BeginningBalance = 25000.00m,
            Earnings = 3500.00m,
            Contributions = 4000.00m,
            Forfeitures = 0.00m,
            Distributions = 1000.00m,
            EndingBalance = 31500.00m,
            BeneficiaryAllocation = 0.00m,
            VestedAmount = 28350.00m,
            VestedPercent = 90,
            DateOfBirth = new DateOnly(1978, 7, 22),
            HireDate = new DateOnly(2012, 1, 15),
            TerminationDate = null,
            EnrollmentId = 4,
            ProfitShareHours = 2000.00m,
            Street1 = "456 Oak Avenue",
            City = "Boston",
            State = "MA",
            PostalCode = "02101",
            CertificateSort = 1,
            IsExecutive = false
        };
    }
}

