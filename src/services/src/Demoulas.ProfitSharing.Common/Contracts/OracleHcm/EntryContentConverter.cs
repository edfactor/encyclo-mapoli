using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class EntryContentConverter<TContext> : JsonConverter<EntryContent<TContext>> where TContext : DeltaContextBase
{
    public override EntryContent<TContext>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Check if the token is a string (raw JSON in a string)
        if (reader.TokenType == JsonTokenType.String)
        {
            // Extract the string and parse it as JSON
            var rawString = reader.GetString();
            if (string.IsNullOrWhiteSpace(rawString))
            {
                return new EntryContent<TContext>();
            }

            using var document = JsonDocument.Parse(rawString);
            var rootElement = document.RootElement;

            // Process as if it's the standard JSON structure
            return ParseEntryContent(rootElement, options);
        }

        // Otherwise, handle it as a direct JSON object
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return ParseEntryContent(document.RootElement, options);
        }

        throw new JsonException($"Unexpected token parsing {typeof(EntryContent<TContext>)}. Expected String or StartObject.");
    }

    private EntryContent<TContext> ParseEntryContent(JsonElement rootElement, JsonSerializerOptions options)
    {
        // Extract the "Context" property
        if (rootElement.TryGetProperty("Context", out var contextElement) && contextElement.ValueKind == JsonValueKind.Array)
        {
            var contextList = JsonSerializer.Deserialize<List<TContext>>(contextElement.GetRawText(), options);

            // Handle optional "Changed Attributes" property
            var changedAttributes = rootElement.TryGetProperty("Changed Attributes", out var changedAttributesElement) &&
                                    changedAttributesElement.ValueKind == JsonValueKind.Array
                ? JsonSerializer.Deserialize<List<ChangedAttribute>>(changedAttributesElement.GetRawText(), options)
                : [];

            return new EntryContent<TContext>
            {
                Context = contextList ?? [],
                ChangedAttributes = changedAttributes
            };
        }

        throw new JsonException($"Missing or invalid 'Context' property in {typeof(EntryContent<TContext>)}.");
    }

    public override void Write(Utf8JsonWriter writer, EntryContent<TContext> value, JsonSerializerOptions options)
    {
        // we will never write. This is a read-only operation
    }
}
