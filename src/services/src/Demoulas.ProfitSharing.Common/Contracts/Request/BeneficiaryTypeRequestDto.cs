namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record BeneficiaryTypeRequestDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }
}
