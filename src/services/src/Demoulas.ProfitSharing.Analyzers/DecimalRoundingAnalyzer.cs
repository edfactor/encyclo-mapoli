using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

/// <summary>
/// DSM006 – Detects Math.Round calls without MidpointRounding.AwayFromZero.
/// Enforces use of AwayFromZero rounding mode for financial calculations to match COBOL behavior.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DecimalRoundingAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM006";

    private static readonly LocalizableString Title = "Math.Round must use MidpointRounding.AwayFromZero";
    private static readonly LocalizableString MessageFormat =
        "Use MidpointRounding.AwayFromZero for financial calculations to match COBOL behavior. Replace with: Math.Round({0}, {1}, MidpointRounding.AwayFromZero).";
    private static readonly LocalizableString Description =
        "Financial calculations must use MidpointRounding.AwayFromZero (0.5 rounds up) to match COBOL's traditional rounding. " +
        ".NET's default Math.Round uses MidpointRounding.ToEven (banker's rounding, 0.5 rounds to nearest even), which causes penny differences in financial reports. " +
        "Apply this to tax calculations, distribution amounts, forfeiture amounts, and any monetary aggregation or reporting.";

    private const string Category = "Reliability";

#pragma warning disable RS2000 // Add analyzer release tracking support
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);
#pragma warning restore RS2000

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

        // Check if this is a Math.Round call
        var methodName = GetMethodName(invocation);
        if (methodName != "Round")
        {
            return;
        }

        // Verify it's Math.Round specifically (not a custom Round method)
        if (!IsMathRound(invocation))
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;

        // Math.Round(value, decimals) - 2 args, no mode parameter
        if (arguments.Count == 2)
        {
            // This is the problematic pattern - no rounding mode specified
            ReportDiagnostic(context, invocation, arguments[0].Expression.ToString(), arguments[1].Expression.ToString());
            return;
        }

        // Math.Round(value, decimals, mode) - 3 args
        if (arguments.Count == 3 && !IsMidpointRoundingAwayFromZero(arguments[2].Expression))
        {
            // It's using a different rounding mode
            ReportDiagnostic(context, invocation, arguments[0].Expression.ToString(), arguments[1].Expression.ToString());
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

    private static bool IsMathRound(InvocationExpressionSyntax invocation)
    {
        // Check if it's Math.Round (not just any Round method)
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is IdentifierNameSyntax identifier &&
            identifier.Identifier.Text == "Math")
        {
            return true;
        }

        return false;
    }

    private static bool IsMidpointRoundingAwayFromZero(ExpressionSyntax? expression)
    {
        if (expression is null)
        {
            return false;
        }

        // Check for MidpointRounding.AwayFromZero pattern
        if (expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is IdentifierNameSyntax identifier &&
            identifier.Identifier.Text == "MidpointRounding" &&
            memberAccess.Name.Identifier.Text == "AwayFromZero")
        {
            return true;
        }

        // Also handle fully qualified: System.MidpointRounding.AwayFromZero
        if (expression is MemberAccessExpressionSyntax fullyQualified &&
            fullyQualified.Expression is MemberAccessExpressionSyntax parent &&
            parent.Expression is IdentifierNameSyntax sysId &&
            sysId.Identifier.Text == "System" &&
            parent.Name.Identifier.Text == "MidpointRounding" &&
            fullyQualified.Name.Identifier.Text == "AwayFromZero")
        {
            return true;
        }

        return false;
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, string valueArg, string decimalsArg)
    {
        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), valueArg, decimalsArg);
        context.ReportDiagnostic(diagnostic);
    }
}

