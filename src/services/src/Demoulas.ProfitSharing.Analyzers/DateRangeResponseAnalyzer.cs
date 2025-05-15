using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers
{
    /// <summary>
    /// PS002 – Ensures every FastEndpoints endpoint declares a response type that implements <see cref="IHasDateRange"/>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DateRangeResponseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DSMPS002";

        private static readonly DiagnosticDescriptor _rule = new(
            id: DiagnosticId,
            title: "Endpoint response must expose a date range (StartDate / EndDate)",
            messageFormat: "The response type '{0}' for endpoint '{1}' does not implement IHasDateRange",
            category: "Architecture",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description:
            "Every FastEndpoints endpoint must return a response type that implements IHasDateRange so that consumers always know the covered date range.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

        public override void Initialize(AnalysisContext context)
        {
            // We only need compilation-wide symbols, no syntax trees, so non‑configurable.
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

            // We only care about concrete classes – skip interfaces/abstract types.
            if (typeSymbol.TypeKind != TypeKind.Class || typeSymbol.IsAbstract)
            {
                return;
            }

            // Quick exit if the class doesn't derive from any FastEndpoints base type.
            if (!IsFastEndpoint(typeSymbol))
            {
                return;
            }

            // FastEndpoints<TReq, TRes> – get response type argument (index 1).
            var responseType = GetEndpointResponseType(typeSymbol);
            if (responseType is null)
            {
                return; // Non‑generic endpoints such as FastEndpoint.NotImplemented etc.
            }

            // Look up IHasDateRange in the compilation.
            var hasDateRangeSymbol = context.Compilation.GetTypeByMetadataName("IHasDateRange");
            if (hasDateRangeSymbol is null)
            {
                return; // Interface not found; leave to other analyzers.
            }

            // If response doesn't implement the interface, raise PS002.
            if (!Implements(responseType, hasDateRangeSymbol))
            {
                var diagnostic = Diagnostic.Create(
                    _rule,
                    typeSymbol.Locations[0],
                    responseType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

                context.ReportDiagnostic(diagnostic);
            }
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────────

        private static bool IsFastEndpoint(INamedTypeSymbol symbol)
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

        private static INamedTypeSymbol? GetEndpointResponseType(INamedTypeSymbol endpointSymbol)
        {
            for (INamedTypeSymbol? current = endpointSymbol; current is not null; current = current.BaseType)
            {
                var constructed = current.ConstructedFrom?.ToDisplayString();
                if (constructed is "FastEndpoints.Endpoint<,>" && current.TypeArguments.Length == 2)
                {
                    return current.TypeArguments[1] as INamedTypeSymbol;
                }

                if (constructed is "FastEndpoints.EndpointWithoutRequest<>" && current.TypeArguments.Length == 1)
                {
                    return current.TypeArguments[0] as INamedTypeSymbol;
                }
            }

            return null;
        }

        private static bool Implements(ITypeSymbol type, INamedTypeSymbol iface)
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
    }
}
