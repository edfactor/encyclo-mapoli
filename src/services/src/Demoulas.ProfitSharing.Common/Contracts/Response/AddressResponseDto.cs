using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record AddressResponseDto
{
    [MaskSensitive] public required string Street { get; init; }
    public string? Street2 { get; init; }
    [MaskSensitive] public required string? City { get; init; }
    public required string? State { get; init; }
    public required string? PostalCode { get; init; }
    public required string CountryIso { get; init; }


    public static AddressResponseDto ResponseExample()
    {
        return new AddressResponseDto
        {
            State = "MA",
            PostalCode = "01876",
            City = "Tewksbury",
            Street = "1900 Main St",
            CountryIso = "US"
        };
    }
}
