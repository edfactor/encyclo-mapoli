using System.Text.Json;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.Contracts.Response.Testing;

public record SampleDto
{
    [MaskSensitive]
    public string Secret { get; init; } = "Alpha123";
    public decimal Amount { get; init; } = 123.45m;
    public bool IsExecutive { get; init; }
    [Unmask(Role.ITDEVOPS)]
    public string VisibleToIt { get; init; } = "DevOnly";
}

public class MaskingJsonConverterTests
{
    private readonly ITestOutputHelper _output;
    public MaskingJsonConverterTests(ITestOutputHelper output) => _output = output;
    private static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions opts = new(JsonSerializerDefaults.Web);
        opts.Converters.Insert(0, new MaskingJsonConverterFactory());
        return opts;
    }

    private static string Serialize(SampleDto dto, params string[] roles)
    {
        bool isItDevOps = roles.Any(r => string.Equals(r, Role.ITDEVOPS, StringComparison.OrdinalIgnoreCase));
        bool isExecAdmin = roles.Any(r => string.Equals(r, Role.EXECUTIVEADMIN, StringComparison.OrdinalIgnoreCase));
        MaskingAmbientRoleContext.Current = new RoleContextSnapshot(roles, isItDevOps, isExecAdmin);
        try
        {
            return JsonSerializer.Serialize(dto, CreateOptions());
        }
        finally
        {
            MaskingAmbientRoleContext.Clear();
        }
    }

    [Fact]
    public void NonPrivileged_User_Sees_Unmasked_For_NonDecimal_And_UnmaskAttribute()
    {
        // Legacy semantics: [MaskSensitive] only masks in privileged contexts (executive row or IT). Standard user + non-executive row => unmasked.
        // [Unmask(Role.ITDEVOPS)] does NOT cause masking for other roles; property remains visible.
        SampleDto dto = new();
        string json = Serialize(dto, "StandardUser");
        _output.WriteLine("JSON=>" + json);
        using JsonDocument doc = JsonDocument.Parse(json);
        foreach (var p in doc.RootElement.EnumerateObject())
        {
            Console.WriteLine($"PROP: {p.Name}={p.Value}");
        }
        doc.RootElement.GetProperty("secret").GetString()!.ShouldBe("Alpha123");
        doc.RootElement.GetProperty("amount").GetDecimal().ShouldBe(123.45m);
        doc.RootElement.GetProperty("visibleToIt").GetString()!.ShouldBe("DevOnly");
    }

    [Fact]
    public void Unmask_For_ExecutiveAdmin()
    {
        SampleDto dto = new();
        string json = Serialize(dto, Role.EXECUTIVEADMIN);
        Console.WriteLine("Unmask_For_ExecutiveAdmin => " + json);
        using JsonDocument doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("secret").GetString()!.ShouldBe("Alpha123");
        doc.RootElement.GetProperty("amount").GetDecimal().ShouldBe(123.45m);
        doc.RootElement.GetProperty("visibleToIt").GetString()!.ShouldBe("DevOnly");
    }

    [Fact]
    public void Unmask_Role_Specific_Property()
    {
        SampleDto dto = new();
        string json = Serialize(dto, Role.ITDEVOPS);
        Console.WriteLine("Unmask_Role_Specific_Property => " + json);
        using JsonDocument doc = JsonDocument.Parse(json);
        // IT role is privileged -> [MaskSensitive] string should be masked, decimal should be masked (privileged context), property with Unmask attribute for IT remains unmasked.
        doc.RootElement.GetProperty("visibleToIt").GetString()!.ShouldBe("DevOnly");
        doc.RootElement.GetProperty("secret").GetString()!.ShouldBe("XXXXXXXX");
        doc.RootElement.GetProperty("amount").GetString()!.ShouldBe("XXX.XX");
    }

    [Fact]
    public void Executive_Row_Decimal_Masking()
    {
        SampleDto dto = new() { IsExecutive = true };
        string json = Serialize(dto, "StandardUser");
        Console.WriteLine("Executive_Row_Decimal_Masking => " + json);
        using JsonDocument doc = JsonDocument.Parse(json);
        // Executive row makes context privileged -> decimal masked, [MaskSensitive] string masked.
        doc.RootElement.GetProperty("amount").GetString()!.ShouldBe("XXX.XX");
        doc.RootElement.GetProperty("secret").GetString()!.ShouldBe("XXXXXXXX");
    }

    [Fact]
    public void Decimal_Unmasked_For_NonExec_NonIT()
    {
        SampleDto dto = new() { IsExecutive = false };
        string json = Serialize(dto, "StandardUser");
        Console.WriteLine("Decimal_Unmasked_For_NonExec_NonIT => " + json);
        using JsonDocument doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("amount").GetDecimal().ShouldBe(123.45m);
    }
}
