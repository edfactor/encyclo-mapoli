using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Logging;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;

namespace Demoulas.ProfitSharing.Contracts.Response.Testing;

public class SerilogItDevOpsMaskingOperatorTests
{
    private sealed record SampleDto(
        [property: ProfitSharing.Common.Attributes.MaskSensitive] string Secret,
        decimal Amount,
        [property: ProfitSharing.Common.Attributes.Unmask(Role.ITDEVOPS)] string VisibleToIt)
    {
        public SampleDto() : this("Alpha123", 123.45m, "DevOnly") { }
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
    public void SerilogOperator_Output_Equals_IT_Context_Output()
    {
        var dto = new SampleDto();
        var op = new SerilogItDevOpsMaskingOperator();
        string expected = SerializeAsIt(dto);
        string actual = op.MaskObject(dto);
        actual.ShouldBe(expected);
    }
}
