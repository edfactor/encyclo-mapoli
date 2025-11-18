using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Demoulas.ProfitSharing.Analyzers;

internal static class Utils
{
    internal static bool IsFastEndpoint(INamedTypeSymbol symbol)
    {
        for (INamedTypeSymbol? current = symbol; current is not null; current = current.BaseType)
        {
            var name = current.ConstructedFrom?.ToDisplayString();
            // Matches FastEndpoints.Endpoint<,> & FastEndpoints.EndpointWithoutRequest<>
            if (name is "FastEndpoints.Endpoint<,>" or "FastEndpoints.EndpointWithoutRequest<>")
            {
                return true;
            }
        }

        return false;
    }

    internal static bool DerivesFromFastEndpoints(INamedTypeSymbol type)
    {
        for (var cur = type; cur is not null; cur = cur.BaseType)
        {
            var constructed = cur.ConstructedFrom?.ToDisplayString();
            if (constructed is "FastEndpoints.Endpoint<TRequest, TResponse>")
            {
                return true;
            }
        }

        return false;
    }

    internal static INamedTypeSymbol? GetEndpointResponseType(INamedTypeSymbol endpointSymbol)
    {
        for (INamedTypeSymbol? current = endpointSymbol; current is not null; current = current.BaseType)
        {
            var constructed = current.ConstructedFrom?.ToDisplayString();
            if (constructed is "FastEndpoints.Endpoint<TRequest, TResponse>" && current.TypeArguments.Length == 2)
            {
                return current.TypeArguments[1] as INamedTypeSymbol;
            }

            if (constructed is "FastEndpoints.EndpointWithoutRequest<TResponse>" && current.TypeArguments.Length == 1)
            {
                return current.TypeArguments[0] as INamedTypeSymbol;
            }
        }

        return null;
    }

