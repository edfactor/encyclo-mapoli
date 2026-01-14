using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

public sealed record BeneficiaryDisbursementRequest : IBadgeNumberRequest
{
    public required int BadgeNumber { get; init; }
    public short? PsnSuffix { get; init; }
    public bool IsDeceased { get; init; }
    public required List<RecipientBeneficiary> Beneficiaries { get; init; }

    public static BeneficiaryDisbursementRequest SampleRequest()
    {
        return new BeneficiaryDisbursementRequest
        {
            BadgeNumber = 700024,
            IsDeceased = true,
            Beneficiaries = new List<RecipientBeneficiary>
            {
                new RecipientBeneficiary { PsnSuffix = 1, Percentage = 50.0m },
                new RecipientBeneficiary { PsnSuffix = 2, Percentage = 50.0m }
            }
        };
    }
}
