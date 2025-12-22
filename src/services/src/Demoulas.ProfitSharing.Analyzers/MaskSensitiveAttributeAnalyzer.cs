using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MaskSensitiveAttributeAnalyzer : DiagnosticAnalyzer
{
    private const string BadgeDiagnosticId = "DSM009";
    private const string RequiredDiagnosticId = "DSM007";

    private static readonly LocalizableString BadgeTitle = "BadgeNumber should not be marked as sensitive";
    private static readonly LocalizableString BadgeMessage =
        "Property '{0}' in DTO '{1}' should not be annotated with [MaskSensitive] (badge numbers are not treated as sensitive)";

    private static readonly LocalizableString RequiredTitle = "Sensitive person fields must be masked";
    private static readonly LocalizableString RequiredMessage =
        "Property '{0}' in DTO '{1}' should be annotated with [MaskSensitive] to enforce PII masking";

    private const string Category = "Security";

    private static readonly DiagnosticDescriptor BadgeRule = new(
        BadgeDiagnosticId,
        BadgeTitle,
        BadgeMessage,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RequiredRule = new(
        RequiredDiagnosticId,
        RequiredTitle,
        RequiredMessage,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // Types that represent people (case-insensitive substring match)
    private static readonly string[] PersonTypeIndicators =
    {
        "Employee",
        "Member",
        "Beneficiary",
        "Demographic",
        "Participant",
        "Person",
        "Profile",
        "Worker"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(BadgeRule, RequiredRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not PropertyDeclarationSyntax propertyDecl)
        {
            return;
        }

        var propertyName = propertyDecl.Identifier.Text;
        if (propertyName is not ("BadgeNumber" or "Age" or "DateOfBirth"))
        {
            return;
        }

        var containingType = propertyDecl.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (containingType is null)
        {
            return;
        }

        if (!IsContractsResponseDto(containingType))
        {
            return;
        }

        var typeName = containingType.Identifier.Text;
        if (!IsPersonRelatedDto(typeName))
        {
            return;
        }

        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDecl, context.CancellationToken) as IPropertySymbol;
        if (propertySymbol is null)
        {
            return;
        }

        var hasMaskSensitive = HasMaskSensitiveAttribute(propertySymbol);

        if (propertyName == "BadgeNumber")
        {
            if (hasMaskSensitive)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    BadgeRule,
                    propertyDecl.Identifier.GetLocation(),
                    propertyName,
                    typeName));
            }

            return;
        }

        // Age and DateOfBirth are expected to be masked unless the whole DTO is already masked.
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(containingType, context.CancellationToken) as INamedTypeSymbol;
        var typeHasMaskSensitive = typeSymbol is not null && HasMaskSensitiveAttribute(typeSymbol);

        if (!hasMaskSensitive && !typeHasMaskSensitive)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                RequiredRule,
                propertyDecl.Identifier.GetLocation(),
                propertyName,
                typeName));
        }
    }

    private static bool IsPersonRelatedDto(string typeName)
    {
        return PersonTypeIndicators.Any(indicator =>
            typeName.IndexOf(indicator, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static bool IsContractsResponseDto(TypeDeclarationSyntax typeDecl)
    {
        var ns = typeDecl.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString()
                 ?? typeDecl.FirstAncestorOrSelf<FileScopedNamespaceDeclarationSyntax>()?.Name.ToString();

        return ns is not null && ns.Contains("Contracts.Response", StringComparison.Ordinal);
    }

    private static bool HasMaskSensitiveAttribute(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name is "MaskSensitiveAttribute" or "MaskSensitive");
    }
}
