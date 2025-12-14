using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record StateTaxRateDto
{
    public required string Abbreviation { get; init; }

    [UnmaskSensitive]
    public required decimal Rate { get; init; }

    public DateOnly? DateModified { get; init; }

    public string? UserModified { get; init; }
}
