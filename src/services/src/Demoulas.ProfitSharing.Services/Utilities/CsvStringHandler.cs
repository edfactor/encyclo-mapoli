using System.Globalization;
using System.Runtime.CompilerServices;

namespace Demoulas.ProfitSharing.Services.Utilities;

/// <summary>
/// Custom interpolated string handler for CSV generation that minimizes allocations.
/// Uses DefaultInterpolatedStringHandler internally but adds CSV-specific optimizations.
/// </summary>
/// <remarks>
/// This handler is designed for CSV string building where:
/// - Fields are separated by semicolons
/// - Null values should be replaced with empty strings
/// - Numeric values use InvariantCulture formatting
/// - Memory allocations are minimized
/// 
/// Usage:
/// <code>
/// var csv = new CsvStringHandler(5); // 5 fields
/// csv.AppendField(name);
/// csv.AppendField(address);
/// csv.AppendField(city);
/// csv.AppendField(state);
/// csv.AppendField(postalCode);
/// return csv.ToString();
/// </code>
/// </remarks>
[InterpolatedStringHandler]
public ref struct CsvStringHandler
{
    private DefaultInterpolatedStringHandler _handler;
    private int _fieldCount;
    private int _currentField;

    /// <summary>
    /// Initializes a new CSV string handler with estimated capacity.
    /// </summary>
    /// <param name="literalLength">Estimated total literal string length</param>
    /// <param name="formattedCount">Number of formatted values</param>
    /// <param name="fieldCount">Expected number of CSV fields (for separator calculation)</param>
    public CsvStringHandler(int literalLength, int formattedCount, int fieldCount = 0)
    {
        // Account for semicolons between fields
        var separatorLength = fieldCount > 0 ? fieldCount - 1 : 0;
        _handler = new DefaultInterpolatedStringHandler(literalLength + separatorLength, formattedCount, CultureInfo.InvariantCulture);
        _fieldCount = fieldCount;
        _currentField = 0;
    }

    /// <summary>
    /// Initializes a new CSV string handler for field-by-field building.
    /// </summary>
    /// <param name="fieldCount">Number of fields that will be appended</param>
    public CsvStringHandler(int fieldCount)
    {
        // Estimate 20 chars per field average
        _handler = new DefaultInterpolatedStringHandler(fieldCount * 20, fieldCount, CultureInfo.InvariantCulture);
        _fieldCount = fieldCount;
        _currentField = 0;
    }

    /// <summary>
    /// Appends a literal string.
    /// </summary>
    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    /// <summary>
    /// Appends a formatted string value.
    /// </summary>
    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value ?? string.Empty);
    }

    /// <summary>
    /// Appends a formatted object value with optional format string.
    /// </summary>
    public void AppendFormatted<T>(T value, string? format = null)
    {
        if (value is null)
        {
            return; // Skip null values
        }

        if (format != null)
        {
            _handler.AppendFormatted(value, format);
        }
        else
        {
            _handler.AppendFormatted(value);
        }
    }

    /// <summary>
    /// Appends a CSV field value with automatic separator handling.
    /// Null values are converted to empty strings.
    /// </summary>
    /// <param name="value">The field value to append</param>
    public void AppendField(string? value)
    {
        if (_currentField > 0)
        {
            _handler.AppendLiteral(";");
        }
        _handler.AppendFormatted(value ?? string.Empty);
        _currentField++;
    }

    /// <summary>
    /// Appends a CSV field value with formatting.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="value">The field value to append</param>
    /// <param name="format">Optional format string</param>
    public void AppendField<T>(T value, string? format = null)
    {
        if (_currentField > 0)
        {
            _handler.AppendLiteral(";");
        }

        if (value is null)
        {
            _handler.AppendFormatted(string.Empty);
        }
        else if (format != null)
        {
            _handler.AppendFormatted(value, format);
        }
        else
        {
            _handler.AppendFormatted(value);
        }

        _currentField++;
    }

    /// <summary>
    /// Returns the CSV string.
    /// </summary>
    public override string ToString() => _handler.ToString();

    /// <summary>
    /// Gets the constructed CSV string and clears the handler.
    /// </summary>
    public string ToStringAndClear() => _handler.ToStringAndClear();
}

/// <summary>
/// Extension methods for CSV string building.
/// </summary>
public static class CsvStringHandlerExtensions
{
    /// <summary>
    /// Builds a CSV string using field-by-field appending.
    /// More efficient than string interpolation for many fields.
    /// </summary>
    /// <param name="fieldCount">Number of fields</param>
    /// <param name="builder">Action to build the CSV fields</param>
    /// <returns>CSV string</returns>
    public static string BuildCsv(int fieldCount, Action<CsvStringHandler> builder)
    {
        var handler = new CsvStringHandler(fieldCount);
        builder(handler);
        return handler.ToString();
    }
}
