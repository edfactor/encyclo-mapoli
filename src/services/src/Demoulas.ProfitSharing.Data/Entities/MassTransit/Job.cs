using Demoulas.ProfitSharing.Common.Contracts.Messaging;

namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class Job
{
    public int Id { get; set; }
    public required OracleHcmJobRequest.Enum.JobTypeEnum JobType { get; set; }
    public required OracleHcmJobRequest.Enum.StartMethodEnum StartMethod { get; set; }
    public required string RequestedBy { get; set; }
    public OracleHcmJobRequest.Enum.JobStatusEnum StatusEnum { get; set; }
    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; }
}
