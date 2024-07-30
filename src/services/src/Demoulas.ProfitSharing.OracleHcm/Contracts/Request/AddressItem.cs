using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record AddressItem(
    [property: JsonPropertyName("AddressId")] string AddressId,
    [property: JsonPropertyName("EffectiveStartDate")] string EffectiveStartDate,
    [property: JsonPropertyName("EffectiveEndDate")] string EffectiveEndDate,
    [property: JsonPropertyName("AddressLine1")] string AddressLine1,
    [property: JsonPropertyName("AddressLine2")] string AddressLine2,
    [property: JsonPropertyName("AddressLine3")] string AddressLine3,
    [property: JsonPropertyName("AddressLine4")] string AddressLine4,
    [property: JsonPropertyName("TownOrCity")] string TownOrCity,
    [property: JsonPropertyName("Region1")] string County,
    [property: JsonPropertyName("Region2")] string State,
    [property: JsonPropertyName("Country")] string Country,
    [property: JsonPropertyName("CountryName")] string CountryName,
    [property: JsonPropertyName("PostalCode")] string PostalCode,
    [property: JsonPropertyName("LongPostalCode")] object LongPostalCode,
    [property: JsonPropertyName("Building")] object Building,
    [property: JsonPropertyName("FloorNumber")] object FloorNumber,
    [property: JsonPropertyName("CreatedBy")] string CreatedBy,
    [property: JsonPropertyName("CreationDate")] DateTime? CreationDate,
    [property: JsonPropertyName("LastUpdatedBy")] string LastUpdatedBy,
    [property: JsonPropertyName("PersonAddrUsageId")] string PersonAddrUsageId,
    [property: JsonPropertyName("AddressType")] string AddressType,
    [property: JsonPropertyName("AddressTypeMeaning")] string AddressTypeMeaning,
    [property: JsonPropertyName("PrimaryFlag")] bool? PrimaryFlag
);
