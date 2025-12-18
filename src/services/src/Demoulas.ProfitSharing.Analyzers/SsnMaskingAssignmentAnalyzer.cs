using System.Collections.Immutable;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demoulas.ProfitSharing.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SsnMaskingAssignmentAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DSM008";

    private static readonly LocalizableString Title = "SSN must be masked when mapped to response DTO";
    private static readonly LocalizableString Message =
        "Assignment to '{0}.Ssn' should use MaskSsn() to prevent returning unmasked SSNs in response DTOs";

    private const string Category = "Security";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        Message,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeNamedArgument, SyntaxKind.Argument);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AssignmentExpressionSyntax assignment)
        {
            return;
        }

        // Ignore assignments within the Contracts.Response DTO declarations themselves (e.g., ResponseExample()).
        // This analyzer is intended to enforce masking at mapping sites (services/endpoints), not inside DTO definition files.
        if (IsWithinContractsResponseDeclaration(assignment))
        {
            return;
        }

        var leftSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol;
        if (leftSymbol is not IPropertySymbol { Name: "Ssn" })
        {
            return;
        }

        var responseDtoType = GetEnclosingResponseDtoTypeSymbol(context.SemanticModel, assignment, context.CancellationToken);
        if (responseDtoType is null)
        {
            return;
        }

        if (ContainsMaskSsnInvocation(context.SemanticModel, assignment.Right, context.CancellationToken))
        {
            return;
        }

        if (IsExplicitlyMaskedSsnLiteral(assignment.Right))
        {
            return;
        }

        // Allow explicit null assignment
        if (assignment.Right.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            assignment.GetLocation(),
            responseDtoType.Name));
    }

    private static void AnalyzeNamedArgument(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ArgumentSyntax argument)
        {
            return;
        }

        // Ignore assignments within the Contracts.Response DTO declarations themselves (e.g., ResponseExample()).
        if (IsWithinContractsResponseDeclaration(argument))
        {
            return;
        }

        if (argument.NameColon?.Name.Identifier.Text != "Ssn")
        {
            return;
        }

        var objectCreation = argument.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation is null)
        {
            return;
        }

        var typeInfo = context.SemanticModel.GetTypeInfo(objectCreation, context.CancellationToken).Type as INamedTypeSymbol;
        if (!IsContractsResponseDto(typeInfo))
        {
            return;
        }

        if (argument.Expression is null)
        {
            return;
        }

        if (ContainsMaskSsnInvocation(context.SemanticModel, argument.Expression, context.CancellationToken))
        {
            return;
        }

        if (IsExplicitlyMaskedSsnLiteral(argument.Expression))
        {
            return;
        }

        if (argument.Expression.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            argument.GetLocation(),
            typeInfo!.Name));
    }

    private static INamedTypeSymbol? GetEnclosingResponseDtoTypeSymbol(
        SemanticModel semanticModel,
        SyntaxNode node,
        CancellationToken cancellationToken)
    {
        // object initializer: new T { Ssn = ... }
        var objectCreation = node.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation is not null)
        {
            var typeInfo = semanticModel.GetTypeInfo(objectCreation, cancellationToken).Type as INamedTypeSymbol;
            return IsContractsResponseDto(typeInfo) ? typeInfo : null;
        }

        // record with-expression: dto with { Ssn = ... }
        var withExpression = node.FirstAncestorOrSelf<WithExpressionSyntax>();
        if (withExpression is not null)
        {
            var typeInfo = semanticModel.GetTypeInfo(withExpression, cancellationToken).Type as INamedTypeSymbol;
            return IsContractsResponseDto(typeInfo) ? typeInfo : null;
        }

        return null;
    }

    private static bool IsContractsResponseDto(INamedTypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        var ns = typeSymbol.ContainingNamespace.ToDisplayString();
        return ns.Contains("Contracts.Response", StringComparison.Ordinal);
    }

    private static bool IsWithinContractsResponseDeclaration(SyntaxNode node)
    {
        var typeDecl = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (typeDecl is null)
        {
            return false;
        }

        var ns = typeDecl.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString()
                 ?? typeDecl.FirstAncestorOrSelf<FileScopedNamespaceDeclarationSyntax>()?.Name.ToString();

        return ns is not null
               && ns.Contains("Contracts.Response", StringComparison.Ordinal);
    }

    private static bool ContainsMaskSsnInvocation(
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        CancellationToken cancellationToken)
    {
        foreach (var invocation in expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
            if (symbol?.Name == "MaskSsn")
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsExplicitlyMaskedSsnLiteral(ExpressionSyntax expression)
    {
        // Allow already-masked literals in DTO example factories (e.g., "***-**-1234", "XXX-XX-1234").
        // We still require MaskSsn() for real mappings, but we don't want sample DTOs to fail the build.
        if (expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression)
            && literal.Token.ValueText is { Length: > 0 } value
            && (value.IndexOf('*') >= 0 || value.IndexOf('X') >= 0 || value.IndexOf('x') >= 0))
        {
            return true;
        }

        return false;
    }
}
