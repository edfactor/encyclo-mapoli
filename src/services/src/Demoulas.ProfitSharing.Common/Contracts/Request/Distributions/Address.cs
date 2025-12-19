using Demoulas.ProfitSharing.Common.Contracts.Shared;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

public sealed record Address : ICity
{
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryIso { get; set; } = "US";

    public static Address RequestExample()
    {        return new Address
        {
            Street = "123 Main Street",
            Street2 = "Apt 4B",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01876",
            CountryIso = "US"
        };
    }
}
