using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record OracleEmployee(
    [property: JsonPropertyName("PersonId")] long PersonId,
    [property: JsonPropertyName("PersonNumber")] int BadgeNumber,
    [property: JsonPropertyName("DateOfBirth")] DateOnly DateOfBirth,
    [property: JsonPropertyName("DateOfDeath")] DateOnly? DateOfDeath,
    [property: JsonPropertyName("CreatedBy")] string CreatedBy,
    [property: JsonPropertyName("CreationDate")] DateTimeOffset CreationDate,
    [property: JsonPropertyName("LastUpdatedBy")] string LastUpdatedBy,
    [property: JsonPropertyName("LastUpdateDate")] DateTimeOffset? LastUpdateDate,
    [property: JsonPropertyName("addresses")] List<AddressItem> Addresses,
    [property: JsonPropertyName("emails")] List<EmailItem> Emails,
    [property: JsonPropertyName("names")] List<NameItem> Names,
    [property: JsonPropertyName("phones")] List<PhoneItem> Phones,
    [property: JsonPropertyName("@context")]
    Context Context
)
{
    public AddressItem? Address => Addresses.FirstOrDefault(a => a.PrimaryFlag ?? false);
    public EmailItem? Email => Emails.FirstOrDefault();
    public NameItem Name => Names.First();
    public PhoneItem? Phone => Phones.FirstOrDefault(a => a.PrimaryFlag ?? false);

}



