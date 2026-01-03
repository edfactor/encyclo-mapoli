using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM013 – Enforces RequestExample() and ResponseExample() methods on request/response DTOs.
/// Detects when a DTO is used as a request or response type in an endpoint and verifies
/// that the DTO has the corresponding public static example method for OpenAPI documentation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RequestResponseExampleMethodAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM013";

    private static readonly LocalizableString Title =
        "Request/Response DTO must have example method";

    private static readonly LocalizableString MessageFormat =
        "Type '{0}' used as {1} in endpoint must have a public static `{2}()` method";

    private const string Category = "API Documentation";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Request and response DTOs must implement public static RequestExample() and ResponseExample() methods " +
                     "for OpenAPI documentation generation. Primitive types, framework types, and Results<T> wrappers are excluded.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDecl)
        {
            return;
        }

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDecl, context.CancellationToken) as INamedTypeSymbol;
        if (classSymbol is null)
        {
            return;
        }

        // Only analyze concrete, non-abstract endpoint classes
        if (classSymbol.IsAbstract)
        {
            return;
        }

        // Check if this class derives from ProfitSharingEndpoint<TRequest, TResponse> or variants
        var (requestType, responseType) = ExtractEndpointTypeParameters(classSymbol);
        if (requestType is null && responseType is null)
        {
            return; // Not a relevant endpoint
        }

        // Analyze request type if present
        if (requestType is not null && ShouldCheckType(requestType) && !HasPublicStaticMethod(requestType, "RequestExample"))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                classSymbol.Locations[0],
                requestType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                "request",
                "RequestExample");
            context.ReportDiagnostic(diagnostic);
        }

        // Analyze response type if present
        if (responseType is not null && ShouldCheckType(responseType) && !HasPublicStaticMethod(responseType, "ResponseExample"))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                classSymbol.Locations[0],
                responseType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                "response",
                "ResponseExample");
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Extracts TRequest and TResponse type parameters from endpoint base class.
    /// Handles ProfitSharingEndpoint<TRequest, TResponse> and ProfitSharingRequestEndpoint<TRequest>.
    /// </summary>
    private static (INamedTypeSymbol? RequestType, INamedTypeSymbol? ResponseType) ExtractEndpointTypeParameters(INamedTypeSymbol classSymbol)
    {
        INamedTypeSymbol? requestType = null;
        INamedTypeSymbol? responseType = null;

        for (var current = classSymbol; current is not null; current = current.BaseType)
        {
            var constructedFrom = current.ConstructedFrom?.ToDisplayString();

            // Check for ProfitSharingEndpoint<TRequest, TResponse>
            if ((constructedFrom is "Demoulas.ProfitSharing.Endpoints.Base.ProfitSharingEndpoint<,>" ||
                constructedFrom is "ProfitSharingEndpoint<,>") &&
                current.TypeArguments.Length >= 2)
            {
                requestType = current.TypeArguments[0] as INamedTypeSymbol;
                responseType = current.TypeArguments[1] as INamedTypeSymbol;
                break;
            }

            // Check for ProfitSharingRequestEndpoint<TRequest>
            if ((constructedFrom is "Demoulas.ProfitSharing.Endpoints.Base.ProfitSharingRequestEndpoint<>" ||
                constructedFrom is "ProfitSharingRequestEndpoint<>") &&
                current.TypeArguments.Length >= 1)
            {
                requestType = current.TypeArguments[0] as INamedTypeSymbol;
                break;
            }

            // Fallback: Check for FastEndpoints.Endpoint<TRequest, TResponse>
            if (constructedFrom is "FastEndpoints.Endpoint<,>" &&
                current.TypeArguments.Length >= 2)
            {
                requestType = current.TypeArguments[0] as INamedTypeSymbol;
                responseType = current.TypeArguments[1] as INamedTypeSymbol;
                break;
            }

            // Fallback: Check for FastEndpoints.EndpointWithoutRequest<TResponse>
            if (constructedFrom is "FastEndpoints.EndpointWithoutRequest<>" &&
                current.TypeArguments.Length >= 1)
            {
                responseType = current.TypeArguments[0] as INamedTypeSymbol;
                break;
            }
        }

        return (requestType, responseType);
    }

    /// <summary>
    /// Determines whether a type should be checked for example methods.
    /// Excludes primitives, framework types, Results<T>, and other special types.
    /// </summary>
    private static bool ShouldCheckType(INamedTypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        // Exclude primitive and special types
        if (IsPrimitiveOrSpecialType(typeSymbol))
        {
            return false;
        }

        // Exclude Results<T> and FastEndpoints framework types
        var fullName = typeSymbol.ToDisplayString();
        if (fullName.StartsWith("FastEndpoints.") ||
            fullName.StartsWith("Microsoft.AspNetCore.") ||
            fullName is "Results")
        {
            return false;
        }

        // Exclude generic type parameters without concrete implementations
        if (typeSymbol.TypeKind == TypeKind.TypeParameter)
        {
            return false;
        }

        // Exclude file download/stream result types
        if (fullName.Contains("FileResult") || fullName.Contains("StreamResult"))
        {
            return false;
        }

        // Exclude abstract classes and interfaces
        if (typeSymbol.IsAbstract || typeSymbol.TypeKind == TypeKind.Interface)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a type is a primitive or special .NET type that should be excluded from analysis.
    /// </summary>
    private static bool IsPrimitiveOrSpecialType(INamedTypeSymbol typeSymbol)
    {
        var specialType = typeSymbol.SpecialType;
        if (specialType != SpecialType.None)
        {
            return true;
        }

        var fullName = typeSymbol.ToDisplayString();

        // Exclude common built-in types
        var excludedPatterns = new[]
        {
            "System.Collections.Generic.IEnumerable<",
            "System.Collections.Generic.IReadOnlyList<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<",
            "System.Collections.Generic.IQueryable<",
            "System.Collections.Generic.List<",
            "System.Collections.Generic.Dictionary<",
            "System.Collections.Generic.HashSet<",
            "System.Linq.IQueryable<",
            "System.Collections.Generic.IAsyncEnumerable<",
            "System.IO.Stream",
            "System.IO.MemoryStream",
            "System.String",
            "System.Int32",
            "System.Int64",
            "System.Decimal",
            "System.Boolean",
            "System.Guid",
            "System.DateTime",
            "System.DateTimeOffset",
            "System.TimeSpan",
            "System.DateOnly",
            "System.TimeOnly",
            "System.Byte",
            "System.Char",
            "System.Double",
            "System.Single",
            "System.Nullable<",
            "System.Task",
            "System.Threading.Tasks.Task"
        };

        return excludedPatterns.Any(pattern => fullName.StartsWith(pattern));
    }

    /// <summary>
    /// Verifies that a type has a public static method with the specified name that returns the same type.
    /// </summary>
    private static bool HasPublicStaticMethod(INamedTypeSymbol typeSymbol, string methodName)
    {
        var method = typeSymbol.GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m =>
                m.IsStatic &&
                m.DeclaredAccessibility == Accessibility.Public &&
                m.Parameters.Length == 0 &&
                SymbolEqualityComparer.Default.Equals(m.ReturnType, typeSymbol));

        return method is not null;
    }
}
