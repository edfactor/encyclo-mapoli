namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

public record BeneficiaryTypeDto
{
    public byte Id { get; set; }
    public required string Name { get; set; }

    public static BeneficiaryTypeDto ResponseExample() => new()
    {
        Id = 1,
        Name = "Primary"
    };
}
