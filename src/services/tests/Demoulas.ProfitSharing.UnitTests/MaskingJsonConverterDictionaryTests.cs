using System.ComponentModel;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Serialization;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Contracts.Response;

/// <summary>
/// Tests for masking Dictionary&lt;string, decimal&gt; properties in response DTOs.
/// Dictionaries with decimal values should follow the same masking rules as direct decimal properties:
/// - Masked in privileged contexts (IT role or executive rows)
/// - Unmasked for standard users viewing non-executive data
/// </summary>
public class MaskingJsonConverterDictionaryTests
{
    private readonly ITestOutputHelper _output;
    public MaskingJsonConverterDictionaryTests(ITestOutputHelper output) => _output = output;

    private static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions opts = new(JsonSerializerDefaults.Web);
        opts.Converters.Insert(0, new MaskingJsonConverterFactory());
        return opts;
    }

    private static string Serialize<T>(T dto, params string[] roles) where T : class
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

    public sealed record DtoWithDictionary
    {
        public Dictionary<string, decimal> StateTaxTotals { get; init; } = new()
        {
            { "MA", 100.00m },
            { "NH", 23.56m },
            { "CT", 45.78m }
        };

        public decimal DirectDecimal { get; init; } = 999.99m;
        public bool IsExecutive { get; init; }
    }

    [Fact]
    [Description("PS-XXXX: Standard users viewing non-executive data should see unmasked dictionary decimal values")]
    public void StandardUser_NonExecutiveRow_DictionaryDecimalsUnmasked()
    {
        // Arrange
        var dto = new DtoWithDictionary { IsExecutive = false };

        // Act
        string json = Serialize(dto, "StandardUser");
        _output.WriteLine("StandardUser_NonExecutiveRow JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Direct decimal should be unmasked
        doc.RootElement.GetProperty("directDecimal").GetDecimal().ShouldBe(999.99m);

        // Dictionary decimal values should also be unmasked
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.GetProperty("MA").GetDecimal().ShouldBe(100.00m);
        stateTaxTotals.GetProperty("NH").GetDecimal().ShouldBe(23.56m);
        stateTaxTotals.GetProperty("CT").GetDecimal().ShouldBe(45.78m);
    }

    [Fact]
    [Description("PS-XXXX: IT users should see masked dictionary decimal values")]
    public void ItDevOpsUser_DictionaryDecimalsMasked()
    {
        // Arrange
        var dto = new DtoWithDictionary { IsExecutive = false };

        // Act
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("ItDevOpsUser JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Direct decimal should be masked (privileged context)
        doc.RootElement.GetProperty("directDecimal").GetString()!.ShouldBe("XXX.XX");

        // Dictionary decimal values should also be masked
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.GetProperty("MA").GetString()!.ShouldBe("XXX.XX");
        stateTaxTotals.GetProperty("NH").GetString()!.ShouldBe("XX.XX");
        stateTaxTotals.GetProperty("CT").GetString()!.ShouldBe("XX.XX");
    }

    [Fact]
    [Description("PS-XXXX: Standard users viewing executive rows should see masked dictionary decimal values")]
    public void StandardUser_ExecutiveRow_DictionaryDecimalsMasked()
    {
        // Arrange
        var dto = new DtoWithDictionary { IsExecutive = true };

        // Act
        string json = Serialize(dto, "StandardUser");
        _output.WriteLine("StandardUser_ExecutiveRow JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Direct decimal should be masked (executive row = privileged context)
        doc.RootElement.GetProperty("directDecimal").GetString()!.ShouldBe("XXX.XX");

        // Dictionary decimal values should also be masked
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.GetProperty("MA").GetString()!.ShouldBe("XXX.XX");
        stateTaxTotals.GetProperty("NH").GetString()!.ShouldBe("XX.XX");
        stateTaxTotals.GetProperty("CT").GetString()!.ShouldBe("XX.XX");
    }

    [Fact]
    [Description("PS-XXXX: Executive admins should see everything unmasked")]
    public void ExecutiveAdmin_AllUnmasked()
    {
        // Arrange
        var dto = new DtoWithDictionary { IsExecutive = true };

        // Act
        string json = Serialize(dto, Role.EXECUTIVEADMIN);
        _output.WriteLine("ExecutiveAdmin JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Direct decimal should be unmasked
        doc.RootElement.GetProperty("directDecimal").GetDecimal().ShouldBe(999.99m);

        // Dictionary decimal values should also be unmasked
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.GetProperty("MA").GetDecimal().ShouldBe(100.00m);
        stateTaxTotals.GetProperty("NH").GetDecimal().ShouldBe(23.56m);
        stateTaxTotals.GetProperty("CT").GetDecimal().ShouldBe(45.78m);
    }

    [Fact]
    [Description("PS-XXXX: Empty dictionary should serialize correctly")]
    public void EmptyDictionary_SerializesCorrectly()
    {
        // Arrange
        var dto = new DtoWithDictionary { StateTaxTotals = new Dictionary<string, decimal>() };

        // Act
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("EmptyDictionary JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.GetRawText().ShouldBe("{}");
    }

    [Fact]
    [Description("PS-XXXX: Null dictionary should serialize as null")]
    public void NullDictionary_SerializesAsNull()
    {
        // Arrange
        var dto = new DtoWithDictionary { StateTaxTotals = null! };

        // Act
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("NullDictionary JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");
        stateTaxTotals.ValueKind.ShouldBe(JsonValueKind.Null);
    }

    public sealed record DtoWithMultipleDictionaries
    {
        public Dictionary<string, decimal> StateTaxTotals { get; init; } = new()
        {
            { "MA", 100.00m },
            { "NH", 50.00m }
        };

        public Dictionary<string, decimal> FederalTaxTotals { get; init; } = new()
        {
            { "Category1", 200.00m },
            { "Category2", 75.00m }
        };

        public Dictionary<string, string> Metadata { get; init; } = new()
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" }
        };

        public bool IsExecutive { get; init; }
    }

    [Fact]
    [Description("PS-XXXX: Multiple dictionaries with decimal values should all be masked in privileged context")]
    public void MultipleDictionaries_AllMaskedForItUser()
    {
        // Arrange
        var dto = new DtoWithMultipleDictionaries { IsExecutive = false };

        // Act
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("MultipleDictionaries_IT JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Both decimal dictionaries should be masked
        var stateTax = doc.RootElement.GetProperty("stateTaxTotals");
        stateTax.GetProperty("MA").GetString()!.ShouldBe("XXX.XX");
        stateTax.GetProperty("NH").GetString()!.ShouldBe("XX.XX");

        var federalTax = doc.RootElement.GetProperty("federalTaxTotals");
        federalTax.GetProperty("Category1").GetString()!.ShouldBe("XXX.XX");
        federalTax.GetProperty("Category2").GetString()!.ShouldBe("XX.XX");

        // String dictionary should not be affected
        var metadata = doc.RootElement.GetProperty("metadata");
        metadata.GetProperty("Key1").GetString()!.ShouldBe("Value1");
        metadata.GetProperty("Key2").GetString()!.ShouldBe("Value2");
    }

    [Fact]
    [Description("PS-XXXX: Dictionary with large decimal values should be masked correctly")]
    public void LargeDecimalValues_MaskedCorrectly()
    {
        // Arrange
        var dto = new DtoWithDictionary
        {
            StateTaxTotals = new Dictionary<string, decimal>
            {
                { "State1", 1234567.89m },
                { "State2", 0.01m },
                { "State3", -500.00m }
            },
            IsExecutive = false
        };

        // Act
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("LargeDecimalValues JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);
        var stateTaxTotals = doc.RootElement.GetProperty("stateTaxTotals");

        // All digits should be masked
        stateTaxTotals.GetProperty("State1").GetString()!.ShouldBe("XXXXXXX.XX");
        stateTaxTotals.GetProperty("State2").GetString()!.ShouldBe("X.XX");
        stateTaxTotals.GetProperty("State3").GetString()!.ShouldBe("-XXX.XX");
    }

    [Fact]
    [Description("PS-XXXX: Unmask attribute should work with dictionary properties")]
    public void UnmaskAttribute_WorksWithDictionaries()
    {
        // Arrange
        var dto = new DtoWithUnmaskDictionary { IsExecutive = false };

        // Act - IT user should still see unmasked due to Unmask attribute
        string json = Serialize(dto, Role.ITDEVOPS);
        _output.WriteLine("UnmaskAttribute_IT JSON: " + json);

        // Assert
        using JsonDocument doc = JsonDocument.Parse(json);

        // Dictionary with Unmask attribute should be unmasked even for IT
        var publicTotals = doc.RootElement.GetProperty("publicTotals");
        publicTotals.GetProperty("Total1").GetDecimal().ShouldBe(100.00m);
        publicTotals.GetProperty("Total2").GetDecimal().ShouldBe(200.00m);

        // Regular dictionary should still be masked for IT
        var privateTotals = doc.RootElement.GetProperty("privateTotals");
        privateTotals.GetProperty("Private1").GetString()!.ShouldBe("XXX.XX");
    }

    public sealed record DtoWithUnmaskDictionary
    {
        [UnmaskSensitive]
        public Dictionary<string, decimal> PublicTotals { get; init; } = new()
        {
            { "Total1", 100.00m },
            { "Total2", 200.00m }
        };

        public Dictionary<string, decimal> PrivateTotals { get; init; } = new()
        {
            { "Private1", 300.00m }
        };

        public bool IsExecutive { get; init; }
    }
}
