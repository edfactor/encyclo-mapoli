namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record UpdateBeneficiaryRequest : UpdateBeneficiaryContactRequest
{
    public string? Relationship { get; set; }
    public decimal? Percentage { get; set; }

    public static new UpdateBeneficiaryRequest SampleRequest()
    {
        return new UpdateBeneficiaryRequest()
        {
            Id = 1,
            Relationship = "Cousin",
            Percentage = 100m
        };
    }
}