    internal static bool Implements(ITypeSymbol type, INamedTypeSymbol iface)
    {
        if (type is null)
        {
            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(type, iface))
        {
            return true;
        }

        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iface));
    }
    public static bool HasAnyMemberImplementingInterface(
         INamedTypeSymbol typeSymbol,
         string interfaceName = "IIsOk",  // Can be fully qualified like "MyNamespace.IIsOk"
         HashSet<ITypeSymbol>? visitedTypes = null)
    {
        // Initialize visited types set to prevent infinite recursion
        visitedTypes ??= new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        // If we've already visited this type, return false to avoid cycles
        if (!visitedTypes.Add(typeSymbol))
        {
            return false;
        }

        // Check if the type itself implements the interface
        if (ImplementsInterfaceByName(typeSymbol, interfaceName))
        {
            return true;
        }

        // Check generic type arguments
        if (typeSymbol.IsGenericType)
        {
            foreach (var typeArg in typeSymbol.TypeArguments)
            {
                if (ImplementsInterfaceByName(typeArg, interfaceName))
                {
                    return true;
                }

                // Recursively check type argument if it's a named type
                if (typeArg is INamedTypeSymbol namedTypeArg
                    && HasAnyMemberImplementingInterface(namedTypeArg, interfaceName, visitedTypes))
                {
                    return true;
                }
            }
        }

        // Get all properties and fields from the type
        var members = typeSymbol.GetMembers()
            .Where(m => m is IPropertySymbol || m is IFieldSymbol)
            .Where(m => !m.IsStatic); // Usually you want instance members

        foreach (var member in members)
        {
            ITypeSymbol? memberType = member switch
            {
                IPropertySymbol property => property.Type,
                IFieldSymbol field => field.Type,
                _ => null
            };

            // Skip if member type is null or error type
            if (memberType == null || memberType.TypeKind == TypeKind.Error)
            {
                continue;
            }

            // Check if this member directly implements the interface
            if (ImplementsInterfaceByName(memberType, interfaceName))
            {
                return true;
            }

            // If it's not a value type, recursively examine its members
            if (IsValueTypeOrPrimitive(memberType))
            {
                continue;
            }

            // Handle nullable reference types
            if (memberType is INamedTypeSymbol namedType)
            {
                // Unwrap nullable if needed
                if (namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                    && namedType.TypeArguments.Length == 1)
                {
                    memberType = namedType.TypeArguments[0];
                }

                // Check generic type arguments of the member type
                if (namedType.IsGenericType)
                {
                    foreach (var typeArg in namedType.TypeArguments)
                    {
                        if (ImplementsInterfaceByName(typeArg, interfaceName))
                        {
                            return true;
                        }

                        if (typeArg is INamedTypeSymbol namedTypeArg
                            && !IsValueTypeOrPrimitive(typeArg)
                            && HasAnyMemberImplementingInterface(namedTypeArg, interfaceName, visitedTypes))
                        {
                            return true;
                        }
                    }
                }

                // Handle collection types - check the element type
                if (IsCollectionType(namedType, out var elementType) && elementType != null)
                {
                    // Check if element type implements the interface
                    if (ImplementsInterfaceByName(elementType, interfaceName))
                    {
                        return true;
                    }

                    // Recursively check element type if it's not a value type
                    if (!IsValueTypeOrPrimitive(elementType) && elementType is INamedTypeSymbol namedElementType
                        && HasAnyMemberImplementingInterface(namedElementType, interfaceName, visitedTypes))
                    {
                        return true;
                    }
                }
            }

            // Handle arrays
            if (memberType is IArrayTypeSymbol arrayType)
            {
                var elementTypeArray = arrayType.ElementType;
                if (ImplementsInterfaceByName(elementTypeArray, interfaceName))
                {
                    return true;
                }

                if (!IsValueTypeOrPrimitive(elementTypeArray) && elementTypeArray is INamedTypeSymbol namedArrayElement
                    && HasAnyMemberImplementingInterface(namedArrayElement, interfaceName, visitedTypes))
                {
                    return true;
                }
            }

            // Recursively check this member's type
            if (memberType is INamedTypeSymbol namedMemberType
                && HasAnyMemberImplementingInterface(namedMemberType, interfaceName, visitedTypes))
            {
                return true;
            }
        }

        // Optionally check base type
        if (typeSymbol.BaseType != null
            && typeSymbol.BaseType.SpecialType != SpecialType.System_Object
            && typeSymbol.BaseType.SpecialType != SpecialType.System_ValueType
            && HasAnyMemberImplementingInterface(typeSymbol.BaseType, interfaceName, visitedTypes))
        {
            return true;
        }

        return false;
    }

    private static bool ImplementsInterfaceByName(ITypeSymbol type, string interfaceName)
    {
        // Check if the type itself matches the interface name
        if (type.TypeKind == TypeKind.Interface && IsMatchingInterfaceName(type, interfaceName))
        {
            return true;
        }

        // Check all interfaces
        return type.AllInterfaces.Any(i => IsMatchingInterfaceName(i, interfaceName));
    }

    private static bool IsMatchingInterfaceName(ITypeSymbol type, string interfaceName)
    {
        // Simple name match (e.g., "IIsOk")
        if (type.Name == interfaceName)
        {
            return true;
        }

        // Check metadata name for generic types (e.g., "IIsOk`1")
        if (type.MetadataName == interfaceName || type.MetadataName.StartsWith(interfaceName + "`"))
        {
            return true;
        }

        // Full name match with namespace (e.g., "MyNamespace.IIsOk")
        var fullName = type.ToDisplayString();
        if (fullName == interfaceName)
        {
            return true;
        }

        // Handle case where interfaceName is partial but with namespace
        if (!interfaceName.Contains("."))
        {
            return false;
        }

        // For "MyNamespace.IIsOk", also match "MyNamespace.IIsOk<T>"
        var baseFullName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        if (baseFullName == interfaceName || baseFullName.StartsWith(interfaceName + "<"))
        {
            return true;
        }

        // Also check with fully qualified format
        var fullyQualifiedName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (fullyQualifiedName.Contains(interfaceName))
        {
            return true;
        }

        return false;
    }

    private static bool IsValueTypeOrPrimitive(ITypeSymbol type)
    {
        // Check if it's a value type
        if (type.IsValueType)
        {
            return true;
        }

        // Check for string (which is a reference type but we treat as primitive)
        if (type.SpecialType == SpecialType.System_String)
        {
            return true;
        }

        // Check for other special types you might want to skip
        switch (type.SpecialType)
        {
            case SpecialType.System_Object:
            case SpecialType.System_Void:
                return true;
        }

        // You might also want to skip certain namespaces
        var ns = type.ContainingNamespace?.ToDisplayString();
        if (ns != null && (ns.StartsWith("System.") || ns == "System"))
        {
            // Optionally skip System types to avoid deep diving into framework types
            // Remove this if you want to examine System types too
            return true;
        }

        return false;
    }

    private static bool IsCollectionType(INamedTypeSymbol type, out ITypeSymbol? elementType)
    {
        elementType = null;

        // Check for common collection interfaces
        var collectionInterfaces = new[]
        {
            "System.Collections.Generic.IEnumerable`1",
            "System.Collections.Generic.IList`1",
            "System.Collections.Generic.ICollection`1",
            "System.Collections.Generic.ISet`1",
            "System.Collections.Generic.IDictionary`2"
        };

        foreach (var iface in type.AllInterfaces.Concat(new[] { type }))
        {
            if (iface.IsGenericType && collectionInterfaces.Contains(iface.ConstructedFrom.ToDisplayString())
                && iface.TypeArguments.Length > 0)
            {
                elementType = iface.TypeArguments[0];
                // For dictionary, you'd also want to check TypeArguments[1] for the value type
                return true;
            }
        }

        return false;
    }
}
