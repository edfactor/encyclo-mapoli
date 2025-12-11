using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record MissiveResponse : IdRequest
{
    public required string Message { get; set; }
    public required string Description { get; set; }
    public required string Severity { get; set; }
}
