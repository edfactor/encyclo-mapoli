using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM004 – Detects manual name concatenation fallback patterns like: FullName ?? $"{LastName}, {FirstName}"
/// Enforces use of FullName property only without manual concatenation fallbacks.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NameConcatenationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM004";

    private static readonly LocalizableString Title = "Manual name concatenation fallback detected";
    private static readonly LocalizableString MessageFormat =
        "Do not use manual name concatenation fallback. Use FullName property only without fallback to '{0}'.";
    private static readonly LocalizableString Description =
        "Manual name concatenation like 'FullName ?? $\"{LastName}, {FirstName}\"' creates inconsistent formatting. " +
        "Always use the pre-computed FullName property. If FullName is null, investigate the data source rather than falling back to manual concatenation.";

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    // Name-related identifiers to look for in concatenation patterns
    private static readonly HashSet<string> NameIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "FirstName", "LastName", "MiddleName", "Name", "FullName"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Check null-coalescing expressions: x ?? y
        context.RegisterSyntaxNodeAction(AnalyzeCoalesceExpression, SyntaxKind.CoalesceExpression);

        // Check ternary expressions: x != null ? x : y
        context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
    }

    private static void AnalyzeCoalesceExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not BinaryExpressionSyntax coalesce)
        {
            return;
        }

        // Check if left side references FullName and right side is a name concatenation pattern
        if (ReferencesNameProperty(coalesce.Left, "FullName")
            && IsNameConcatenationPattern(coalesce.Right, out var patternDescription))
        {
            var diagnostic = Diagnostic.Create(Rule, coalesce.GetLocation(), patternDescription);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ConditionalExpressionSyntax conditional)
        {
            return;
        }

        var whenTrue = conditional.WhenTrue;
        var whenFalse = conditional.WhenFalse;

        // Pattern: condition ? FullName : concatenation
        if (ReferencesNameProperty(whenTrue, "FullName")
            && IsNameConcatenationPattern(whenFalse, out var pattern1))
        {
            var diagnostic = Diagnostic.Create(Rule, conditional.GetLocation(), pattern1);
            context.ReportDiagnostic(diagnostic);
            return;
        }

        // Pattern: condition ? concatenation : FullName
        if (ReferencesNameProperty(whenFalse, "FullName")
            && IsNameConcatenationPattern(whenTrue, out var pattern2))
        {
            var diagnostic = Diagnostic.Create(Rule, conditional.GetLocation(), pattern2);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool ReferencesNameProperty(ExpressionSyntax expression, string propertyName)
    {
        return expression switch
        {
            MemberAccessExpressionSyntax memberAccess =>
                memberAccess.Name.Identifier.Text.Equals(propertyName, StringComparison.OrdinalIgnoreCase),
            IdentifierNameSyntax identifier =>
                identifier.Identifier.Text.Equals(propertyName, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static bool IsNameConcatenationPattern(ExpressionSyntax expression, out string patternDescription)
    {
        patternDescription = string.Empty;

        switch (expression)
        {
            // Check interpolated strings: $"{LastName}, {FirstName}"
            case InterpolatedStringExpressionSyntax interpolated when ContainsNameInterpolations(interpolated):
                patternDescription = expression.ToString();
                return true;

            // Check string concatenation: LastName + ", " + FirstName
            case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AddExpression) && ContainsNameConcatenation(binary):
                patternDescription = expression.ToString();
                return true;

            // Check string.Format calls
            case InvocationExpressionSyntax invocation when IsStringFormatWithNames(invocation):
                patternDescription = expression.ToString();
                return true;

            // Check .Trim() on concatenation
            case InvocationExpressionSyntax trimInvocation when IsTrimOnNameConcatenation(trimInvocation, out patternDescription):
                return true;

            // Check parenthesized expression
            case ParenthesizedExpressionSyntax parenthesized:
                return IsNameConcatenationPattern(parenthesized.Expression, out patternDescription);

            default:
                return false;
        }
    }

    private static bool IsTrimOnNameConcatenation(InvocationExpressionSyntax invocation, out string patternDescription)
    {
        patternDescription = string.Empty;

        if (invocation.Expression is MemberAccessExpressionSyntax trimMemberAccess
            && trimMemberAccess.Name.Identifier.Text == "Trim"
            && IsNameConcatenationPattern(trimMemberAccess.Expression, out _))
        {
            patternDescription = invocation.ToString();
            return true;
        }

        return false;
    }

    private static bool ContainsNameInterpolations(InterpolatedStringExpressionSyntax interpolated)
    {
        var nameCount = interpolated.Contents
            .OfType<InterpolationSyntax>()
            .Select(interp => interp.Expression)
            .Count(IsNameIdentifier);

        // If we found 2+ name-related interpolations, it's likely a name concatenation pattern
        return nameCount >= 2;
    }

    private static bool IsNameIdentifier(ExpressionSyntax expression)
    {
        return expression switch
        {
            MemberAccessExpressionSyntax memberAccess =>
                NameIdentifiers.Contains(memberAccess.Name.Identifier.Text),
            IdentifierNameSyntax identifier =>
                NameIdentifiers.Contains(identifier.Identifier.Text),
            _ => false
        };
    }

    private static bool ContainsNameConcatenation(BinaryExpressionSyntax binary)
    {
        var nameReferences = 0;
        CollectNameReferences(binary, ref nameReferences);
        return nameReferences >= 2;
    }

    private static void CollectNameReferences(ExpressionSyntax expression, ref int count)
    {
        switch (expression)
        {
            case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AddExpression):
                CollectNameReferences(binary.Left, ref count);
                CollectNameReferences(binary.Right, ref count);
                break;

            case MemberAccessExpressionSyntax memberAccess when NameIdentifiers.Contains(memberAccess.Name.Identifier.Text):
                count++;
                break;

            case IdentifierNameSyntax identifier when NameIdentifiers.Contains(identifier.Identifier.Text):
                count++;
                break;

            case ParenthesizedExpressionSyntax parenthesized:
                CollectNameReferences(parenthesized.Expression, ref count);
                break;
        }
    }

    private static bool IsStringFormatWithNames(InvocationExpressionSyntax invocation)
    {
        // Check for string.Format("{0}, {1}", LastName, FirstName) patterns
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name.Identifier.Text != "Format")
        {
            return false;
        }

        var nameCount = invocation.ArgumentList.Arguments
            .Skip(1) // Skip format string
            .Select(arg => arg.Expression)
            .Count(IsNameIdentifier);

        return nameCount >= 2;
    }
}
