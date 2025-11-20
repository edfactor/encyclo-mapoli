using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Services.Serialization;

/// <summary>
/// A System.Text.Json <see cref="JsonConverterFactory"/> that applies role-aware masking rules
/// to response DTOs (namespaces containing ".Contracts.Response."). Masking happens at serialization time
/// and never interferes with request model binding (deserialization path is pass-through).
/// </summary>
public sealed class MaskingJsonConverterFactory : JsonConverterFactory
{
    private static readonly Type _stringType = typeof(string);
    private static readonly ConcurrentDictionary<Type, TypeMetadata> _typeMetadataCache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert.IsAbstract || typeToConvert.IsInterface)
        {
            return false;
        }
        if (typeToConvert.IsPrimitive || typeToConvert == _stringType)
        {
            return false;
        }
        if (typeToConvert.Assembly.GetName().Name?.StartsWith("Demoulas.ProfitSharing", StringComparison.OrdinalIgnoreCase) != true)
        {
            return false;
        }
        string? ns = typeToConvert.Namespace;
        if (ns is null || (
            !ns.Contains(".Contracts.Response.", StringComparison.Ordinal) &&
            !ns.EndsWith(".Contracts.Response", StringComparison.Ordinal)))
        {
            return false;
        }

        TypeMetadata meta = _typeMetadataCache.GetOrAdd(typeToConvert, BuildMetadata);
        // Only enable converter if at least one property could be masked (performance)
        return meta.HasMaskableProperties;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        TypeMetadata meta = _typeMetadataCache.GetOrAdd(typeToConvert, BuildMetadata);
        Type concrete = typeof(MaskingConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(concrete, meta, options)!;
    }

    private static TypeMetadata BuildMetadata(Type t)
    {
        List<PropertyMetadata> props = new();
        bool hasMaskable = false;

        MaskSensitiveAttribute? classMask = t.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true);
        UnmaskSensitiveAttribute? classUnmask = t.GetCustomAttribute<UnmaskSensitiveAttribute>(inherit: true);

        foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!pi.CanRead)
            {
                continue;
            }
            Type underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
            bool isDecimal = underlying == typeof(decimal);
            bool isOtherNumeric = underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short) ||
                                  underlying == typeof(uint) || underlying == typeof(ulong) || underlying == typeof(ushort) ||
                                  underlying == typeof(byte) || underlying == typeof(sbyte) || underlying == typeof(double) || underlying == typeof(float);
            bool primitiveOrString = underlying.IsPrimitive || underlying == _stringType || underlying.IsEnum || isDecimal || isOtherNumeric;

            // Check if property is a dictionary with decimal values
            bool isDictionaryWithDecimalValues = CheckIsDictionaryWithDecimalValues(pi.PropertyType);

            MaskSensitiveAttribute? maskAttr = pi.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true) ?? classMask;
            UnmaskSensitiveAttribute? unmaskAttr = pi.GetCustomAttribute<UnmaskSensitiveAttribute>(inherit: true) ?? classUnmask;

            bool maskAll = maskAttr != null && (maskAttr.Roles.Length == 0);
            HashSet<string>? maskRoles = (maskAttr != null && !maskAll && maskAttr.Roles.Length > 0) ? new HashSet<string>(maskAttr.Roles, StringComparer.OrdinalIgnoreCase) : null;
            bool unmaskAll = unmaskAttr != null && (unmaskAttr.Roles.Length == 0);
            HashSet<string>? unmaskRoles = (unmaskAttr != null && !unmaskAll && unmaskAttr.Roles.Length > 0) ? new HashSet<string>(unmaskAttr.Roles, StringComparer.OrdinalIgnoreCase) : null;

            bool potentiallyMaskable = maskAttr != null || unmaskAttr != null || isDecimal || isDictionaryWithDecimalValues;
            if (potentiallyMaskable)
            {
                hasMaskable = true;
            }

            // capture any explicit JsonPropertyName
            string? jsonName = pi.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
            props.Add(new PropertyMetadata(pi, maskAll, maskRoles, unmaskAll, unmaskRoles, isDecimal, isOtherNumeric, primitiveOrString, jsonName, isDictionaryWithDecimalValues));
        }
        return new TypeMetadata(t, props, hasMaskable);
    }

    private static bool CheckIsDictionaryWithDecimalValues(Type type)
    {
        // Check if type implements IDictionary<TKey, TValue> with TValue = decimal
        if (type.IsGenericType)
        {
            Type genericTypeDef = type.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(Dictionary<,>) || genericTypeDef == typeof(IDictionary<,>))
            {
                Type[] genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 2 && genericArgs[1] == typeof(decimal))
                {
                    return true;
                }
            }
        }

        // Check implemented interfaces
        foreach (Type iface in type.GetInterfaces())
        {
            if (iface.IsGenericType)
            {
                Type ifaceGenericDef = iface.GetGenericTypeDefinition();
                if (ifaceGenericDef == typeof(IDictionary<,>))
                {
                    Type[] genericArgs = iface.GetGenericArguments();
                    if (genericArgs.Length == 2 && genericArgs[1] == typeof(decimal))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private sealed class MaskingConverter<T> : JsonConverter<T> where T : class
    {
        private readonly TypeMetadata _meta;
        private readonly JsonSerializerOptions _originalOptions;
        private readonly JsonSerializerOptions _unmaskedOptions;

        // Constructor is used via reflection in CreateConverter (Activator.CreateInstance)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection in CreateConverter")]
        public MaskingConverter(TypeMetadata meta, JsonSerializerOptions original)
        {
            _meta = meta;
            _originalOptions = original;
            // Clone options minus this factory to avoid recursion during pass-through deserialization
            _unmaskedOptions = new JsonSerializerOptions(original);
            for (int i = _unmaskedOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (_unmaskedOptions.Converters[i] is MaskingJsonConverterFactory)
                {
                    _unmaskedOptions.Converters.RemoveAt(i);
                }
            }
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Pass-through (no masking on read)
            try
            {
                return JsonSerializer.Deserialize<T>(ref reader, _unmaskedOptions);
            }
            catch (JsonException ex)
            {
                // For better debugging, try to deserialize to JsonElement first to capture the full JSON
                string fullJsonDebug = TryExtractFullJsonDocument(ref reader, ex);
                System.Diagnostics.Debug.WriteLine($"[MaskingConverter.Read] Deserialization failed for type {typeToConvert.Name}");
                System.Diagnostics.Debug.WriteLine($"  Path: {ex.Path}");
                System.Diagnostics.Debug.WriteLine($"  LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                System.Diagnostics.Debug.WriteLine($"  Inner exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine(fullJsonDebug);
                throw;
            }
        }

        private static string TryExtractFullJsonDocument(ref Utf8JsonReader reader, JsonException ex)
        {
            try
            {
                var readerCopy = reader;
                var buffer = new System.Text.StringBuilder();
                buffer.AppendLine("  === Full JSON Context ===");

                // Try to parse as a generic JsonElement to see the full structure
                try
                {
                    System.Text.Json.JsonDocument doc;
                    if (readerCopy.ValueSpan.Length > 0)
                    {
                        doc = System.Text.Json.JsonDocument.Parse(readerCopy.ValueSpan);
                    }
                    else
                    {
                        // For ValueSequence, we need to convert to a byte array or use a different approach
                        var sequenceBytes = readerCopy.ValueSequence.ToArray();
                        doc = System.Text.Json.JsonDocument.Parse(sequenceBytes);
                    }
                    
                    buffer.AppendLine($"  Full JSON: {doc.RootElement.GetRawText()}");
                    
                    // If we have a path, try to navigate to it and show that value
                    if (!string.IsNullOrEmpty(ex.Path))
                    {
                        buffer.AppendLine($"  Problem at path '{ex.Path}':");
                        string[] pathSegments = ex.Path.Split(new[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                        var current = doc.RootElement;
                        
                        foreach (var segment in pathSegments)
                        {
                            if (segment.StartsWith('$'))
                            {continue;}
                            
                            // Remove array indices like [0]
                            string cleanSegment = System.Text.RegularExpressions.Regex.Replace(segment, @"\[\d+\]", "");
                            
                            if (current.TryGetProperty(cleanSegment, out var prop))
                            {
                                buffer.AppendLine($"    {cleanSegment}: {prop.GetRawText()}");
                                current = prop;
                            }
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    buffer.AppendLine($"  Could not parse as JsonDocument: {parseEx.Message}");
                    buffer.AppendLine($"  Trying to extract raw bytes from reader...");
                    
                    // Fallback: try to show raw bytes
                    try
                    {
                        byte[] bytes = readerCopy.ValueSpan.ToArray();
                        string rawText = System.Text.Encoding.UTF8.GetString(bytes);
                        buffer.AppendLine($"  Raw value (first 500 chars): {rawText.Substring(0, Math.Min(500, rawText.Length))}");
                    }
                    catch (Exception bytesEx)
                    {
                        buffer.AppendLine($"  Could not extract raw bytes: {bytesEx.Message}");
                    }
                }

                return buffer.ToString();
            }
            catch (Exception debugEx)
            {
                return $"  === Debug extraction failed: {debugEx.Message} ===";
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            RoleContextSnapshot? ctx = MaskingAmbientRoleContext.Current;
            if (ctx?.IsExecutiveAdmin == true)
            {
                JsonSerializer.Serialize(writer, value, _unmaskedOptions);
                return;
            }

            bool isExecutiveRow = DetermineExecutiveRow(value);
            writer.WriteStartObject();
            foreach (PropertyMetadata pm in _meta.Properties)
            {
                string propName = pm.JsonName ?? options.PropertyNamingPolicy?.ConvertName(pm.Property.Name) ?? pm.Property.Name;
                writer.WritePropertyName(propName);
                object? propVal = pm.Property.GetValue(value);

                bool shouldMask = ShouldMask(pm, isExecutiveRow, ctx);
                if (!shouldMask)
                {
                    if (propVal is null)
                    {
                        writer.WriteNullValue();
                    }
                    else if (pm.IsPrimitiveOrString)
                    {
                        // Write primitive directly for performance where possible
                        WritePrimitive(writer, propVal);
                    }
                    else if (pm.IsDictionaryWithDecimalValues)
                    {
                        // Dictionary with decimal values - serialize without masking
                        JsonSerializer.Serialize(writer, propVal, propVal.GetType(), _originalOptions);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, propVal, propVal.GetType(), _originalOptions);
                    }
                }
                else
                {
                    if (pm.IsDictionaryWithDecimalValues)
                    {
                        WriteMaskedDictionary(writer, propVal, isExecutiveRow, ctx);
                    }
                    else
                    {
                        WriteMasked(writer, propVal, pm);
                    }
                }
            }
            writer.WriteEndObject();
        }

        private static bool DetermineExecutiveRow(object instance)
        {
            PropertyInfo? execProp = instance.GetType().GetProperty("IsExecutive", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (execProp != null && execProp.PropertyType == typeof(bool))
            {
                object? v = execProp.GetValue(instance);
                if (v is bool b)
                {
                    return b;
                }
            }
            return false;
        }

        private static void WritePrimitive(Utf8JsonWriter writer, object value)
        {
            switch (value)
            {
                case string s: writer.WriteStringValue(s); break;
                case bool b: writer.WriteBooleanValue(b); break;
                case int i: writer.WriteNumberValue(i); break;
                case long l: writer.WriteNumberValue(l); break;
                case short sh: writer.WriteNumberValue(sh); break;
                case uint ui: writer.WriteNumberValue(ui); break;
                case ulong ul: writer.WriteNumberValue(ul); break;
                case ushort us: writer.WriteNumberValue(us); break;
                case byte by: writer.WriteNumberValue(by); break;
                case sbyte sb: writer.WriteNumberValue(sb); break;
                case float f: writer.WriteNumberValue(f); break;
                case double d: writer.WriteNumberValue(d); break;
                case decimal dec: writer.WriteNumberValue(dec); break;
                case Enum e: writer.WriteStringValue(e.ToString()); break;
                default:
                    JsonSerializer.Serialize(writer, value, value.GetType());
                    break;
            }
        }

        private static void WriteMasked(Utf8JsonWriter writer, object? value, PropertyMetadata pm)
        {
            if (value is null)
            {
                // Chosen behavior: keep nulls as null even if masked (simpler & explicit)
                writer.WriteNullValue();
                return;
            }
            if (value is string s)
            {
                writer.WriteStringValue(MaskAlphaNumeric(s));
                return;
            }
            if (pm.IsDecimal || pm.IsOtherNumeric)
            {
                string raw = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                writer.WriteStringValue(MaskDigits(raw));
                return;
            }
            // Fallback: serialize then apply masking to resulting JSON token if it's a primitive
            string serialized = JsonSerializer.Serialize(value, value.GetType());
            writer.WriteStringValue(MaskAlphaNumeric(serialized.Trim('"')));
        }

        private static void WriteMaskedDictionary(Utf8JsonWriter writer, object? value, bool isExecutiveRow, RoleContextSnapshot? ctx)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            bool privilegedContext = isExecutiveRow || (ctx?.IsItDevOps == true);

            // Use reflection to access dictionary entries
            Type valueType = value.GetType();
            PropertyInfo? keysProperty = valueType.GetProperty("Keys");
            PropertyInfo? itemProperty = valueType.GetProperty("Item");

            if (keysProperty == null || itemProperty == null)
            {
                // Fallback: serialize as-is if we can't introspect
                JsonSerializer.Serialize(writer, value, valueType);
                return;
            }

            object? keys = keysProperty.GetValue(value);
            if (keys is System.Collections.IEnumerable enumerable)
            {
                writer.WriteStartObject();
                foreach (object? key in enumerable)
                {
                    if (key is not null)
                    {
                        string keyStr = key.ToString() ?? string.Empty;
                        writer.WritePropertyName(keyStr);

                        object? dictValue = itemProperty.GetValue(value, new[] { key });

                        // Mask decimal values in privileged contexts
                        if (dictValue is decimal decValue && privilegedContext)
                        {
                            string raw = Convert.ToString(decValue, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                            writer.WriteStringValue(MaskDigits(raw));
                        }
                        else if (dictValue is null)
                        {
                            writer.WriteNullValue();
                        }
                        else
                        {
                            JsonSerializer.Serialize(writer, dictValue, dictValue.GetType());
                        }
                    }
                }
                writer.WriteEndObject();
            }
            else
            {
                // Fallback
                JsonSerializer.Serialize(writer, value, valueType);
            }
        }

        private static bool ShouldMask(PropertyMetadata pm, bool isExecutiveRow, RoleContextSnapshot? ctx)
        {
            // 1. Unmask attribute (no roles) => always unmasked
            if (pm.UnmaskAllRoles)
            {
                return false;
            }

            // 2. Role-scoped unmask: if caller in list => unmasked; if not, fall back to baseline (don't force mask)
            if (pm.UnmaskRoles is not null && ctx is not null && ctx.Roles.Any(r => pm.UnmaskRoles.Contains(r)))
            {
                return false;
            }

            bool privilegedContext = isExecutiveRow || (ctx?.IsItDevOps == true);
            bool maskedByAttribute = pm.MaskAllRoles || (pm.MaskRoles is not null && ctx is not null && ctx.Roles.Any(r => pm.MaskRoles.Contains(r)));

            // 3. Decimal baseline: mask for privileged contexts (IT or executive row) unless unmasked; otherwise show value
            if (pm.IsDecimal)
            {
                return privilegedContext; // attribute presence does not expand masking beyond privileged contexts per legacy middleware
            }

            // 4. Dictionary with decimal values: same masking rules as decimal properties
            if (pm.IsDictionaryWithDecimalValues)
            {
                return privilegedContext;
            }

            // 5. Non-decimal primitives/strings: only mask when attribute says mask AND context is privileged (IT or executive row)
            if (maskedByAttribute && privilegedContext)
            {
                return true;
            }

            return false;
        }

        private static string MaskAlphaNumeric(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            Span<char> buf = input.Length <= 512 ? stackalloc char[input.Length] : new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                buf[i] = char.IsLetterOrDigit(c) ? 'X' : c;
            }
            return new string(buf);
        }

        private static string MaskDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            Span<char> buf = input.Length <= 256 ? stackalloc char[input.Length] : new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                buf[i] = char.IsDigit(c) ? 'X' : c;
            }
            return new string(buf);
        }
    }

    private sealed record PropertyMetadata(
        PropertyInfo Property,
        bool MaskAllRoles,
        HashSet<string>? MaskRoles,
        bool UnmaskAllRoles,
        HashSet<string>? UnmaskRoles,
        bool IsDecimal,
        bool IsOtherNumeric,
        bool IsPrimitiveOrString,
        string? JsonName,
        bool IsDictionaryWithDecimalValues);

    private sealed record TypeMetadata(Type Type, IReadOnlyList<PropertyMetadata> Properties, bool HasMaskableProperties);
}
