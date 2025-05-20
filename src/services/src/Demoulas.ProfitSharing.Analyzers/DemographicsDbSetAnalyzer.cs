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
            "Direct access to the Demographics DbSet is prohibited; use IDemographicReaderService.BuildDemographicQuery(...) instead";
        private static readonly LocalizableString _description =
            "Prevents direct calls to the Demographics DbSet to ensure navigation properties (e.g. ContactInfo, Address) are always included via the service.";

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

            // Look for any member‐access expressions like "ctx.Demographics"
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
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
            {return;}

            // Confirm it's the DbSet<Demographic> on IProfitSharingDbContext
            if (propSymbol.Name != "Demographics")
            {
                return;
            }

            var containingType = propSymbol.ContainingType;
            if (!ImplementsIProfitSharingDbContext(containingType))
            { return;}

            // Report diagnostic on the 'Demographics' identifier
            var diagnostic = Diagnostic.Create(_rule, memberAccess.Name.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool ImplementsIProfitSharingDbContext(ITypeSymbol type)
        {
            foreach (var iface in type.AllInterfaces)
            {
                if (iface.Name == "IProfitSharingDbContext")
                {return true;}
            }
            return false;
        }
    }
}
