using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Logging;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;

namespace Demoulas.ProfitSharing.Contracts.Response.Testing;

public class ItDevOpsResponseMaskingOperatorTests
{
    private sealed record SampleDto(
        [property: ProfitSharing.Common.Attributes.MaskSensitive] string Secret,
        decimal Amount,
        [property: ProfitSharing.Common.Attributes.Unmask(Role.ITDEVOPS)] string VisibleToIt,
        bool IsExecutive = false)
    {
        public SampleDto() : this("Alpha123", 123.45m, "DevOnly", false) { }
    }

    private static string SerializeAsIt(SampleDto dto)
    {
        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.Converters.Insert(0, new MaskingJsonConverterFactory());
        MaskingAmbientRoleContext.Current = new RoleContextSnapshot(new[] { Role.ITDEVOPS }, true, false);
        try { return JsonSerializer.Serialize(dto, opts); }
        finally { MaskingAmbientRoleContext.Clear(); }
    }

    [Fact]
    public void Operator_Output_Equals_IT_Context_Output()
    {
        var dto = new SampleDto();
        var op = new SerilogItDevOpsMaskingOperator();
        string expected = SerializeAsIt(dto);
        string actual = op.MaskObject(dto);
        actual.ShouldBe(expected);
        actual.ShouldContain("\"secret\":\"XXXXXXXX\"");
        actual.ShouldContain("\"amount\":\"XXX.XX\"");
        actual.ShouldContain("\"visibleToIt\":\"DevOnly\"");
    }

}
