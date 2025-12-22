using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record AnnuityRateDto
{
    public required short Year { get; init; }

    public required byte Age { get; init; }

    [UnmaskSensitive] public required decimal SingleRate { get; init; }

    [UnmaskSensitive] public required decimal JointRate { get; init; }

    public static AnnuityRateDto ResponseExample()
    {
        return new AnnuityRateDto
        {
            Year = 2024,
            Age = 65,
            SingleRate = 0.0525m,
            JointRate = 0.0475m
        };
    }
}
