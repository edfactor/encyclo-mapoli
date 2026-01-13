namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record BeneficiaryTypeResponseDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }

    public static BeneficiaryTypeResponseDto ResponseExample()
    {
        return new BeneficiaryTypeResponseDto
        {
            Id = 1,
            Name = "Primary"
        };
    }
}
