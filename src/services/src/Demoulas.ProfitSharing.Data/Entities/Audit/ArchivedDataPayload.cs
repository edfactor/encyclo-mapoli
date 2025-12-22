using System.Text.Json;

namespace Demoulas.ProfitSharing.Data.Entities.Audit;

/// <summary>
/// Represents the structure for archived entity data in audit events.
/// Contains type metadata and the raw serialized data.
/// </summary>
public sealed class ArchivedDataPayload
{
    /// <summary>
    /// The fully qualified type name of the entity being archived (e.g., "MyNamespace.MyClass, MyAssembly").
    /// </summary>
    public required string TypeName { get; set; }

    /// <summary>
    /// The raw JSON object of the archived entity (not escaped as a string).
    /// </summary>
    public required JsonElement RawData { get; set; }
}
