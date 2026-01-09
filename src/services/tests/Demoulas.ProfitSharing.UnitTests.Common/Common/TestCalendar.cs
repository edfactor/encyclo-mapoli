using System.Reflection;
using System.Text.Json;

namespace Demoulas.ProfitSharing.UnitTests.Common.Common;

/// <summary>
/// Helper for accessing calendar data in tests.
/// Use <see cref="CurrentYear"/> instead of DateTime.Now.Year to avoid test failures at year boundaries.
/// </summary>
public static class TestCalendar
{
    /// <summary>
    /// Gets the maximum calendar year available in the seeded calendar data.
    /// </summary>
    public static short CurrentYear { get; } = GetMaxYearFromCalendarData();

    private static short GetMaxYearFromCalendarData()
    {
        try
        {
            var assembly = Assembly.Load("Demoulas.Common.Data.Services");
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("CALDAR_RECORD.json"));

            if (string.IsNullOrWhiteSpace(resourceName))
                return (short)DateTime.Now.Year;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return (short)DateTime.Now.Year;

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            using var doc = JsonDocument.Parse(json);
            var maxYear = 0;

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                // Try ACC_WKEND2_N first (format: YYYYMMDD as integer)
                if (element.TryGetProperty("ACC_WKEND2_N", out var wkend2) && wkend2.TryGetInt32(out var dateInt))
                {
                    var year = dateInt / 10000;
                    if (year > maxYear) maxYear = year;
                }
                // Fallback: try WeekendingDate as string (ISO format)
                else if (element.TryGetProperty("WeekendingDate", out var wkendDate)
                         && wkendDate.ValueKind == JsonValueKind.String
                         && DateOnly.TryParse(wkendDate.GetString(), System.Globalization.CultureInfo.InvariantCulture, out var date)
                         && date.Year > maxYear)
                {
                    maxYear = date.Year;
                }
            }

            return maxYear > 0 ? (short)maxYear : (short)DateTime.Now.Year;
        }
        catch
        {
            // Fallback if anything goes wrong
            return (short)DateTime.Now.Year;
        }
    }
}
