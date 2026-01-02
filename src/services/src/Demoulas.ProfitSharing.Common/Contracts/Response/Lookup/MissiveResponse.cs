using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record MissiveResponse : IdRequest<int>
{
    public required string Message { get; set; }
    public required string Description { get; set; }
    public required string Severity { get; set; }

    public static MissiveResponse ResponseExample()
    {
        return new MissiveResponse
        {
            Id = 1,
            Message = "System notification",
            Description = "Example missive description",
            Severity = "Info"
        };
    }
}
