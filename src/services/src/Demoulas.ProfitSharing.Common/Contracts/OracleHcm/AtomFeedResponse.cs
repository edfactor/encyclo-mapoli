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

    [JsonConverter(typeof(EntryContentConverterFactory))]
    public required EntryContent<TContext> Content { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Published { get; set; }
    public List<DeltaLink> Links { get; set; } = new List<DeltaLink>();
    public List<Author> Authors { get; set; } = new List<Author>();
}

public class EntryContent<TContext> where TContext : IDeltaContext
{
    public List<TContext> Context { get; set; } = new List<TContext>();

    [JsonPropertyName("Changed Attributes")]
    public List<ChangedAttribute>? ChangedAttributes { get; set; } = new List<ChangedAttribute>();
}

public interface IDeltaContext
{
    public long PersonId { get; set; }
}

public class NewHireContext : IDeltaContext
{
    public long? PeriodOfServiceId { get; set; }
    
    public required long PersonId { get; set; }

    public string? PersonName { get; set; }
    public int? PersonNumber { get; set; }
    public string? WorkEmail { get; set; }
    public string? PrimaryPhoneNumber { get; set; }

    public string? PeriodType { get; set; }

    public string? WorkerType { get; set; }

    public string? DMLOperation { get; set; }

    public DateOnly? EffectiveStartDate { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}

public record ChangedAttribute
{
    public string? SalaryBasisId { get; set; }
    public long? NationalIdentifierId { get; set; }
    public string? LegislationCode { get; set; }
    public string? NationalIdentifierType { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}


public class AssignmentContext : IDeltaContext
{
    public long SalaryId { get; set; }
    public string? PersonName { get; set; }
    public int? PersonNumber { get; set; }
    public required long PersonId { get; set; }

    public string? WorkEmail { get; set; }
    public string? PrimaryPhoneNumber { get; set; }

    public string? PeriodType { get; set; }

    public string? WorkerType { get; set; }

    public string? DMLOperation { get; set; }

    public long? AssignmentId { get; set; }

    public DateOnly? EffectiveDate { get; set; }
}

public class EmployeeUpdateContext : IDeltaContext
{
    public required long PersonId { get; set; }
    public long? NationalIdentifierId { get; set; }
    public string? PersonName { get; set; }
    public int? PersonNumber { get; set; }
    public string? PrimaryPhoneNumber { get; set; }

    public string? PeriodType { get; set; }

    public string? WorkerType { get; set; }

    public string? DMLOperation { get; set; }

    public DateOnly? EffectiveStartDate { get; set; }
    public DateOnly? EffectiveDate { get; set; }
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

public class EntryContentConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(EntryContent<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var contextType = typeToConvert.GetGenericArguments()[0]; // Get the TContext type
        var converterType = typeof(EntryContentConverter<>).MakeGenericType(contextType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}


public class EntryContentConverter<TContext> : JsonConverter<EntryContent<TContext>> where TContext : IDeltaContext
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
                : new List<ChangedAttribute>();

            return new EntryContent<TContext>
            {
                Context = contextList ?? new List<TContext>(),
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



