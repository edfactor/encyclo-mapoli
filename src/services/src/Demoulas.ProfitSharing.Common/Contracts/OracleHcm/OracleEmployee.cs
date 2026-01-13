using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record OracleEmployee(
    [property: JsonPropertyName("PersonId")] long PersonId,
    [property: JsonPropertyName("PersonNumber")] int BadgeNumber,
    [property: JsonPropertyName("DateOfBirth")] DateOnly DateOfBirth,
    [property: JsonPropertyName("DateOfDeath")] DateOnly? DateOfDeath,
    [property: JsonPropertyName("CreatedBy")] string CreatedBy,
    [property: JsonPropertyName("CreationDate")] DateTimeOffset CreationDate,
    [property: JsonPropertyName("LastUpdatedBy")] string LastUpdatedBy,
    [property: JsonPropertyName("LastUpdateDate")] DateTimeOffset? LastUpdateDate,
    [property: JsonPropertyName("addresses")] Addresses Addresses,
    [property: JsonPropertyName("emails")] Emails Emails,
    [property: JsonPropertyName("names")] Names Names,
    [property: JsonPropertyName("phones")] Phones? Phones,
    [property: JsonPropertyName("workRelationships")] WorkRelationships? WorkRelationships,
    [property: JsonPropertyName("nationalIdentifiers")] NationalIdentifiers? NationalIdentifiers,
    [property: JsonPropertyName("legislativeInfo")] LegislativeInfo? LegislativeInfo,
    [property: JsonPropertyName("@context")]
    Context Context
)
{
    [JsonIgnore]
    public AddressItem? Address => Addresses.Items.FirstOrDefault(a => a.PrimaryFlag ?? false);

    [JsonIgnore]
    public EmailItem? Email => Emails.Items.FirstOrDefault(a => a.PrimaryFlag ?? false);

    [JsonIgnore]
    public NameItem Name => Names.Items[0];

    [JsonIgnore]
    public PhoneItem? Phone => Phones?.Items.FirstOrDefault(a => a.PrimaryFlag ?? false);

    [JsonIgnore]
    public NationalIdentifier? NationalIdentifier => NationalIdentifiers?.Items.FirstOrDefault(a => a.PrimaryFlag ?? false);

    [JsonIgnore]
    public WorkRelationship? WorkRelationship => WorkRelationships?.Items.FirstOrDefault(a => a.PrimaryFlag ?? false);

    [JsonIgnore]
    public LegislativeInfoItem? LegislativeInfoItem => LegislativeInfo?.Items.FirstOrDefault();
}



