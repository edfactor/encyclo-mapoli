using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Api.Converters;

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        _ = DateTime.TryParse(reader.GetString(), out DateTime date);

        return date;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
        writer.WriteStringValue(value.ToUniversalTime().ToString("O"));
    }
}
