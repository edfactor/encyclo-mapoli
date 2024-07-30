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

        public enum StartMethodEnum : byte
        {
            System = 0,
            OnDemand = 1
        }

        public enum JobStatusEnum
        {
            Pending,
            Running,
            Completed,
            Failed
        }
    }

    public required Enum.JobTypeEnum JobType { get; set; } // Full, Delta, Individual
    public required Enum.StartMethodEnum StartMethod { get; set; } // System, on-demand
    public required string RequestedBy { get; set; }
}
