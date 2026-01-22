using System.Collections.Concurrent;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Endpoints.Base;

public static class SensitiveFieldCache
{
    private static readonly ConcurrentDictionary<Type, string[]> Cache = new();

    public static string[] GetSensitiveFields(Type type)
    {
        return Cache.GetOrAdd(type, BuildFields);
    }

    private static string[] BuildFields(Type type)
    {
        var fields = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<Type>();
        AddSensitiveFields(type, fields, visited);
        return fields.Count == 0 ? [] : fields.ToArray();
    }

    private static bool AddSensitiveFields(Type type, HashSet<string> fields, HashSet<Type> visited)
    {
        type = NormalizeType(type);

        if (type == typeof(object))
        {
            return false;
        }

        if (!visited.Add(type))
        {
            return false;
        }

        if (IsResultsUnion(type))
        {
            foreach (var genericType in type.GetGenericArguments())
            {
                AddSensitiveFields(genericType, fields, visited);
            }

            return fields.Count > 0;
        }

        if (TryGetEnumerableElementType(type, out var elementType))
        {
            return AddSensitiveFields(elementType, fields, visited);
        }

        if (type.IsPrimitive || type.IsEnum || type == typeof(string))
        {
            return false;
        }

        bool hasSensitive = false;
        if (type.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true) is not null)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length == 0)
                {
                    fields.Add(property.Name);
                    hasSensitive = true;
                }
            }
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            if (property.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true) is not null)
            {
                fields.Add(property.Name);
                hasSensitive = true;
                continue;
            }

            bool childSensitive = AddSensitiveFields(property.PropertyType, fields, visited);
            if (childSensitive)
            {
                fields.Add(property.Name);
                hasSensitive = true;
            }
        }

        return hasSensitive;
    }

    private static bool IsResultsUnion(Type type)
    {
        return type.IsGenericType
            && type.Name.StartsWith("Results`", StringComparison.Ordinal);
    }

    private static Type NormalizeType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        return type;
    }

    private static bool TryGetEnumerableElementType(Type type, out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType() ?? type;
            return true;
        }

        if (type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = typeof(object);
        return false;
    }
}
