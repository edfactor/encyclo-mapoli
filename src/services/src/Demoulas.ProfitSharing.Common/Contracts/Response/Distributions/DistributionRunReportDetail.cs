using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed record DistributionRunReportDetail
{
    public int BadgeNumber { get; set; }
    public byte DepartmentId { get; set; }
    public required string DepartmentName { get; set; }
    public required string PayClassificationId { get; set; }
    public required string PayClassificationName { get; set; }
    public int StoreNumber { get; set; }
    public char TaxCodeId { get; set;}
    public required string TaxCodeName { get; set; }
    [MaskSensitive]
    public required string EmployeeName { get; set; }
    public char EmploymentTypeId { get; set; }
    public required string EmploymentTypeName { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? FullTimeDate { get; set; }
    [MaskSensitive]
    public DateOnly DateOfBirth { get; set; }
    public int Age { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal StateTaxAmount { get; set; }
    public decimal FederalTaxAmount { get; set; }
    public decimal CheckAmount { get; set; }

    [MaskSensitive]
    public string? PayeeName { get; set; }
    public string? PayeeAddress { get; set; }
    public string? PayeeCity { get; set; }
    public string? PayeeState { get; set; }
    public string? PayeePostalCode { get; set; }
    public string? ThirdPartyPayeeName { get; set; }
    public string? ThirdPartyPayeeAddress { get; set; }
    public string? ThirdPartyPayeeCity { get; set; }
    public string? ThirdPartyPayeeState { get; set; }
    public string? ThirdPartyPayeePostalCode { get; set; }
    public bool IsDesceased { get; set; }
    public string? ForTheBenefitOfPayee { get; set; }
    public string? ForTheBenefitOfAccountType { get; set; }
    public bool Tax1099ForEmployee { get; set; }
    public bool Tax1099ForBeneficiary { get; set; }

    public static DistributionRunReportDetail SampleResponse()
    {
        return new DistributionRunReportDetail()
        {
            DepartmentId = 1,
            BadgeNumber = 701001,
            DepartmentName = "Grocery",
            PayClassificationId = "1",
            PayClassificationName = "Manager",
            StoreNumber = 123,
            TaxCodeId = 'F',
            TaxCodeName = "Charitable gift annuity",
            EmployeeName = "Doe, John",
            EmploymentTypeId = 'F',
            EmploymentTypeName = "Full Time",
            HireDate = new DateOnly(2010, 5, 1),
            DateOfBirth = new DateOnly(1985, 3, 15),
            GrossAmount = 2000.00M,
            StateTaxAmount = 100.00M,
            FederalTaxAmount = 200.00M,
            CheckAmount = 1700.00M,
            IsDesceased = false
        };
    }
}
