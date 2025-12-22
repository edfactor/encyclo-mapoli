using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM010 – Detects usage of CommentType.Name in business logic (comparisons, switches, etc.).
/// Since CommentType.Name is user-editable, business logic must use CommentType.Id instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CommentTypeNameBusinessLogicAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM010";

    private static readonly LocalizableString Title = "CommentType.Name used in business logic";
    private static readonly LocalizableString MessageFormat =
        "Do not use CommentType.Name for business logic. Names are user-editable and will break logic. Use CommentType.Id instead.";
    private static readonly LocalizableString Description =
        "CommentType.Name is editable by users, so business logic using name-based comparisons (if/switch/Contains) will fail when names change. " +
        "Always use CommentType.Id for business logic. Display purposes (logging, UI) are allowed.";

    private const string Category = "Reliability";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error, // Error - this will cause runtime bugs
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register analyzers for different patterns
        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeSwitchExpression, SyntaxKind.SwitchExpression);
        context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not BinaryExpressionSyntax binaryExpr)
        {
            return;
        }

        // Check left side: commentType.Name == "Transfer Out"
        if (IsCommentTypeNameAccess(binaryExpr.Left) && IsStringLiteral(binaryExpr.Right))
        {
            ReportDiagnostic(context, binaryExpr);
            return;
        }

        // Check right side: "Transfer Out" == commentType.Name
        if (IsStringLiteral(binaryExpr.Left) && IsCommentTypeNameAccess(binaryExpr.Right))
        {
            ReportDiagnostic(context, binaryExpr);
            return;
        }
    }

    private static void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not SwitchExpressionSyntax switchExpr)
        {
            return;
        }

        // Check if switch is on CommentType.Name
        if (IsCommentTypeNameAccess(switchExpr.GoverningExpression))
        {
            ReportDiagnostic(context, switchExpr);
        }
    }

    private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not SwitchStatementSyntax switchStmt)
        {
            return;
        }

        // Check if switch is on CommentType.Name
        if (IsCommentTypeNameAccess(switchStmt.Expression))
        {
            ReportDiagnostic(context, switchStmt);
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        // Check for string methods on CommentType.Name: commentType.Name.Contains("Transfer")
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.Text;

            // String comparison methods that indicate business logic
            if (methodName is "Contains" or "StartsWith" or "EndsWith" or "Equals" &&
                IsCommentTypeNameAccess(memberAccess.Expression))
            {
                ReportDiagnostic(context, invocation);
            }

            // Also check for String.Equals(commentType.Name, "something")
            if (methodName is "Equals" &&
                memberAccess.Expression is IdentifierNameSyntax { Identifier.Text: "String" })
            {
                var arguments = invocation.ArgumentList.Arguments;
                if (arguments.Count >= 2 &&
                    (IsCommentTypeNameAccess(arguments[0].Expression) ||
                     IsCommentTypeNameAccess(arguments[1].Expression)))
                {
                    ReportDiagnostic(context, invocation);
                }
            }
        }
    }

    private static bool IsCommentTypeNameAccess(SyntaxNode? node)
    {
        if (node is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var propertyName = memberAccess.Name.Identifier.Text;
        if (!propertyName.Equals("Name", StringComparison.Ordinal))
        {
            return false;
        }

        // Check if the member is accessed on a CommentType-like identifier
        // Handle various patterns:
        // - commentType.Name
        // - record.CommentType.Name
        // - item.CommentType.Name
        var currentNode = memberAccess.Expression;
        while (currentNode is MemberAccessExpressionSyntax innerMemberAccess)
        {
            if (innerMemberAccess.Name.Identifier.Text.Contains("CommentType", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            currentNode = innerMemberAccess.Expression;
        }

        // Direct access: commentType.Name
        if (currentNode is IdentifierNameSyntax identifier)
        {
            var identifierName = identifier.Identifier.Text;
            if (identifierName.Contains("commentType", StringComparison.OrdinalIgnoreCase) ||
                identifierName.Contains("comment", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStringLiteral(SyntaxNode? node)
    {
        return node is LiteralExpressionSyntax { RawKind: (int)SyntaxKind.StringLiteralExpression };
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
    {
        var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
