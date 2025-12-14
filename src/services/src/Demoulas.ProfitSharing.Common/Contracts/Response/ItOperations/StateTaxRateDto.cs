namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record StateTaxRateDto
{
    public required string Abbreviation { get; init; }

    public required decimal Rate { get; init; }

    public DateOnly? DateModified { get; init; }

    public string? UserModified { get; init; }
}
