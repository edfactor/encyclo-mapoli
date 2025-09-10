using System.Text.Json;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Serialization;
using Serilog.Enrichers.Sensitive;

namespace Demoulas.ProfitSharing.Services.LogMasking;

/// <summary>
/// Serilog masking operator implementing <see cref="IMaskingOperator"/> so it can be registered in <c>smartConfig.MaskingOperators</c>.
/// Adds an overload <see cref="Mask(object?)"/> used by tests (and optionally by custom logging) to apply the full response masking logic
/// by serializing with <see cref="MaskingJsonConverterFactory"/> under an IT DevOps role context.
/// The string-based <see cref="IMaskingOperator.Mask(string)"/> implementation is a no-op so existing regex / other operators still run first;
/// callers wanting full object masking should invoke the object overload before logging.
/// </summary>
public sealed class SensitiveValueMaskingOperator : RegexMaskingOperator
{
    private readonly JsonSerializerOptions _options;

    public SensitiveValueMaskingOperator()
        : base("(?!)") // regex that never matches (no-op for simple strings)
    {
        _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _options.Converters.Insert(0, new MaskingJsonConverterFactory());
    }

    // Extended helper for structured objects applying IT DevOps masking semantics.
    public string MaskObject(object? value)
    {
        if (value is null)
        {
            return "null";
        }
        RoleContextSnapshot? previous = MaskingAmbientRoleContext.Current;
        try
        {
            MaskingAmbientRoleContext.Current = new RoleContextSnapshot(new[] { Role.ITDEVOPS }, true, false);
            return JsonSerializer.Serialize(value, _options);
        }
        finally
        {
            MaskingAmbientRoleContext.Current = previous;
        }
    }

    protected override string PreprocessInput(string input) => input; // no-op
    protected override bool ShouldMaskInput(string input) => false; // never mask plain strings
}
