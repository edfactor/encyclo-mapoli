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
    private static readonly LocalizableString Message = "Record '{0}' contains '{1}' but does not implement interface '{2}'";
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
        context.RegisterSyntaxNodeAction(AnalyzeRecord, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeRecord(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not RecordDeclarationSyntax recordDecl)
        {
            return;
        }

        var name = recordDecl.Identifier.Text;
        if (!(name.EndsWith("Request") || name.EndsWith("Response")))
        {
            return;
        }

        var ns = recordDecl.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString()
                 ?? recordDecl.FirstAncestorOrSelf<FileScopedNamespaceDeclarationSyntax>()?.Name.ToString();
        if (ns is null || (!ns.Contains("Contracts.Request") && !ns.Contains("Contracts.Response")))
        {
            return;
        }

        var props = new HashSet<string>(recordDecl.Members.OfType<PropertyDeclarationSyntax>().Select(p => p.Identifier.Text));
        var hasNameParts = props.Contains("FirstName") || props.Contains("LastName") || props.Contains("MiddleName");
        var hasFullName = props.Contains("FullName");
        var hasPhone = props.Contains("PhoneNumber");
        var hasEmail = props.Contains("EmailAddress");
        var hasCity = props.Contains("City");

        if (!(hasNameParts || hasFullName || hasPhone || hasEmail || hasCity))
        {
            return;
        }

        var implemented = recordDecl.BaseList != null
            ? new HashSet<string>(recordDecl.BaseList.Types.Select(t => t.Type.ToString()))
            : new HashSet<string>();

        void ReportIf(bool condition, string propertyExample, string requiredInterface)
        {
            if (condition && !implemented.Contains(requiredInterface))
            {
                var diagnostic = Diagnostic.Create(Rule, recordDecl.Identifier.GetLocation(), name, propertyExample, requiredInterface);
                context.ReportDiagnostic(diagnostic);
            }
        }

        ReportIf(hasNameParts, "FirstName/LastName", "INameParts");
        ReportIf(hasFullName, "FullName", "IFullNameProperty");
        ReportIf(hasPhone, "PhoneNumber", "IPhoneNumber");
        ReportIf(hasEmail, "EmailAddress", "IEmailAddress");
        ReportIf(hasCity, "City", "ICity");
    }
}
