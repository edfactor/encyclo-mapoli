using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/*
 * Compares two CSVs despite numeric fields having leading zeros, spaces around quoted string fields, and case insensitivity.
 */
internal static class TolerantCsvComparisonUtility
{
    internal static void ShouldBeTheSame(string csv1, string csv2)
    {
        using var reader1 = new StringReader(csv1);
        using var reader2 = new StringReader(csv2);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            BadDataFound = null // Ignore bad data, Ready's spaces after double quotes causes grief
        };

        using var csv1Reader = new CsvReader(reader1, csvConfig);
        using var csv2Reader = new CsvReader(reader2, csvConfig);

        // Read all records from both CSVs
        var records1 = csv1Reader.GetRecords<dynamic>().ToList();
        var records2 = csv2Reader.GetRecords<dynamic>().ToList();

        // Compare the counts first
        records1.Count.Should().Be(records2.Count, because: "the number of records in both CSVs should be equal");

        // Compare each record
        for (int i = 0; i < records1.Count; i++)
        {
            var record1 = records1[i];
            var record2 = records2[i];

            AreRecordsEqual(record1, record2, i + 1);
        }
    }

    private static void AreRecordsEqual(dynamic record1, dynamic record2, int lineNumber)
    {
        var dict1 = (IDictionary<string, object>)record1;
        var dict2 = (IDictionary<string, object>)record2;

        // Compare the number of fields
        dict1.Count.Should().Be(dict2.Count, because: $"the number of fields should be equal at line {lineNumber}");


        // Compare each field value
        foreach (var key in dict1.Keys)
        {
            // Ensure the column names are the same
            dict2.Should().ContainKey(key, because: $"at line {lineNumber}, the columns are different?");

            // Get the value from both records
            object value1 = dict1[key];
            object value2 = dict2[key];

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

                convertedValue1.Should().Be(convertedValue2, because: $"numeric values should match for column '{key}' at line {lineNumber}");
            }
            else
            {
                // Compare trimmed case-insensitive string values
                // Our status codes are lower case, ready uses upper case
                value1?.ToString()?.Trim().ToLowerInvariant().Should().Be(value2?.ToString()?.Trim().ToLowerInvariant(), because: $"string values should match for column '{key}' at line {lineNumber}");
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

