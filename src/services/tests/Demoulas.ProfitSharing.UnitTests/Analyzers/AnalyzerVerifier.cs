using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Demoulas.ProfitSharing.UnitTests.Analyzers;

public static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    private const string CommonMaskingStubs = @"
namespace Demoulas.ProfitSharing.Common.Contracts.Masking
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Parameter)]
    public sealed class MaskSensitiveAttribute : System.Attribute;
}
";

    private const string CommonExtensionsStubs = @"
namespace Demoulas.ProfitSharing.Common.Extensions
{
    public static class SsnMaskingExtensions
    {
        public static string MaskSsn(this string value) => value;
    }
}
";

    public static DiagnosticResult Diagnostic(string diagnosticId) => Diagnostic(diagnosticId, DiagnosticSeverity.Warning);

    public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity) => new DiagnosticResult(diagnosticId, severity);

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.Sources.Add(("CommonMaskingStubs.cs", CommonMaskingStubs));
        test.TestState.Sources.Add(("CommonExtensionsStubs.cs", CommonExtensionsStubs));

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync(CancellationToken.None);
    }
}
