using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record AnnuityRateDto
{
    public required short Year { get; init; }

    public required byte Age { get; init; }

    [UnmaskSensitive] public required decimal SingleRate { get; init; }

    [UnmaskSensitive] public required decimal JointRate { get; init; }
}
