using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/*
 * Compares two CSVs despite numeric fields having leading zeros, spaces around quoted string fields, and case insensitivity.
 */
internal static class TolerantCsvComparisonUtility
{
    internal static void ShouldBeTheSame(string actualCsvContents, string expectedCsvContents)
    {
        // Read all records from both CSVs
        var actualRecords = GetRecords(actualCsvContents);
        var expectedRecords = GetRecords(expectedCsvContents);

        // Compare the counts first
        actualRecords.Count.ShouldBe(expectedRecords.Count,
            customMessage:
            $"the number of records in both CSVs should be equal\r\nActual CSV\r\n{actualCsvContents}\r\n\r\nExpected Csv Contents\r\n{expectedCsvContents}");

        // Compare each record
        for (int i = 0; i < actualRecords.Count; i++)
        {
            var actual = actualRecords[i];
            var expected = expectedRecords[i];

            AreRecordsEqual(actual, expected, i + 1);
        }
    }

    private static List<dynamic> GetRecords(string csvContents)
    {
        using var stringReader = new StringReader(csvContents);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            BadDataFound = null // Ignore bad data, Ready's spaces after double quotes causes grief
        };
        using var csvReader = new CsvReader(stringReader, csvConfig);
        return csvReader.GetRecords<dynamic>().ToList();
    }

    private static void AreRecordsEqual(dynamic actualRecords, dynamic expectedRecords, int lineNumber)
    {
        var actual = (IDictionary<string, object>)actualRecords;
        var expected = (IDictionary<string, object>)expectedRecords;

        // Compare the number of fields
        actual.Count.ShouldBe(expected.Count, customMessage: $"the number of fields should be equal at line {lineNumber}");

        // Compare each field value
        foreach (var key in actual.Keys)
        {
            // Ensure the column names are the same
            expected.ShouldContainKey(key, customMessage: $"at line {lineNumber}, the columns are different?");

            // Get the value from both records
            object value1 = actual[key];
            object value2 = expected[key];

            // Handle null values if needed
            if (value1 == null && value2 == null)
            {
                continue; // Both values are null, consider equal
            }

            // We handle numbers specially.   Ready generates "004", Smart just has "4" - no leading zeros.
            if (IsNumeric(value1!) && IsNumeric(value2!))
            {
                double convertedValue1 = Convert.ToDouble(value1);
                double convertedValue2 = Convert.ToDouble(value2);

                convertedValue1.ShouldBe(convertedValue2, customMessage: $"numeric values should match for column '{key}' at line {lineNumber}");
            }
            else
            {
                // Compare trimmed case-insensitive string values
                // Our status codes are lower case, ready uses upper case
                value1?.ToString()?.Trim().ToLowerInvariant().ShouldBe(value2?.ToString()?.Trim().ToLowerInvariant(),
                    customMessage: $"string values should match for column '{key}' at line {lineNumber}");
            }
        }
    }

    private static bool IsNumeric(object value)
    {
        if (value == null)
        {
            return false;
        }

        // Check if the value is a number
        return double.TryParse(value.ToString(), out _);
    }
}
