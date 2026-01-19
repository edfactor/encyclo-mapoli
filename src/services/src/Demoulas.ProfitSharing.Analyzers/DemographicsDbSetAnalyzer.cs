using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DemographicsDbSetAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DSMPS001";
        private const string Category = "Usage";

        private static readonly LocalizableString _title = "Use IDemographicReaderService for Demographic queries";
        private static readonly LocalizableString _messageFormat =
            "Direct access to the Demographics DbSet or FrozenService.GetDemographicSnapshot is prohibited; use IDemographicReaderService.BuildDemographicQueryAsync(...) instead";
        private static readonly LocalizableString _description =
            "Prevents direct calls to the Demographics DbSet or FrozenService.GetDemographicSnapshot to ensure navigation properties (e.g. ContactInfo, Address) are always included via the service.";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId,
            _title,
            _messageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: _description
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Look for any member‐access expressions like "ctx.Demographics" or "FrozenService.GetDemographicSnapshot"
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            // Only interested in ".Demographics"
            if (memberAccess.Name.Identifier.Text != "Demographics")
            {
                return;
            }

            // Get the symbol for the property
            if (context.SemanticModel.GetSymbolInfo(memberAccess).Symbol is not IPropertySymbol propSymbol)
            { return; }

            // Confirm it's the DbSet<Demographic> on IProfitSharingDbContext
            if (propSymbol.Name != "Demographics")
            {
                return;
            }

            // Report diagnostic on any usage of Demographics, even if it's part of a larger expression
            // (e.g., ctx.Demographics.Select(...), ctx.Demographics.Where(...), etc.)
            // This will flag the 'Demographics' identifier in all such cases.
            var diagnostic = Diagnostic.Create(_rule, memberAccess.Name.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            {
                return;
            }

            // Look for FrozenService.GetDemographicSnapshot(...)
            if (memberAccess.Name.Identifier.Text != "GetDemographicSnapshot")
            {
                return;
            }

            // Check that the expression is FrozenService
            if (memberAccess.Expression is not IdentifierNameSyntax identifier)
            {
                return;
            }
            if (identifier.Identifier.Text != "FrozenService")
            {
                return;
            }

            // Get the symbol for the method
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (symbol == null)
            {
                return;
            }
            if (symbol.Name != "GetDemographicSnapshot")
            {
                return;
            }
            if (symbol.ContainingType.Name != "FrozenService")
            {
                return;
            }

            // Report diagnostic on the method name
            var diagnostic = Diagnostic.Create(_rule, memberAccess.Name.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
