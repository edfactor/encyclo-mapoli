namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record AddressResponseDto
{
    public required string Street { get; init; }
    public string? Street2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string PostalCode { get; init; }
    public required string CountryISO { get; init; }
}
