namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record CreateBeneficiaryRequest
{
    public int BeneficiaryContactId { get; set; }
    public int EmployeeBadgeNumber { get; set; }
    public byte? FirstLevelBeneficiaryNumber { get; set; }
    public byte? SecondLevelBeneficiaryNumber { get; set; }
    public byte? ThirdLevelBeneficiaryNumber { get; set; }
    public required string Relationship { get; set; }
    public required char KindId { get; set; }
    public decimal Percentage { get; set; }

    public static CreateBeneficiaryRequest SampleRequest() => new CreateBeneficiaryRequest
    {
        BeneficiaryContactId = 1,
        FirstLevelBeneficiaryNumber = 1,
        SecondLevelBeneficiaryNumber = 2,
        ThirdLevelBeneficiaryNumber = 3,
        Relationship = "Sibling",
        KindId = 'P',
        Percentage = 50
    };
}
