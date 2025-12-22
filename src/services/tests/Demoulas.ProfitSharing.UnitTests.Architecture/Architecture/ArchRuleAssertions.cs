using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

internal static class ArchRuleAssertions
{
    internal static void AssertNoArchitectureViolations(IArchRule rule, ArchUnitNET.Domain.Architecture architecture)
    {
        // ArchUnitNET's public assertion API differs slightly across versions/packages.
        // Evaluate is the most stable API surface; we use reflection to keep these tests resilient.
        var evaluateMethod = rule.GetType().GetMethod("Evaluate", new[] { typeof(ArchUnitNET.Domain.Architecture) });
        evaluateMethod.ShouldNotBeNull($"ArchUnitNET rule type '{rule.GetType().FullName}' must expose Evaluate(Architecture).");

        var evaluation = evaluateMethod.Invoke(rule, new object[] { architecture });
        evaluation.ShouldNotBeNull("ArchUnitNET Evaluate() returned null.");

        if (evaluation is System.Collections.IEnumerable evaluationEnumerable && evaluation is not string)
        {
            var evaluationItems = evaluationEnumerable.Cast<object?>().Where(x => x is not null).ToList();
            evaluationItems.Count.ShouldBeGreaterThan(0, "ArchUnitNET Evaluate() returned an empty evaluation set.");

            foreach (var evaluationItem in evaluationItems)
            {
                AssertEvaluationHasNoViolations(evaluationItem!);
            }

            return;
        }

        AssertEvaluationHasNoViolations(evaluation);
    }

    private static void AssertEvaluationHasNoViolations(object evaluation)
    {
        evaluation.ShouldNotBeNull("ArchUnitNET evaluation item was null.");

        // Newer ArchUnitNET versions return EvaluationResult objects with a Passed flag.
        var passedProperty = evaluation.GetType().GetProperty("Passed");
        if (passedProperty is not null && passedProperty.PropertyType == typeof(bool))
        {
            var passed = (bool)passedProperty.GetValue(evaluation)!;
            passed.ShouldBeTrue(evaluation.ToString());
            return;
        }

        // Try common patterns: HasNoViolations() method or HasNoViolations bool property.
        var hasNoViolationsMethod = evaluation.GetType().GetMethod("HasNoViolations", Type.EmptyTypes);
        if (hasNoViolationsMethod is not null && hasNoViolationsMethod.ReturnType == typeof(bool))
        {
            var hasNoViolations = (bool)hasNoViolationsMethod.Invoke(evaluation, Array.Empty<object>())!;
            hasNoViolations.ShouldBeTrue(evaluation.ToString());
            return;
        }

        var hasNoViolationsProperty = evaluation.GetType().GetProperty("HasNoViolations");
        if (hasNoViolationsProperty is not null && hasNoViolationsProperty.PropertyType == typeof(bool))
        {
            var hasNoViolations = (bool)hasNoViolationsProperty.GetValue(evaluation)!;
            hasNoViolations.ShouldBeTrue(evaluation.ToString());
            return;
        }

        // Fallback: if there's a Violations collection, assert empty.
        var violationsProperty = evaluation.GetType().GetProperty("Violations");
        if (violationsProperty is not null)
        {
            var violations = violationsProperty.GetValue(evaluation);
            if (violations is null)
            {
                return;
            }

            if (violations is System.Collections.IEnumerable enumerable)
            {
                var hasAny = enumerable.Cast<object?>().Any();
                hasAny.ShouldBeFalse(evaluation.ToString());
                return;
            }

            throw new InvalidOperationException($"Unsupported Violations type '{violations.GetType().FullName}'.");
        }

        // If we can't determine pass/fail, fail loudly with available API hints.
        var members = string.Join(", ", evaluation.GetType().GetMembers().Select(m => m.Name).Distinct().OrderBy(n => n));
        throw new InvalidOperationException($"Unsupported ArchUnitNET evaluation result type '{evaluation.GetType().FullName}'. Members: {members}");
    }
}
