namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record UpdateBeneficiaryRequest : UpdateBeneficiaryContactRequest
{
    public char? KindId { get; set; }
    public string? Relationship { get; set; }
    public decimal? Percentage { get; set; }

    public static new UpdateBeneficiaryRequest SampleRequest()
    {
        return new UpdateBeneficiaryRequest()
        {
            Id = 1,
            KindId = 'P',
            Relationship = "Cousin",
            Percentage = 100m
        };
    }
}
