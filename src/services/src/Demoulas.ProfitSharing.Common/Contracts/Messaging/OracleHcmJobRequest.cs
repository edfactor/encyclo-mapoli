namespace Demoulas.ProfitSharing.Common.Contracts.Messaging;

public sealed record OracleHcmJobRequest
{
    public required string JobType { get; set; } // Full, Delta, Individual
    public required string StartMethod { get; set; } // System, on-demand
    public required string RequestedBy { get; set; }
}
