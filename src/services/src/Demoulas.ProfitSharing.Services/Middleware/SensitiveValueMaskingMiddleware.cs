using System.Reflection;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Middleware;

/// <summary>
/// Role-aware response masking middleware.
/// 
/// Behavior:
/// - Honors <see cref="MaskSensitiveAttribute"/> and <see cref="UnmaskAttribute"/> at class and property levels.
/// - Attributes may specify role lists; if none are provided, the rule applies to all roles.
/// - Executive rows (isExecutive=true) are obfuscated for everyone except <see cref="Role.EXECUTIVEADMIN"/>.
/// - IT support roles continue to have decimals masked by default unless explicitly unmasked.
/// - Masking preserves length and formatting characters (digits/letters replaced with 'X').
/// 
/// Notes:
/// - This middleware rewrites successful JSON responses for users in IT roles; for others it passes through unless an attribute requires masking.
/// - Combine with authorization policies to prevent data exposure, using masking only as an additional safeguard.
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
        if (context.User.IsInRole(Role.EXECUTIVEADMIN))
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
                        var isInItDevops = context.User.IsInRole(Role.ITDEVOPS);
                        var userRoles = context.User.Claims
                            .Where(c => string.Equals(c.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase))
                            .Select(c => c.Value)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToArray();
                        MaskElement(doc.RootElement, writer, null, false, isInItDevops, userRoles);
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

    private static void MaskElement(JsonElement element, Utf8JsonWriter writer, string? currentPropertyName, bool isExecutiveRow, bool isInItOps, string[] userRoles)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                if (!isExecutiveRow && element.TryGetProperty("isExecutive", out JsonElement isExecutiveElement) && isExecutiveElement.ValueKind == JsonValueKind.True)
                {
                    isExecutiveRow = true;
                }
                foreach (JsonProperty prop in element.EnumerateObject())
                {
                    writer.WritePropertyName(prop.Name);
                    MaskElement(prop.Value, writer, prop.Name, isExecutiveRow, isInItOps, userRoles);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (JsonElement item in element.EnumerateArray())
                {
                    MaskElement(item, writer, currentPropertyName, isExecutiveRow, isInItOps, userRoles);
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
                    if (SensitiveMaskingRegistry.IsUnmaskedFor(currentPropertyName, userRoles))
                    {
                        element.WriteTo(writer);
                    }
                    else
                    {
                        if (!isInItOps && !isExecutiveRow)
                        {
                            // Non-IT Operations can see decimals for non-executive rows
                            element.WriteTo(writer);
                        }
                        else
                        {
                            writer.WriteStringValue(MaskNumberRaw(raw));
                        }
                    }
                }
                else if (SensitiveMaskingRegistry.IsMaskedFor(currentPropertyName, userRoles) && !SensitiveMaskingRegistry.IsUnmaskedFor(currentPropertyName, userRoles))
                {
                    if (isInItOps || isExecutiveRow)
                    {
                        writer.WriteStringValue(MaskNumberRaw(raw));
                    }
                    else
                    {
                        // Non-IT Operations can see decimals for non-executive rows
                        element.WriteTo(writer);
                    }
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
            case JsonValueKind.String:
                if (SensitiveMaskingRegistry.IsMaskedFor(currentPropertyName, userRoles) && !SensitiveMaskingRegistry.IsUnmaskedFor(currentPropertyName, userRoles))
                {
                    string s = element.GetString() ?? string.Empty;
                    if (isInItOps || isExecutiveRow)
                    {
                        writer.WriteStringValue(MaskString(s));
                    }
                    else
                    {
                         element.WriteTo(writer); 
                    }
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                if (SensitiveMaskingRegistry.IsMaskedFor(currentPropertyName, userRoles) && !SensitiveMaskingRegistry.IsUnmaskedFor(currentPropertyName, userRoles))
                {
                    if (isInItOps || isExecutiveRow)
                    {
                        string token = element.GetRawText();
                        writer.WriteStringValue(MaskGeneric(token));
                    } else
                    {
                        element.WriteTo(writer);
                    }
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
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        Span<char> chars = value.Length <= 512 ? stackalloc char[value.Length] : new char[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            // Replace any letter or digit with 'X'; keep punctuation/whitespace to preserve formatting pattern.
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
    private static readonly Dictionary<string, string[]?> _maskedNames = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string[]?> _unmaskedNames = new(StringComparer.OrdinalIgnoreCase);
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
                    var classMaskAttr = t.GetCustomAttribute<MaskSensitiveAttribute>();
                    var classUnmaskAttr = t.GetCustomAttribute<UnmaskAttribute>();
                    bool classMasked = classMaskAttr != null;
                    bool classUnmasked = classUnmaskAttr != null;
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
                            _maskedNames[name] = classMaskAttr!.Roles.Length == 0 ? null : classMaskAttr.Roles;
                        }
                        if (classUnmasked)
                        {
                            _unmaskedNames[name] = classUnmaskAttr!.Roles.Length == 0 ? null : classUnmaskAttr.Roles;
                        }
                        var propMask = pi.GetCustomAttribute<MaskSensitiveAttribute>();
                        if (propMask != null)
                        {
                            _maskedNames[name] = propMask.Roles.Length == 0 ? null : propMask.Roles;
                        }
                        var propUnmask = pi.GetCustomAttribute<UnmaskAttribute>();
                        if (propUnmask != null)
                        {
                            _unmaskedNames[name] = propUnmask.Roles.Length == 0 ? null : propUnmask.Roles;
                        }
                    }
                }
            }
            _initialized = true;
        }
    }

    public static bool IsMaskedFor(string? propertyName, string[] userRoles)
    {
        if (propertyName == null)
        {
            return false;
        }
        if (!_maskedNames.TryGetValue(propertyName, out var roles))
        {
            return false;
        }
        // Null/empty roles means mask for all roles
        if (roles == null || roles.Length == 0)
        {
            return true;
        }
        return userRoles.Any(r => roles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public static bool IsUnmaskedFor(string? propertyName, string[] userRoles)
    {
        if (propertyName == null)
        {
            return false;
        }
        if (!_unmaskedNames.TryGetValue(propertyName, out var roles))
        {
            return false;
        }
        // Null/empty roles means unmask for all roles
        if (roles == null || roles.Length == 0)
        {
            return true;
        }
        return userRoles.Any(r => roles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }
    public static bool IsDecimalProperty(string propertyName)
    {
        // Only treat as decimal when it's known decimal and not also present as a non-decimal numeric property (ambiguous -> don't mask by default)
        return _decimalPropertyNames.Contains(propertyName) && !_nonDecimalNumericPropertyNames.Contains(propertyName);
    }
}
