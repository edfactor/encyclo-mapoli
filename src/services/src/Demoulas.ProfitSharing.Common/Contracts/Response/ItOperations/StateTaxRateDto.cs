using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record StateTaxRateDto
{
    public required string Abbreviation { get; init; }

    [UnmaskSensitive]
    public required decimal Rate { get; init; }

    public DateOnly? DateModified { get; init; }

    public string? UserModified { get; init; }

    public static StateTaxRateDto ResponseExample()
    {
        return new StateTaxRateDto
        {
            Abbreviation = "MA",
            Rate = 0.05m,
            DateModified = DateOnly.FromDateTime(DateTime.Today),
            UserModified = "admin_user"
        };
    }
}
