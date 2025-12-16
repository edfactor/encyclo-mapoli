using System.ComponentModel;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

[Collection("Architecture Tests")]
public sealed class ArchitectureRulesTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = typeof(ProfitSharingEndpoint).Assembly;
    private static readonly System.Reflection.Assembly s_servicesAssembly = typeof(YearEndService).Assembly;
    private static readonly System.Reflection.Assembly s_dataAssembly = typeof(ProfitSharingDbContext).Assembly;
    private static readonly System.Reflection.Assembly s_efCoreAssembly = typeof(DbContext).Assembly;

    private static readonly ArchUnitNET.Domain.Architecture s_architecture = new ArchLoader()
        .LoadAssemblies(s_endpointsAssembly, s_servicesAssembly, s_dataAssembly, s_efCoreAssembly)
        .Build();

    [Fact]
    [Description("PS-2337 : Endpoints must not depend on EF Core or DbContexts")]
    public void Endpoints_ShouldNotDependOn_EfCore_Or_DataContexts()
    {
        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> efCore = Types().That()
            .ResideInNamespace("Microsoft.EntityFrameworkCore")
            .As("EF Core");

        IObjectProvider<IType> dataContexts = Types().That()
            .ResideInNamespace("Demoulas.ProfitSharing.Data.Contexts")
            .As("Data Contexts");

        IObjectProvider<IType> dataContextFactoryInterfaces = Types().That()
            .ResideInNamespace("Demoulas.ProfitSharing.Data.Interfaces")
            .As("Data Context Factory Interfaces");

        IArchRule endpointsShouldNotDependOnEfOrDb = Types().That().Are(endpoints)
            .Should().NotDependOnAny(efCore)
            .AndShould().NotDependOnAny(dataContexts)
            .AndShould().NotDependOnAny(dataContextFactoryInterfaces)
            .Because("Endpoints must not access EF Core/DbContexts directly; they should call services returning Result<T>.");

        AssertNoArchitectureViolations(endpointsShouldNotDependOnEfOrDb, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Services must not depend on Endpoints")]
    public void Services_ShouldNotDependOn_Endpoints()
    {
        IObjectProvider<IType> services = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .As("Services");

        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IArchRule servicesShouldNotDependOnEndpoints = Types().That().Are(services)
            .Should().NotDependOnAny(endpoints)
            .Because("Service layer should be HTTP-agnostic and must not depend on endpoint implementations.");

        AssertNoArchitectureViolations(servicesShouldNotDependOnEndpoints, s_architecture);
    }

    private static void AssertNoArchitectureViolations(IArchRule rule, ArchUnitNET.Domain.Architecture architecture)
    {
        // ArchUnitNET's public assertion API differs slightly across versions/packages.
        // Evaluate is the most stable API surface; we use reflection to keep this test resilient.
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
        var hasNoViolationsMethod = evaluation.GetType().GetMethod("HasNoViolations", System.Type.EmptyTypes);
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
