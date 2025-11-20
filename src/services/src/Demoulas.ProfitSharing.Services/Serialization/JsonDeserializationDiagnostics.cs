using System.Diagnostics;
using System.Text.Json;

namespace Demoulas.ProfitSharing.Services.Serialization;

/// <summary>
/// Diagnostic helper to capture and validate JSON before deserialization.
/// Use this to debug "too large or too small for a Decimal" errors.
/// </summary>
public static class JsonDeserializationDiagnostics
{
    /// <summary>
    /// Validates a JSON string and reports all numeric values to help identify decimal overflow issues.
    /// Logs output to Debug.WriteLine.
    /// </summary>
    public static void ValidateJsonNumericValues(string jsonString)
    {
        Debug.WriteLine($"=== JSON Validation ===");
        Debug.WriteLine($"Total length: {jsonString.Length} characters");

        try
        {
            using var doc = JsonDocument.Parse(jsonString);
            Debug.WriteLine("\n=== All Numeric Values ===");
            FindAndValidateNumericValues(doc.RootElement, "");
        }
        catch (JsonException parseEx)
        {
            Debug.WriteLine($"Parse Error: {parseEx.Message}");
            Debug.WriteLine($"Line {parseEx.LineNumber}, Column {parseEx.BytePositionInLine}");
            
            // Show context around error
            int errorPos = Math.Min((int)(parseEx.BytePositionInLine ?? 0) - 1, jsonString.Length - 1);
            int start = Math.Max(0, errorPos - 50);
            int end = Math.Min(jsonString.Length, errorPos + 50);
            string context = jsonString.Substring(start, end - start);
            
            Debug.WriteLine($"Context around error: ...{context}...");
        }
    }

    private static void FindAndValidateNumericValues(JsonElement element, string path)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                string propPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                ValidateElement(prop.Value, propPath);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                ValidateElement(item, $"{path}[{index}]");
                index++;
            }
        }
    }

    private static void ValidateElement(JsonElement element, string path)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            string rawValue = element.GetRawText();
            bool canParseDecimal = decimal.TryParse(rawValue, 
                System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowLeadingSign,
                System.Globalization.CultureInfo.InvariantCulture, 
                out var decimalValue);

            if (canParseDecimal)
            {
                Debug.WriteLine($"  ✓ {path}: {rawValue} (decimal: {decimalValue})");
            }
            else
            {
                Debug.WriteLine($"  ✗ {path}: {rawValue}");
                Debug.WriteLine($"    ^^^ CANNOT PARSE AS DECIMAL ^^^");
                
                // Try to give more info
                if (double.TryParse(rawValue, out var dblVal))
                {
                    Debug.WriteLine($"    Parsed as double: {dblVal}");
                    if (Math.Abs(dblVal) > (double)decimal.MaxValue)
                    {
                        Debug.WriteLine($"    This value EXCEEDS Decimal.MaxValue ({decimal.MaxValue})");
                    }
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                ValidateElement(prop.Value, $"{path}.{prop.Name}");
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                ValidateElement(item, $"{path}[{index}]");
                index++;
            }
        }
    }
}
