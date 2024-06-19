using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Api.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateOnly));
        _ = DateOnly.TryParse(reader.GetString(), out DateOnly date);

        return date;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
        writer.WriteStringValue(value.ToString("O"));
    }
}
