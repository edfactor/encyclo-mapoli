namespace Demoulas.ProfitSharing.Data.Entities;
public class Country
{
    public const string US = "US";
    public const string Canada = "CA";

    public required short Id { get; init; }
    public required string Name { get; init; }
    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
