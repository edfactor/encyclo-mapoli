using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

[NoMemberDataExposed]
public sealed record CreateOrUpdateDistributionResponse : CreateDistributionRequest
{
    public long Id { get; set; }
    public required string MaskSsn { get; set; }
    public byte PaymentSequence { get; set; }
    public DateTime CreatedAt { get; set; }

    public static CreateOrUpdateDistributionResponse ResponseExample()
    {
        return new CreateOrUpdateDistributionResponse()
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
            MaskSsn = "XXX-XX-6789",
            PaymentSequence = 1,
            CreatedAt = DateTime.UtcNow,
            Memo = "Distribution for June 2024"
        };
    }
}
