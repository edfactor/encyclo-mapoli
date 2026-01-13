namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

public sealed record UpdateDistributionRequest : CreateDistributionRequest
{
    public long Id { get; set; }

    public static new UpdateDistributionRequest RequestExample()
    {
        return new UpdateDistributionRequest()
        {
            Id = 1,
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
            Memo = "Updated distribution for June 2024"
        };
    }
}
