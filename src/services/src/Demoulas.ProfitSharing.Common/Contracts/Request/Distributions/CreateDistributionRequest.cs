namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
public record CreateDistributionRequest
{
    public int BadgeNumber { get; set; }
    public char StatusId { get; set; }
    public char FrequencyId { get; set; }
    public int? PayeeId { get; set; }
    public ThirdPartyPayee? ThirdPartyPayee { get; set; }
    public string? ForTheBenefitOfPayee { get; set; }
    public string? ForTheBenefitOfAccountType { get; set; }
    public bool Tax1099ForEmployee { get; set; }
    public bool Tax1099ForBeneficiary { get; set; }
    public required Decimal FederalTaxPercentage { get; set; }
    public required Decimal StateTaxPercentage { get; set; }
    public required Decimal GrossAmount { get; set; }
    public required Decimal FederalTaxAmount { get; set; }
    public required Decimal StateTaxAmount { get; set; }
    public required Decimal CheckAmount { get; set; }
    public required char TaxCodeId { get; set; }
    public bool IsDeceased { get; set; }
    public char? GenderId { get; set; }
    public bool IsQdro { get; set; }
    public string? Memo { get; set; }
    public bool IsRothIra { get; set; }

    public static CreateDistributionRequest RequestExample()
    {
        return new CreateDistributionRequest()
        {
            BadgeNumber = 12345,
            StatusId = 'A',
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1',
            Memo = "Distribution for June 2024"
        };
    }
}
