using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM003 – Detects usage of SSN alone as a dictionary key which will crash at runtime if duplicates exist.
/// Enforces use of composite keys like (Ssn, OracleHcmId) or ToLookup() for SSN-based lookups.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SsnDictionaryKeyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM003";

    private static readonly LocalizableString Title = "SSN used alone as dictionary key";
    private static readonly LocalizableString MessageFormat =
        "Using SSN alone as dictionary key will crash at runtime if duplicates exist. Use composite key (Ssn, Id) or ToLookup() instead.";
    private static readonly LocalizableString Description =
        "SSN is not unique in the Demographics/TotalBalance tables. Using it as a sole dictionary key causes ArgumentException at runtime when duplicates are encountered. Always use a composite key or ToLookup().";

    private const string Category = "Reliability";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning, // Warning for now - existing codebase has violations that need separate cleanup
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        // Check if this is a ToDictionary or ToDictionaryAsync call
        var methodName = GetMethodName(invocation);
        if (methodName is not ("ToDictionary" or "ToDictionaryAsync"))
        {
            return;
        }

        // Get the arguments - we're looking for the key selector lambda
        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0)
        {
            return;
        }

        // Check the first argument (key selector)
        var keySelectorArg = arguments[0].Expression;

        // Get the lambda body - handle both simple and parenthesized lambda expressions
        var lambdaBody = keySelectorArg switch
        {
            SimpleLambdaExpressionSyntax simpleLambda => simpleLambda.Body,
            ParenthesizedLambdaExpressionSyntax parenLambda => parenLambda.Body,
            _ => null
        };

        // Report diagnostic if the lambda body is an SSN-only selector
        if (IsSsnOnlySelector(lambdaBody))
        {
            ReportDiagnostic(context, invocation);
        }
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            _ => null
        };
    }

    private static bool IsSsnOnlySelector(CSharpSyntaxNode? body)
    {
        if (body is null)
        {
            return false;
        }

        // Direct property access: x => x.Ssn
        if (body is MemberAccessExpressionSyntax memberAccess)
        {
            var propertyName = memberAccess.Name.Identifier.Text;
            return propertyName.Equals("Ssn", StringComparison.OrdinalIgnoreCase);
        }

        // Could also be an expression body block
        if (body is BlockSyntax block)
        {
            // Look for return x.Ssn; pattern
            var returnStatement = block.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
            if (returnStatement?.Expression is MemberAccessExpressionSyntax returnMemberAccess)
            {
                var propertyName = returnMemberAccess.Name.Identifier.Text;
                return propertyName.Equals("Ssn", StringComparison.OrdinalIgnoreCase);
            }
        }

        return false;
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
    {
        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
