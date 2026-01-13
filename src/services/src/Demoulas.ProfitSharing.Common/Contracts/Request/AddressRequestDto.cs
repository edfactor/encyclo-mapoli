
namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record AddressRequestDto
{
    public required string Street { get; init; }
    public string? Street2 { get; init; }
    public string? Street3 { get; init; }
    public string? Street4 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public string? CountryIso { get; init; }

    public static AddressRequestDto RequestExample() => new()
    {
        Street = "123 Main St",
        Street2 = "Apt 4B",
        City = "Springfield",
        State = "MA",
        PostalCode = "01101",
        CountryIso = "US"
    };
}
