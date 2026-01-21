using Demoulas.ProfitSharing.Analyzers;
using Microsoft.CodeAnalysis;

namespace Demoulas.ProfitSharing.UnitTests.Analyzers;

/// <summary>
/// Unit tests for compliance analyzers: DSM002, DSM005, DSM007, DSM008, DSM009.
/// These tests use xUnit v3 with Microsoft Testing Platform.
/// </summary>
public class ComplianceAnalyzersTests
{
    [Fact]
    public Task Dsm002_ShouldRequireInterface_WhenResponseDtoHasFirstNameAndLastName()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record TestResponse
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}";

        var expected = AnalyzerVerifier<NameInterfaceAnalyzer>.Diagnostic("DSM002", DiagnosticSeverity.Info)
            .WithLocation(4, 19)
            .WithArguments("TestResponse", "FirstName/LastName", "INameParts");

        return AnalyzerVerifier<NameInterfaceAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm005_ShouldFlagGenericNameProperty_InContractsResponsePersonDto()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        public string Name { get; init; }
        public string Ssn { get; init; }
        public int BadgeNumber { get; init; }
    }
}";

        var expected = AnalyzerVerifier<DtoNamePropertyAnalyzer>.Diagnostic("DSM005")
            .WithLocation(6, 23)
            .WithArguments("Name", "PersonResponse");

        return AnalyzerVerifier<DtoNamePropertyAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm005_ShouldNotFlag_WhenLookupTableUsesName()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup
{
    public record StateLookupResponse
    {
        public string Code { get; init; }
        public string Name { get; init; }
    }
}";

        // No diagnostic expected - lookup tables can use Name
        return AnalyzerVerifier<DtoNamePropertyAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Dsm007_ShouldRequireMaskSensitive_OnAge_InResponsePersonDto()
    {
        var source = @"
using Demoulas.ProfitSharing.Common.Contracts.Masking;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        public int? Age { get; init; }
        public string Ssn { get; init; }
    }
}";

        var expected = AnalyzerVerifier<MaskSensitiveAttributeAnalyzer>.Diagnostic("DSM007", DiagnosticSeverity.Error)
            .WithLocation(8, 21)
            .WithArguments("Age", "PersonResponse");

        return AnalyzerVerifier<MaskSensitiveAttributeAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm009_ShouldForbidMaskSensitive_OnBadgeNumber()
    {
        var source = @"
using Demoulas.ProfitSharing.Common.Contracts.Masking;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        [MaskSensitive]
        public int BadgeNumber { get; init; }
        public string Ssn { get; init; }
    }
}";

        var expected = AnalyzerVerifier<MaskSensitiveAttributeAnalyzer>.Diagnostic("DSM009", DiagnosticSeverity.Error)
            .WithLocation(9, 20)
            .WithArguments("BadgeNumber", "PersonResponse");

        return AnalyzerVerifier<MaskSensitiveAttributeAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm008_ShouldRequireMaskSsn_WhenAssigningSsnInObjectInitializer()
    {
        var source = @"
using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Services.Test
{
    public class TestService
    {
        public void AssignSsn()
        {
            var response = new Demoulas.ProfitSharing.Common.Contracts.Response.Test.PersonResponse
            {
                Ssn = ""123456789""
            };
        }
    }
}

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        public string Ssn { get; init; }
    }
}";

        var expected = AnalyzerVerifier<SsnMaskingAssignmentAnalyzer>.Diagnostic("DSM008", DiagnosticSeverity.Error)
            .WithLocation(12, 17)
            .WithArguments("PersonResponse");

        return AnalyzerVerifier<SsnMaskingAssignmentAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm008_ShouldNotReport_WhenMaskSsnUsed()
    {
        var source = @"
using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Services.Test
{
    public class TestService
    {
        public void AssignSsn()
        {
            var response = new Demoulas.ProfitSharing.Common.Contracts.Response.Test.PersonResponse
            {
                Ssn = ""123456789"".MaskSsn()
            };
        }
    }
}

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        public string Ssn { get; init; }
    }
}";

        // No diagnostic expected - MaskSsn() is used
        return AnalyzerVerifier<SsnMaskingAssignmentAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Dsm008_ShouldNotReport_WhenAssignmentInsideContractsResponse()
    {
        var source = @"
using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Test
{
    public record PersonResponse
    {
        public string Ssn { get; init; }

        public static PersonResponse Example() => new()
        {
            Ssn = ""123456789"".MaskSsn()
        };
    }
}";

        // No diagnostic expected - Assignment is inside Contracts.Response declaration (factory method)
        return AnalyzerVerifier<SsnMaskingAssignmentAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
