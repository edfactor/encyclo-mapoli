using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record NationalIdentifiers(
    [property: JsonPropertyName("items")] IReadOnlyList<NationalIdentifier> Items,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset
);
