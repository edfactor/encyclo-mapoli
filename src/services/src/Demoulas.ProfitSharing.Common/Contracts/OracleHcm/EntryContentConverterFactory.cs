using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

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
