namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
public sealed record CreateBeneficiaryResponse
{
    public int BeneficiaryId { get; set; }
    public short PsnSuffix { get; set; }
    public int EmployeeBadgeNumber { get; set; }
    public int DemographicId { get; set; }
    public int BeneficiaryContactId { get; set; }
    public string? Relationship { get; set; }
    public char? KindId { get; set; }
    public required decimal Percent { get; set; }
    public static CreateBeneficiaryResponse SampleResponse() => new CreateBeneficiaryResponse
    {
        BeneficiaryId = 20015,
        PsnSuffix = 1000,
        EmployeeBadgeNumber = 123456,
        DemographicId = 3001,
        BeneficiaryContactId = 4002,
        Relationship = "Sibling",
        KindId = 'P',
        Percent = 50.0m
    };    
}
