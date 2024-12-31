using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class AtomFeedResponse
{
    public required Feed Feed { get; set; }
}

public class Feed
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public DateTime Updated { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();
    public List<Entry> Entries { get; set; } = new List<Entry>();
    public List<Link> Links { get; set; } = new List<Link>();
}

public class Author
{
    public required string Name { get; set; }
}

public class Entry
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    [JsonConverter(typeof(JsonStringToObjectConverter<EntryContent>))]
    public required EntryContent Content { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Published { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
    public List<Author> Authors { get; set; } = new List<Author>();
}

public class EntryContent
{
    public List<Context> Context { get; set; } = new List<Context>();
}

public class Context
{
    public long PeriodOfServiceId { get; set; }
    public required long PersonId { get; set; }

    public string? PersonName { get; set; }
    public string? PersonNumber { get; set; }
    public string? WorkEmail { get; set; }
    public string? PrimaryPhoneNumber { get; set; }

    public string? PeriodType { get; set; }
    public string? WorkerType { get; set; }
    public string? DMLOperation { get; set; }
    public DateTime EffectiveStartDate { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public class Link
{
    public string? Href { get; set; }
    public string? Rel { get; set; }
    public string? RelType { get; set; }
}

public class JsonStringToObjectConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        // Ensure the JSON value is a string
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}.");
        }

        // Get the raw string value and deserialize it
        string? rawString = reader.GetString();
        if (string.IsNullOrWhiteSpace(rawString))
        {
            return default(T)!; // or throw new JsonException()
        }

        return JsonSerializer.Deserialize<T>(rawString, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Serialize the object as a JSON string
        string rawString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(rawString);
    }
}
