namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

public sealed record RecipientBeneficiary
{
    public short PsnSuffix { get; init; }
    public decimal? Percentage { get; init; }
    public decimal? Amount { get; init; }

    public static RecipientBeneficiary RequestExample()
    {
        return new RecipientBeneficiary
        {
            PsnSuffix = 1,
            Percentage = 50.00m,
            Amount = 5000.00m
        };
    }
}
