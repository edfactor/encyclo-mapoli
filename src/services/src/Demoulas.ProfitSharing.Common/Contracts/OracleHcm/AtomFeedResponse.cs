using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class AtomFeedResponse<TContext> where TContext : IDeltaContext
{
    public required Feed<TContext> Feed { get; set; }
}

public class Feed<TContext> where TContext : IDeltaContext
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public DateTime Updated { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();
    public List<Entry<TContext>> Entries { get; set; } = new List<Entry<TContext>>();
    public List<Link> Links { get; set; } = new List<Link>();
}

public class Author
{
    public required string Name { get; set; }
}

public class Entry<TContext> where TContext : IDeltaContext
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    [JsonConverter(typeof(JsonStringToObjectConverterFactory<TContext>))]
    public required EntryContent<TContext> Content { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Published { get; set; }
    public List<DeltaLink> Links { get; set; } = new List<DeltaLink>();
    public List<Author> Authors { get; set; } = new List<Author>();
}

public class JsonStringToObjectConverterFactory<TContext> : JsonConverterFactory where TContext : IDeltaContext
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Only allows conversions for types implementing IDeltaContext
        return typeof(TContext).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Create and return the generic JsonStringToObjectConverter for the given type
        var converterType = typeof(JsonStringToObjectConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

public class EntryContent<TContext> where TContext : IDeltaContext
{
    public List<TContext> Context { get; set; } = new List<TContext>();
}

public interface IDeltaContext
{
    public long PersonId { get; set; }
}

public class NewHireContext : IDeltaContext
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

public class AssignmentContext : IDeltaContext
{
    public required long PersonId { get; set; }
}

public class EmployeeUpdateContext : IDeltaContext
{
    public required long PersonId { get; set; }
}

public class TerminationContext : IDeltaContext
{
    public required long PersonId { get; set; }
}

public class DeltaLink
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
