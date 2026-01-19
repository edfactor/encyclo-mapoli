namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

public sealed record CreateAnnuityRatesRequest
{
    public required short Year { get; init; }

    public required IReadOnlyList<AnnuityRateInputRequest> Rates { get; init; }

    public static CreateAnnuityRatesRequest RequestExample()
    {
        return new CreateAnnuityRatesRequest
        {
            Year = 2025,
            Rates = new List<AnnuityRateInputRequest>
            {
                new()
                {
                    Age = 67,
                    SingleRate = 12.9100m,
                    JointRate = 15.1876m
                }
            }
        };
    }
}
