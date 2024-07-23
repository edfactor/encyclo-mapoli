using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Converter;

public sealed class SingleItemConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            var root = document.RootElement;
            if (root.TryGetProperty("items", out JsonElement itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itemsElement.EnumerateArray())
                {
                    return JsonSerializer.Deserialize<T>(item.GetRawText());
                }
            }
        }

        return default(T);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("This converter is only for reading JSON.");
    }
}
