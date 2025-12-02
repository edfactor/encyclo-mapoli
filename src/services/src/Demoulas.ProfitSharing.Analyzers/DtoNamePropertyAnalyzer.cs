using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM005 – Detects use of non-standard person name properties in employee/member/beneficiary DTOs.
/// Enforces use of FullName (or FirstName/LastName/MiddleName) for person-related properties.
/// Only applies to DTOs that represent people (employees, members, beneficiaries, demographics).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DtoNamePropertyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM005";

    private static readonly LocalizableString Title = "Use 'FullName' instead of non-standard person name properties";
    private static readonly LocalizableString MessageFormat =
        "Property '{0}' in person-related DTO '{1}' should be named 'FullName' for consistency. Non-standard name properties create formatting inconsistencies.";
    private static readonly LocalizableString Description =
        "Person-related DTOs should use standardized name properties: FirstName, LastName, MiddleName, or FullName. " +
        "Non-standard properties like 'EmployeeName', 'MemberName', or 'PersonName' should be avoided as they create formatting inconsistencies.";

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    // Property names that indicate a person's name but use non-standard naming
    // Note: "Name" alone is NOT included - it's too generic and used for many legitimate purposes
    // (e.g., DepartmentName, BeneficiaryTypeName, etc.)
    private static readonly HashSet<string> NonStandardPersonNameProperties = new(StringComparer.Ordinal)
    {
        "EmployeeName",   // Should be FullName
        "MemberName",     // Should be FullName
        "PersonName",     // Should be FullName
        "ParticipantName" // Should be FullName
    };

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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Analyze class and record declarations
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeDecl)
        {
            return;
        }

        var typeName = typeDecl.Identifier.Text;

        // Only check person-related DTOs
        if (!IsPersonRelatedDto(typeName))
        {
            return;
        }

        // Check all properties in the type
        foreach (var member in typeDecl.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                AnalyzeProperty(context, property, typeName);
            }
        }

        // Also check primary constructor parameters for records
        if (typeDecl is RecordDeclarationSyntax recordDecl && recordDecl.ParameterList is not null)
        {
            AnalyzeRecordParameters(context, recordDecl.ParameterList, typeName);
        }
    }

    private static void AnalyzeRecordParameters(
        SyntaxNodeAnalysisContext context,
        ParameterListSyntax parameterList,
        string typeName)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            var paramName = parameter.Identifier.Text;
            if (NonStandardPersonNameProperties.Contains(paramName)
                && IsStringType(context, parameter.Type))
            {
                var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), paramName, typeName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void AnalyzeProperty(
        SyntaxNodeAnalysisContext context,
        PropertyDeclarationSyntax property,
        string typeName)
    {
        var propertyName = property.Identifier.Text;

        // Only flag non-standard person name properties
        if (!NonStandardPersonNameProperties.Contains(propertyName))
        {
            return;
        }

        // Only flag string properties (name properties should be strings)
        if (!IsStringType(context, property.Type))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, property.Identifier.GetLocation(), propertyName, typeName);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsPersonRelatedDto(string typeName)
    {
        // Check if the type name contains any person-related indicators
        return PersonTypeIndicators.Any(indicator =>
            typeName.IndexOf(indicator, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static bool IsStringType(SyntaxNodeAnalysisContext context, TypeSyntax? typeSyntax)
    {
        if (typeSyntax is null)
        {
            return false;
        }

        // Handle nullable types
        if (typeSyntax is NullableTypeSyntax nullable)
        {
            typeSyntax = nullable.ElementType;
        }

        // Check predefined type (string keyword)
        if (typeSyntax is PredefinedTypeSyntax predefined)
        {
            return predefined.Keyword.IsKind(SyntaxKind.StringKeyword);
        }

        // Check identifier (String class name)
        if (typeSyntax is IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.Text == "String";
        }

        // Check qualified name (System.String)
        if (typeSyntax is QualifiedNameSyntax qualified)
        {
            return qualified.ToString() == "System.String";
        }

        // Use semantic model for more complex cases
        var typeInfo = context.SemanticModel.GetTypeInfo(typeSyntax);
        return typeInfo.Type?.SpecialType == SpecialType.System_String;
    }
}
