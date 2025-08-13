using System.Text.Json;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Middleware;

/// <summary>
/// Masks sensitive values for IT Operations role.
/// Rules:
/// 1. All decimal numbers are masked by default, but CAN be unmasked when an UnmaskAttribute is applied on the property or containing class.
/// 2. Any property/class with MaskSensitiveAttribute is masked (any type) unless also UnmaskAttribute.
/// 3. UnmaskAttribute opts out of default masking (including decimals).
/// 4. Numeric values masked become strings preserving non-digit chars; digits -> 'X'.
/// 5. Masked strings show first & last character; interior alphanumerics -> 'X'.
/// 6. Other primitive masked types similarly preserve length and first/last alphanumerics.
/// </summary>
public sealed class SensitiveValueMaskingMiddleware
{
    private readonly RequestDelegate _next;

    public SensitiveValueMaskingMiddleware(RequestDelegate next)
    {
        _next = next;
        SensitiveMaskingRegistry.EnsureInitialized();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.User.IsInRole(Role.ITOPERATIONS))
        {
            await _next(context);
            return;
        }

        Stream originalBody = context.Response.Body;
        await using MemoryStream buffer = new();
        context.Response.Body = buffer;

        try
        {
            await _next(context);
            buffer.Position = 0;
            if (context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true &&
                buffer.Length > 0 && context.Response.StatusCode is >= 200 and < 300)
            {
                using StreamReader reader = new(buffer, leaveOpen: true);
                string json = await reader.ReadToEndAsync(context.RequestAborted);
                buffer.Position = 0;
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true });
                    MemoryStream outStream = new();
                    await using (Utf8JsonWriter writer = new(outStream))
                    {
                        MaskElement(doc.RootElement, writer, null);
                    }
                    outStream.Position = 0;
                    context.Response.ContentLength = outStream.Length;
                    await outStream.CopyToAsync(originalBody, context.RequestAborted);
                    return;
                }
                catch
                {
                    buffer.Position = 0;
                    await buffer.CopyToAsync(originalBody, context.RequestAborted);
                    return;
                }
            }
            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody, context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static void MaskElement(JsonElement element, Utf8JsonWriter writer, string? currentPropertyName)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (JsonProperty prop in element.EnumerateObject())
                {
                    writer.WritePropertyName(prop.Name);
                    MaskElement(prop.Value, writer, prop.Name);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (JsonElement item in element.EnumerateArray())
                {
                    MaskElement(item, writer, currentPropertyName);
                }
                writer.WriteEndArray();
                break;
            case JsonValueKind.Number:
                string raw = element.GetRawText();
                bool hasDecimalSyntax = raw.Contains('.') || raw.Contains('E', StringComparison.OrdinalIgnoreCase);
                bool isDecimalProperty = currentPropertyName != null && SensitiveMaskingRegistry.IsDecimalProperty(currentPropertyName);
                bool isDecimalTarget = hasDecimalSyntax || isDecimalProperty;
                if (isDecimalTarget)
                {
                    // Decimals masked by default unless explicitly unmasked.
                    if (SensitiveMaskingRegistry.IsUnmasked(currentPropertyName))
                    {
                        element.WriteTo(writer);
                    }
                    else
                    {
                        writer.WriteStringValue(MaskNumberRaw(raw));
                    }
                }
                else if (SensitiveMaskingRegistry.IsMasked(currentPropertyName) && !SensitiveMaskingRegistry.IsUnmasked(currentPropertyName))
                {
                    writer.WriteStringValue(MaskNumberRaw(raw));
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
            case JsonValueKind.String:
                if (SensitiveMaskingRegistry.IsMasked(currentPropertyName) && !SensitiveMaskingRegistry.IsUnmasked(currentPropertyName))
                {
                    string s = element.GetString() ?? string.Empty;
                    writer.WriteStringValue(MaskString(s));
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                if (SensitiveMaskingRegistry.IsMasked(currentPropertyName) && !SensitiveMaskingRegistry.IsUnmasked(currentPropertyName))
                {
                    string token = element.GetRawText();
                    writer.WriteStringValue(MaskGeneric(token));
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static string MaskNumberRaw(string raw)
    {
        Span<char> chars = raw.Length <= 256 ? stackalloc char[raw.Length] : new char[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            char c = raw[i];
            chars[i] = char.IsDigit(c) ? 'X' : c;
        }
        return new string(chars);
    }

    private static string MaskString(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= 2)
        {
            return value;
        }
        Span<char> chars = value.Length <= 512 ? stackalloc char[value.Length] : new char[value.Length];
        chars[0] = value[0];
        chars[^1] = value[^1];
        for (int i = 1; i < value.Length - 1; i++)
        {
            char c = value[i];
            chars[i] = char.IsLetterOrDigit(c) ? 'X' : c;
        }
        return new string(chars);
    }

    private static string MaskGeneric(string token)
    {
        if (token.Length <= 2)
        {
            return token;
        }
        Span<char> chars = token.Length <= 256 ? stackalloc char[token.Length] : new char[token.Length];
        chars[0] = token[0];
        chars[^1] = token[^1];
        for (int i = 1; i < token.Length - 1; i++)
        {
            char c = token[i];
            chars[i] = char.IsLetterOrDigit(c) ? 'X' : c;
        }
        return new string(chars);
    }
}

internal static class SensitiveMaskingRegistry
{
    private static bool _initialized;
    private static readonly Lock _lock = new();
    private static readonly HashSet<string> _maskedNames = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _unmaskedNames = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _decimalPropertyNames = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _nonDecimalNumericPropertyNames = new(StringComparer.OrdinalIgnoreCase);

    public static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }
        lock (_lock)
        {
            if (_initialized)
            {
                return;
            }
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.StartsWith("Demoulas.ProfitSharing", StringComparison.OrdinalIgnoreCase) == true))
            {
                foreach (Type t in asm.GetTypes())
                {
                    bool classMasked = t.GetCustomAttribute<MaskSensitiveAttribute>() != null;
                    bool classUnmasked = t.GetCustomAttribute<UnmaskAttribute>() != null;
                    foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        string name = pi.Name;
                        Type pType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                        bool isDecimal = pType == typeof(decimal);
                        bool isOtherNumeric = pType == typeof(int) || pType == typeof(long) || pType == typeof(short) || pType == typeof(byte) || pType == typeof(uint) || pType == typeof(ulong) || pType == typeof(ushort) || pType == typeof(sbyte);
                        if (isDecimal)
                        {
                            _decimalPropertyNames.Add(name);
                        }
                        else if (isOtherNumeric)
                        {
                            _nonDecimalNumericPropertyNames.Add(name);
                        }
                        if (classMasked)
                        {
                            _maskedNames.Add(name);
                        }
                        if (classUnmasked)
                        {
                            _unmaskedNames.Add(name);
                        }
                        if (pi.GetCustomAttribute<MaskSensitiveAttribute>() != null)
                        {
                            _maskedNames.Add(name);
                        }
                        if (pi.GetCustomAttribute<UnmaskAttribute>() != null)
                        {
                            _unmaskedNames.Add(name);
                        }
                    }
                }
            }
            _initialized = true;
        }
    }

    public static bool IsMasked(string? propertyName) => propertyName != null && _maskedNames.Contains(propertyName);
    public static bool IsUnmasked(string? propertyName) => propertyName != null && _unmaskedNames.Contains(propertyName);
    public static bool IsDecimalProperty(string propertyName)
    {
        // Only treat as decimal when it's known decimal and not also present as a non-decimal numeric property (ambiguous -> don't mask by default)
        return _decimalPropertyNames.Contains(propertyName) && !_nonDecimalNumericPropertyNames.Contains(propertyName);
    }
}
