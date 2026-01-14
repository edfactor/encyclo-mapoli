using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class IsExecutiveAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSMPS003";

    private static readonly DiagnosticDescriptor _rule = new(
        id: DiagnosticId,
        title: "Class must be marked as an Executive",
        messageFormat: "The class '{0}' must be marked with the [NoMemberDataExposed] attribute",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All classes in the Executives namespace must be marked with the [Executive] attribute.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

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

        // We only care about concrete classes – skip interfaces/abstract types.
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

        // Quick exit if the class doesn't derive from any FastEndpoints base type.
        if (!Utils.DerivesFromFastEndpoints(typeSymbol))
        {
            return;
        }

        var responseType = Utils.GetEndpointResponseType(typeSymbol);
        if (responseType is null)
        {
            return; // Non‑generic endpoints such as FastEndpoint.NotImplemented etc.
        }

        ns = responseType.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (!ns.StartsWith("Demoulas.ProfitSharing.Common.Contracts"))
        {
            return; // The response type is not in the Contracts namespace
        }

        if (Utils.HasAnyMemberImplementingInterface(responseType, "Demoulas.ProfitSharing.Common.Interfaces.IIsExecutive"))
        {
            return; // The class already has a property named IsExecutive 
        }

        var attributes = responseType.GetAttributes();
        if (attributes.Any(attr => attr.AttributeClass?.Name == "NoMemberDataExposedAttribute"))
        {
            return; // The class is marked with the [NoMemberDataExposed] attribute.
        }

        //If we're here, the class doesn't have the attribute, nor any property named IsExecutive
        var diagnostic = Diagnostic.Create(
            _rule,
            typeSymbol.Locations[0],
            responseType.ToDisplayString());

        context.ReportDiagnostic(diagnostic);


    }
}
