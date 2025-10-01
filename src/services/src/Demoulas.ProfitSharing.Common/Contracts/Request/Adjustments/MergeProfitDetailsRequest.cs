using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Demographics;

/// <summary>
/// Request to merge profit details from a source demographic to a target demographic
/// </summary>
public sealed class MergeProfitDetailsRequest
{
    /// <summary>
    /// Source demographic ID from which profit details will be transferred
    /// </summary>
    public int SourceSsn { get; set; }

    /// <summary>
    /// Target demographic ID to which profit details will be transferred
    /// </summary>
    public int DestinationSsn { get; set; }

    /// <summary>
    /// Creates an example request for documentation
    /// </summary>
    public static MergeProfitDetailsRequest RequestExample() => new()
    {
        SourceSsn = 700000039,
        DestinationSsn = 700000678
    };
}
