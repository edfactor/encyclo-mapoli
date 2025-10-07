namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Validation result for a single field
/// </summary>
public class FieldValidationResult
{
    /// <summary>
    /// Whether this field's value matches the archived checksum
    /// </summary>
    public bool Matches { get; init; }

    /// <summary>
    /// The value provided by the caller for this field
    /// </summary>
    public decimal ProvidedValue { get; init; }

    /// <summary>
    /// The checksum hash of the provided value
    /// </summary>
    public string ProvidedChecksum { get; init; } = string.Empty;

    /// <summary>
    /// The archived checksum for this field (if it exists in archive)
    /// </summary>
    public string? ArchivedChecksum { get; init; }

    /// <summary>
    /// Descriptive message about this field's validation
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
