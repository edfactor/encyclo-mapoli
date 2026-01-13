namespace Demoulas.ProfitSharing.Common.Contracts.Messaging;

public sealed record OracleHcmJobRequest
{
    public required byte JobType { get; set; } // Full, Delta, Individual
    public required byte StartMethod { get; set; } // System, on-demand
    public required string RequestedBy { get; set; }
}
