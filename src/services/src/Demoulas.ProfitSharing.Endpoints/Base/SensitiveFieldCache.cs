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

    private static void AddSensitiveFields(Type type, HashSet<string> fields, HashSet<Type> visited)
    {
        type = NormalizeType(type);

        if (type == typeof(object))
        {
            return;
        }

        if (!visited.Add(type))
        {
            return;
        }

        if (IsResultsUnion(type))
        {
            foreach (var genericType in type.GetGenericArguments())
            {
                AddSensitiveFields(genericType, fields, visited);
            }

            return;
        }

        if (TryGetEnumerableElementType(type, out var elementType))
        {
            AddSensitiveFields(elementType, fields, visited);
            return;
        }

        if (IsLeafType(type))
        {
            return;
        }

        var isTypeMasked = type.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true) is not null;
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            var propertyType = NormalizeType(property.PropertyType);
            if (isTypeMasked)
            {
                AddLeafOrRecurse(propertyType, property.Name, fields, visited);
                continue;
            }

            if (property.GetCustomAttribute<MaskSensitiveAttribute>(inherit: true) is not null)
            {
                AddLeafOrRecurse(propertyType, property.Name, fields, visited);
            }
            else
            {
                AddSensitiveFields(propertyType, fields, visited);
            }
        }
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

    private static void AddLeafOrRecurse(Type propertyType, string propertyName, HashSet<string> fields, HashSet<Type> visited)
    {
        if (IsLeafType(propertyType))
        {
            fields.Add(propertyName);
            return;
        }

        AddSensitiveFields(propertyType, fields, visited);
    }

    private static bool IsLeafType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateOnly)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeOnly)
            || type == typeof(Guid);
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
