using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NameInterfaceAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "DSM002";
    private static readonly LocalizableString Title = "DTO with name/contact properties should implement DRY interfaces";
    private static readonly LocalizableString Message = "Type '{0}' contains '{1}' but does not implement interface '{2}'";
    private static readonly LocalizableString Description = "Enforces implementation of INameParts/IFullNameProperty/IPhoneNumber/IEmailAddress/ICity on request/response DTOs.";
    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        Message,
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.RecordDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeDecl)
        {
            return;
        }

        var name = typeDecl.Identifier.Text;
        if (!IsRelevantContractDtoTypeName(name))
        {
            return;
        }

        var ns = typeDecl.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString()
                 ?? typeDecl.FirstAncestorOrSelf<FileScopedNamespaceDeclarationSyntax>()?.Name.ToString();
        if (ns is null || (!ns.Contains("Contracts.Request") && !ns.Contains("Contracts.Response")))
        {
            return;
        }

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken);
        if (typeSymbol is null)
        {
            return;
        }

        var propertyNames = new HashSet<string>(
            typeSymbol.GetMembers().OfType<IPropertySymbol>().Select(p => p.Name),
            StringComparer.Ordinal);

        var hasNameParts = propertyNames.Contains("FirstName") || propertyNames.Contains("LastName") || propertyNames.Contains("MiddleName");
        var hasFullName = propertyNames.Contains("FullName");
        var hasPhone = propertyNames.Contains("PhoneNumber");
        var hasEmail = propertyNames.Contains("EmailAddress");
        var hasCity = propertyNames.Contains("City");

        if (!(hasNameParts || hasFullName || hasPhone || hasEmail || hasCity))
        {
            return;
        }

        var implemented = new HashSet<string>(
            typeSymbol.AllInterfaces.Select(i => i.Name),
            StringComparer.Ordinal);

        void ReportIf(bool condition, string propertyExample, string requiredInterface)
        {
            if (condition && !implemented.Contains(requiredInterface))
            {
                var diagnostic = Diagnostic.Create(Rule, typeDecl.Identifier.GetLocation(), name, propertyExample, requiredInterface);
                context.ReportDiagnostic(diagnostic);
            }
        }

        ReportIf(hasNameParts, "FirstName/LastName", "INameParts");
        ReportIf(hasFullName, "FullName", "IFullNameProperty");
        ReportIf(hasPhone, "PhoneNumber", "IPhoneNumber");
        ReportIf(hasEmail, "EmailAddress", "IEmailAddress");
        ReportIf(hasCity, "City", "ICity");
    }

    private static bool IsRelevantContractDtoTypeName(string name)
    {
        return name.EndsWith("Request", StringComparison.Ordinal)
               || name.EndsWith("Response", StringComparison.Ordinal)
               || name.EndsWith("Dto", StringComparison.Ordinal)
               || name.EndsWith("DetailDto", StringComparison.Ordinal);
    }
}
