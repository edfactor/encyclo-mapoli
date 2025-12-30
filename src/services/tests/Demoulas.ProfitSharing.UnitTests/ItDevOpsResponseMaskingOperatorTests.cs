using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.LogMasking;
using Demoulas.ProfitSharing.Services.Serialization;
using Microsoft.Extensions.Hosting;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests;

public class ItDevOpsResponseMaskingOperatorTests
{
    private sealed record SampleDto(
        [property: ProfitSharing.Common.Attributes.MaskSensitive] string Secret,
        decimal Amount,
        [property: ProfitSharing.Common.Attributes.UnmaskSensitive(Role.ITDEVOPS)] string VisibleToIt,
        bool IsExecutive = false)
    {
        public SampleDto() : this("Alpha123", 123.45m, "DevOnly", false) { }
    }

    private static string SerializeAsIt(SampleDto dto)
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Testing");
        mockEnvironment.Setup(e => e.ApplicationName).Returns("Demoulas.ProfitSharing.UnitTests");

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.Converters.Insert(0, new MaskingJsonConverterFactory(mockEnvironment.Object));
        MaskingAmbientRoleContext.Current = new RoleContextSnapshot(new[] { Role.ITDEVOPS }, true, false);
        try { return JsonSerializer.Serialize(dto, opts); }
        finally { MaskingAmbientRoleContext.Clear(); }
    }

    [Fact]
    public void Operator_Output_Equals_IT_Context_Output()
    {
        var dto = new SampleDto();
        
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Testing");
        mockEnvironment.Setup(e => e.ApplicationName).Returns("Demoulas.ProfitSharing.UnitTests");
        
        var op = new SensitiveValueMaskingOperator(mockEnvironment.Object);
        string expected = SerializeAsIt(dto);
        string actual = op.MaskObject(dto);
        actual.ShouldBe(expected);
        actual.ShouldContain("\"secret\":\"XXXXXXXX\"");
        actual.ShouldContain("\"amount\":\"XXX.XX\"");
        actual.ShouldContain("\"visibleToIt\":\"DevOnly\"");
    }

}
