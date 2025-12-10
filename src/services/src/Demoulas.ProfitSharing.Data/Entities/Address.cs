namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Address
{
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryIso { get; set; } = Country.Constants.Us;
}
