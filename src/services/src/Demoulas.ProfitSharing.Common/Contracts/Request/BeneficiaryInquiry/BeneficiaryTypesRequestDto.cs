namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;

public record BeneficiaryTypesRequestDto
{
    public byte? Id { get; set; }

    public static BeneficiaryTypesRequestDto RequestExample()
    {
        return new BeneficiaryTypesRequestDto
        {
            Id = 1
        };
    }
}
