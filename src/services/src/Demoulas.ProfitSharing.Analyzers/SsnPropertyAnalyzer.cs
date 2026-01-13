using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SsnPropertyAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "DSM001";
    private static readonly LocalizableString _title = "Ssn property must be a string";
    private static readonly LocalizableString _messageFormat = "The property '{0}' must be of type 'string'";
    private static readonly LocalizableString _description = "Ensures the 'Ssn' property is defined as a string.";
    private const string Category = "Security";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        _title,
        _messageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: _description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register a syntax node action to analyze properties.
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        // Check if the node is a property declaration.
        if (context.Node is not PropertyDeclarationSyntax propertyDeclaration)
        {
            return;
        }

        // Check if the property name is "Ssn".
        if (propertyDeclaration.Identifier.Text != "Ssn")
        {
            return;
        }

        // Get the type of the property.
        var typeInfo = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type);

        // Check if the property type is string.
        if (typeInfo.Type?.SpecialType != SpecialType.System_String)
        {
            var diagnostic = Diagnostic.Create(
                _rule,
                propertyDeclaration.GetLocation(),
                propertyDeclaration.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }
    }
}

