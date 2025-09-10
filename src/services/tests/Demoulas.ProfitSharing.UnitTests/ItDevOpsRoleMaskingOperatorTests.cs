using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.LogMasking;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Contracts.Response;

public class SerilogItDevOpsMaskingOperatorObjectMaskTests
{
    private sealed record SampleDto
    {
        [ProfitSharing.Common.Attributes.MaskSensitive]
        public string Secret { get; init; } = "Alpha123";
        public decimal Amount { get; init; } = 123.45m;
        [ProfitSharing.Common.Attributes.UnmaskSensitive(Role.ITDEVOPS)]
        public string VisibleToIt { get; init; } = "DevOnly";
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
    public void Serilog_ObjectMask_Matches_IT_Privileged_Output()
    {
        var dto = new SampleDto();
        string viaConverter = SerializeAsIt(dto);
        var serilogOp = new SensitiveValueMaskingOperator();
        string viaOperator = serilogOp.MaskObject(dto);
        viaOperator.ShouldBe(viaConverter);
        // Spot check essential expectations
        viaOperator.ShouldContain("\"secret\":\"XXXXXXXX\"");
        viaOperator.ShouldContain("\"amount\":\"XXX.XX\"");
        viaOperator.ShouldContain("\"visibleToIt\":\"DevOnly\"");
    }
}
