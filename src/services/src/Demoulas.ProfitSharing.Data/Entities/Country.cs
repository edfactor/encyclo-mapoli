namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Country
{
    public required short Id { get; init; }
    public required string Name { get; init; }
    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
