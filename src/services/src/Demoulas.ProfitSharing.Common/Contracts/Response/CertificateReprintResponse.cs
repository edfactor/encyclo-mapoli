using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record CertificateReprintResponse
{
    public required short StoreNumber { get; init; }
    public required int BadgeNumber { get; init; }
    [MaskSensitive] public required string FullName { get; init; } = string.Empty;

    public required byte PayClassificationId { get; init; }
    public required string PayClassificationName { get; init; } = string.Empty;

    public decimal BeginningBalance { get; init; }
    public decimal Earnings { get; init; }
    public decimal Contributions { get; init; }
    public decimal Forfeitures { get; init; }
    public decimal Distributions { get; init; }
    public decimal EndingBalance { get; init; }
    public decimal VestedAmount { get; init; }
    public byte VestedPercent { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public byte? EnrollmentId { get; init; }
    public decimal ProfitShareHours { get; init; }
    [MaskSensitive] public string Street1 { get; set; } = string.Empty;
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public int CertificateSort { get; set; }
    public decimal? AnnuitySingleRate { get; set; }
    public decimal? AnnuityJointRate { get; set; }
    public decimal? MonthlyPaymentSingle { get; set; }
    public decimal? MonthlyPaymentJoint { get; set; }

    public static CertificateReprintResponse ResponseExample()
    {
        return new CertificateReprintResponse() 
        {
            StoreNumber = 1,
            BadgeNumber = 12345,
            FullName = "Public, John",
            PayClassificationId = 1,
            PayClassificationName = "Full Time",
            BeginningBalance = 1000.00M,
            Earnings = 500.00M,
            Contributions = 200.00M,
            Forfeitures = 0.00M,
            Distributions = 300.00M,
            EndingBalance = 1400.00M,
            VestedAmount = 1400.00M,
            VestedPercent = 100,
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
            TerminationDate = null,
            EnrollmentId = 2,
            ProfitShareHours = 1500.00M,
            PostalCode = "02110",
            City = "Boston",
            State = "MA",
            Street1 = "100 Main St",
            CertificateSort = 1,
            AnnuitySingleRate = 10.00M,
            AnnuityJointRate = 12.00M,
            MonthlyPaymentSingle = 140.00M,
            MonthlyPaymentJoint = 120.00M
        };
    }
}
