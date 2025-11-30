using System.Globalization;
using System.Text.RegularExpressions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public static class FormatUtils
{
    public static string rformat(object? value, string type, string picClause)
    {
        string fmt;
        switch (type)
        {
            case "decimal":
                fmt = FormatDecimal((decimal)value!, picClause);
                break;
            case "long":
                fmt = FormatLong((long)value!, picClause);
                break;
            case "DateOnly?":
                fmt = FormatDate((DateOnly?)value, picClause);
                break;
            case "string?":
                fmt = FormatString((string?)value, picClause);
                break;
            default:
                throw new ArgumentException($"Unsupported type: {type}");
        }

        return fmt;
    }

    private static string FormatDecimal(decimal value, string picClause)
    {
        // Define COBOL-like decimal formats.
        string jj = picClause switch
        {
            // Weird dropping of Comma cases.
            "ZZZZ,ZZZ.99-" => FormatWithSingleComma(value, picClause.Length),
            "ZZZZZ,ZZZ,ZZZ" => FormatWithDoubleComma(value, picClause.Length + 1),
            "ZZZZZZ,ZZZ,ZZZ" => FormatWithDoubleComma(value, picClause.Length + 1),

            "ZZ,ZZZ,ZZZ.99-" => FormatCobolDecimal(value, "#,0.00", picClause.Length),
            "Z,ZZZ,ZZZ,ZZZ.99-" => FormatCobolDecimal(value, "#,0.00", picClause.Length),
            "ZZZ,ZZZ.99-" => FormatCobolDecimal(value, "#,0.00", picClause.Length),
            "ZZZ,ZZZ,ZZZ.99-" => FormatCobolDecimal(value, "#,0.00", picClause.Length),

            // no comma
            "ZZZZZZZ.99-" => FormatCobolDecimal(value, "0000000.00", picClause.Length),

            "S9(10)V99" => FormatCobolDecimal(value, "0000000000.00", 13),
            "9(3)V99" => FormatCobolDecimal(value, "000.00", 6),
            "S9(5)V99" => FormatCobolDecimal(value, "00000.00", 8),
            _ => throw new ArgumentException($"Unsupported PIC clause for decimal: {picClause}")
        };
        return jj;
    }


    private static string FormatLong(long value, string picClause)
    {
        return picClause switch
        {
            "9(11)" => value.ToString("D11"),
            "9(07)" => value.ToString("D7"),
            "9(05)" => value.ToString("D5"),
            "9(4)" => value.ToString("D4"),
            "99" => value.ToString("D2"),
            "S9(11)" => FormatSignedLong(value, 11),
            "Z,ZZZ,ZZ9." => FormatCobolLong(value, "#,###,###0", 10 - 2),
            "ZZZ,ZZ9." => FormatCobolLong(value, "###,###0", 9 - 2),
            "Z,ZZZ,ZZ9" => FormatCobolLong(value, "###,###0", 8),
            "ZZ,ZZ9." => FormatCobolLong(value, "###,###0", 8 - 2),
            "ZZ,ZZZ,ZZZ-" => FormatCobolLong(value, "###,###0", 10),
            _ => throw new ArgumentException($"Unsupported PIC clause for long: {picClause}")
        };
    }

    // In order to print nicely, values over 1 million do not have a second comma.
    //   1,000,000 <-- bad
    //    1000,000 <--good
    private static string FormatWithSingleComma(decimal number, int length)
    {
        string numberStr = number.ToString("#,###.00 ;#,###.00-");

        string[] parts = numberStr.Split(',');
        if (parts.Length == 3)
        {
            return $"{parts[0]}{parts[1]},{parts[2]}";
        }

        return numberStr.PadLeft(length);
    }

    private static string FormatWithDoubleComma(decimal number, int length)
    {
        string numberStr = number.ToString("#,### ;#,###.-");

        // if we have three commas remove the first one.
        if (numberStr.Count(c => c == ',') == 3)
        {
            int cdex = numberStr.IndexOf(",");
            numberStr = numberStr[..cdex] + numberStr[cdex + 1];
        }

        return numberStr.PadLeft(length);
    }


    private static string FormatSignedLong(long value, int width)
    {
        string formattedValue = value < 0 ? $"{Math.Abs(value)}-" : $"{value} ";
        return formattedValue.PadLeft(width);
    }


    private static string FormatDate(DateOnly? date, string picClause)
    {
        if (picClause != "9(8).")
        {
            throw new ArgumentException($"Unsupported PIC clause for DateOnly?: {picClause}");
        }

        return date.HasValue ? date.Value.ToString("yyyyMMdd") : new string(' ', 8);
    }

    private static string FormatString(string? value, string picClause)
    {
        // Strip 'X(' and ')' to get the length from the PIC clause.
        if (picClause.StartsWith("X(") && picClause.EndsWith(')'))
        {
            int length = int.Parse(picClause[2..^1]);
            return (value ?? "").PadRight(length).Substring(0, length);
        }

        if (picClause == "X")
        {
            if (value == null)
            {
                return " ";
            }

            return value.PadLeft(1);
        }

        return value ?? ""; // Handle simple 'X' or unsupported patterns as raw string.
    }

    private static string FormatCobolDecimal(decimal value, string format, int width)
    {
        string formattedValue;

        // If value is zero, use ".00" (or ".00-" for signed formats) for decimal formats.
        if (value == 0m)
        {
            formattedValue = ".00 ";
        }
        else
        // Handle negative values with trailing minus.
        {
            formattedValue = value < 0
                ? $"{Math.Abs(value).ToString(format, CultureInfo.InvariantCulture)}-"
                : $"{value.ToString(format, CultureInfo.InvariantCulture)} ";
        }

        return formattedValue.PadLeft(width);
    }

    private static string FormatCobolLong(long value, string format, int width)
    {
        string formattedValue =
            // Use zeroes as usual for integer fields; no special formatting needed for zero.
            value.ToString(format, CultureInfo.InvariantCulture);

        if (value != 0)
        // Replace leading zeros with spaces.
        {
            formattedValue = Regex.Replace(formattedValue, @"(?<!\S)0+", m => new string(' ', m.Length));
        }

        return formattedValue.PadLeft(width);
    }
}
