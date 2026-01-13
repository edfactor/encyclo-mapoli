

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Headers
{
    /// <summary>Metadata captured for every demographic read.</summary>
    public sealed record DataWindowMetadata(
        bool IsFrozen,
        short? ProfitYear,
        DateTimeOffset WindowEnd);
}

