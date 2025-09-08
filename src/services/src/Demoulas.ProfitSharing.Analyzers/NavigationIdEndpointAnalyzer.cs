using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSMPS010 – Ensure every FastEndpoints endpoint implements IHasNavigationId.
/// Triggers when a class derives from FastEndpoints.Endpoint*, but does not implement
/// Demoulas.ProfitSharing.Endpoints.Base.IHasNavigationId (directly or via a base class).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NavigationIdEndpointAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSMPS010";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Endpoints must implement IHasNavigationId",
        messageFormat: "Endpoint '{0}' derives from FastEndpoints but does not implement IHasNavigationId",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All endpoints must carry a NavigationId via IHasNavigationId. Inherit from ProfitSharingEndpoint<> base types or implement the interface explicitly.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        // Only concrete classes
        if (typeSymbol.TypeKind != TypeKind.Class || typeSymbol.IsAbstract)
        {
            return;
        }

        // Only consider types in the Endpoints assembly/namespaces to reduce noise
    var ns = typeSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
    if (!ns.Contains("Demoulas.ProfitSharing.Endpoints"))
        {
            return;
        }

        // Must derive from a FastEndpoints base type
        if (!DerivesFromFastEndpoints(typeSymbol))
        {
            return;
        }

        // Check for IHasNavigationId
        var iHasNav = context.Compilation.GetTypeByMetadataName("Demoulas.ProfitSharing.Endpoints.Base.IHasNavigationId");
        if (iHasNav is null)
        {
            return; // interface not available
        }

        if (Implements(typeSymbol, iHasNav))
        {
            return; // compliant
        }

        // Report diagnostic on class declaration
        var location = typeSymbol.Locations.FirstOrDefault();
        if (location is not null)
        {
            var diag = Diagnostic.Create(Rule, location, typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            context.ReportDiagnostic(diag);
        }
    }

    private static bool DerivesFromFastEndpoints(INamedTypeSymbol type)
    {
        for (var cur = type; cur is not null; cur = cur.BaseType)
        {
            var constructed = cur.ConstructedFrom?.ToDisplayString();
            if (constructed is "FastEndpoints.Endpoint<TRequest, TResponse>"
                or "FastEndpoints.Endpoint<>"
                or "FastEndpoints.EndpointWithoutRequest<TResponse>"
                or "FastEndpoints.EndpointWithoutRequest")
            {
                return true;
            }
        }

        return false;
    }

    private static bool Implements(INamedTypeSymbol type, INamedTypeSymbol iface)
    {
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iface));
    }
}
