using Demoulas.ProfitSharing.Common;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Address
{
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string CountryISO { get; set; } = Constants.US;
}
