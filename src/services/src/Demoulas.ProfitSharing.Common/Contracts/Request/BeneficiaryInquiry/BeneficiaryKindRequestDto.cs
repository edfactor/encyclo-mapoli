namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;

public class BeneficiaryKindRequestDto
{
    public char? Id { get; set; }

    public static BeneficiaryKindRequestDto RequestExample()
    {
        return new BeneficiaryKindRequestDto
        {
            Id = 'S'
        };
    }
}
