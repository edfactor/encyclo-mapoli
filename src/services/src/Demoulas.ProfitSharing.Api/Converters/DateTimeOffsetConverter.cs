using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Api.Converters;

public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTimeOffset));
        _ = DateTimeOffset.TryParse(reader.GetString(), out DateTimeOffset date);

        return date;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
        writer.WriteStringValue(value.ToUniversalTime().ToString("O"));
    }
}
