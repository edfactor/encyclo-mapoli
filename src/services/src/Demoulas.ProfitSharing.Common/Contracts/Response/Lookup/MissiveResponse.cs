namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
public sealed record MissiveResponse
{
    public int Id { get; set; }
    public required string Message { get; set; }
}
