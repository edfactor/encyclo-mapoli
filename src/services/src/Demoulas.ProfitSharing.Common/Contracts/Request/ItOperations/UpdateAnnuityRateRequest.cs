namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

public sealed record UpdateAnnuityRateRequest
{
    public required short Year { get; init; }

    public required byte Age { get; init; }

    public required decimal SingleRate { get; init; }

    public required decimal JointRate { get; init; }

    public static UpdateAnnuityRateRequest RequestExample()
    {
        return new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 0.85m,
            JointRate = 0.75m
        };
    }
}
