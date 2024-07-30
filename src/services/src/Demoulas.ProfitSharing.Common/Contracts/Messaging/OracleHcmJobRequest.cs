namespace Demoulas.ProfitSharing.Common.Contracts.Messaging;

public sealed record OracleHcmJobRequest
{
    public sealed record Enum
    {
        public enum JobTypeEnum : byte
        {
            Full = 0,
            Delta = 1,
            Individual = 2
        }
    }

    public required Enum.JobTypeEnum JobType { get; set; } // Full, Delta, Individual
    public required byte StartMethod { get; set; } // System, on-demand
    public required string RequestedBy { get; set; }
}
