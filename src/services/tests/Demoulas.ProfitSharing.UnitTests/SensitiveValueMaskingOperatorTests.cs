using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.LogMasking;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests;

public class SensitiveValueMaskingOperatorTests
{
    private sealed record SampleDto(
        [property: ProfitSharing.Common.Attributes.MaskSensitive] string Secret,
        decimal Amount,
        [property: ProfitSharing.Common.Attributes.UnmaskSensitive(Role.ITDEVOPS)] string VisibleToIt)
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
        var op = new SensitiveValueMaskingOperator();
        string expected = SerializeAsIt(dto);
        string actual = op.MaskObject(dto);
        actual.ShouldBe(expected);
    }
}
