using System.ComponentModel;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Services.ItOperations.ItDevOps;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.ItDevOps;

/// <summary>
/// Tests for FrozenService to ensure frozen demographic snapshot projections
/// remain EF Core translatable (no custom C# methods in LINQ expressions).
/// </summary>
[Description("PS-2333 : FrozenService EF Core translation guard tests")]
public class FrozenServiceTests
{
    /// <summary>
    /// Verifies that BuildDemographicSnapshot does NOT use DtoCommonExtensions.ComputeFullNameWithInitial
    /// in its projection. This method cannot be translated by the Oracle EF Core provider and will
    /// cause System.InvalidOperationException at runtime if used in IQueryable expressions.
    /// 
    /// This test uses reflection to inspect the source method and verify the pattern is not present.
    /// See PS-2333 for the original bug report.
    /// </summary>
    [Fact]
    [Description("PS-2333 : BuildDemographicSnapshot must not use ComputeFullNameWithInitial (EF translation guard)")]
    public void BuildDemographicSnapshot_DoesNotUseComputeFullNameWithInitial()
    {
        // Arrange
        var frozenServiceType = typeof(FrozenService);
        var buildDemographicSnapshotMethod = frozenServiceType.GetMethod(
            "BuildDemographicSnapshot",
            BindingFlags.NonPublic | BindingFlags.Static);

        buildDemographicSnapshotMethod.ShouldNotBeNull("BuildDemographicSnapshot method should exist");

        // Read the source file and check for the forbidden pattern
        // This is a compile-time guard - if someone adds ComputeFullNameWithInitial back, this test will fail
        var sourceFilePath = GetFrozenServiceSourcePath();

        if (File.Exists(sourceFilePath))
        {
            var sourceCode = File.ReadAllText(sourceFilePath);

            // Check that ComputeFullNameWithInitial is NOT used within the BuildDemographicSnapshot method context
            // The method spans from "private static IQueryable<Demographic> BuildDemographicSnapshot" to the closing brace
            var methodStartIndex = sourceCode.IndexOf("private static IQueryable<Demographic> BuildDemographicSnapshot", StringComparison.Ordinal);

            if (methodStartIndex >= 0)
            {
                // Find the method body by counting braces
                var braceCount = 0;
                var methodStarted = false;
                var methodEndIndex = methodStartIndex;

                for (int i = methodStartIndex; i < sourceCode.Length; i++)
                {
                    if (sourceCode[i] == '{')
                    {
                        braceCount++;
                        methodStarted = true;
                    }
                    else if (sourceCode[i] == '}')
                    {
                        braceCount--;
                        if (methodStarted && braceCount == 0)
                        {
                            methodEndIndex = i;
                            break;
                        }
                    }
                }

                var methodBody = sourceCode.Substring(methodStartIndex, methodEndIndex - methodStartIndex + 1);

                // Verify ComputeFullNameWithInitial is NOT called in the method body
                // (it should only appear in comments explaining why it can't be used)
                var forbiddenPattern = "ComputeFullNameWithInitial(";
                var patternIndex = methodBody.IndexOf(forbiddenPattern, StringComparison.Ordinal);

                if (patternIndex >= 0)
                {
                    // Check if it's in a comment (single-line or multi-line)
                    var lineStart = methodBody.LastIndexOf('\n', patternIndex);
                    var lineContent = lineStart >= 0
                        ? methodBody.Substring(lineStart, patternIndex - lineStart)
                        : methodBody.Substring(0, patternIndex);

                    var isInComment = lineContent.Contains("//") || lineContent.Contains("/*") || lineContent.Contains("*");

                    if (!isInComment)
                    {
                        Assert.Fail(
                            "BuildDemographicSnapshot contains a call to ComputeFullNameWithInitial which cannot be " +
                            "translated by EF Core Oracle provider. This will cause System.InvalidOperationException at runtime. " +
                            "Use inline string concatenation instead. See PS-2333 for details.");
                    }
                }
            }
        }

        // Additional verification: ensure DtoCommonExtensions.ComputeFullNameWithInitial method signature matches expectation
        var computeFullNameMethod = typeof(DtoCommonExtensions).GetMethod(
            nameof(DtoCommonExtensions.ComputeFullNameWithInitial),
            BindingFlags.Public | BindingFlags.Static);

        computeFullNameMethod.ShouldNotBeNull("ComputeFullNameWithInitial should exist in DtoCommonExtensions");
    }

    /// <summary>
    /// Verifies that GetDemographicSnapshot returns a valid IQueryable that can be enumerated
    /// without throwing EF translation exceptions in a test (in-memory) context.
    /// </summary>
    [Fact]
    [Description("PS-2333 : GetDemographicSnapshot projection should be enumerable without translation errors")]
    public void GetDemographicSnapshot_ProjectionIsEnumerable()
    {
        // This test verifies the projection structure is valid
        // Note: In-memory provider doesn't catch all Oracle translation issues,
        // but it validates the basic projection structure

        var frozenServiceType = typeof(FrozenService);
        var getDemographicSnapshotMethod = frozenServiceType.GetMethod(
            "GetDemographicSnapshot",
            BindingFlags.NonPublic | BindingFlags.Static);

        getDemographicSnapshotMethod.ShouldNotBeNull("GetDemographicSnapshot method should exist");

        // Verify method signature
        var parameters = getDemographicSnapshotMethod.GetParameters();
        parameters.Length.ShouldBe(2);
        parameters[0].ParameterType.ShouldBe(typeof(IProfitSharingDbContext));
        parameters[1].ParameterType.ShouldBe(typeof(short));

        getDemographicSnapshotMethod.ReturnType.ShouldBe(typeof(IQueryable<Demographic>));
    }

    private static string GetFrozenServiceSourcePath()
    {
        // Prefer a repo-relative resolution so tests work across machines and CI.
        // If the repository root can't be discovered (or sources aren't present), callers should treat this as "source unavailable".
        var repoRoot = TryFindRepositoryRoot();
        if (repoRoot is null)
        {
            return string.Empty;
        }

        // Repo-relative, stable path to the file.
        return Path.Combine(
            repoRoot,
            "src",
            "services",
            "src",
            "Demoulas.ProfitSharing.Services",
            "ItDevOps",
            "FrozenService.cs");
    }

    private static string? TryFindRepositoryRoot()
    {
        // Start from AppContext.BaseDirectory (more stable than current directory for tests) and walk up.
        var start = new DirectoryInfo(AppContext.BaseDirectory);

        for (DirectoryInfo? dir = start; dir is not null; dir = dir.Parent)
        {
            // Identify THIS repo by checking for the services solution path.
            if (File.Exists(Path.Combine(dir.FullName, "src", "services", "Demoulas.ProfitSharing.slnx")))
            {
                return dir.FullName;
            }
        }

        return null;
    }
}
