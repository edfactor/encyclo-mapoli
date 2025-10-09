using System.Text.Json.Serialization;


namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
public record AddressItem
{
    [JsonPropertyName("AddressLine1")]
    public required string AddressLine1 { get; init; }
    [JsonPropertyName("AddressLine2")]
    public string? AddressLine2 { get; init; }
    [JsonPropertyName("AddressLine3")]
    public string? AddressLine3 { get; init; }
    [JsonPropertyName("AddressLine4")]
    public string? AddressLine4 { get; init; }
    [JsonPropertyName("TownOrCity")]
    public required string TownOrCity { get; init; }

    [JsonPropertyName("Region2")]
    public required string State { get; init; }

    [JsonPropertyName("Country")]
    public string? Country { get; init; }

    [JsonPropertyName("PostalCode")]
    public required string PostalCode { get; init; }

    public bool? PrimaryFlag { get; init; }
}

