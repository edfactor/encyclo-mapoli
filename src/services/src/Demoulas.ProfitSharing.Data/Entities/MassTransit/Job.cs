using Demoulas.ProfitSharing.Common.Contracts.Messaging;

namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class Job
{
    public int Id { get; set; }
    public required OracleHcmJobRequest.Enum.JobTypeEnum JobType { get; set; }
    public StartMethod? StartMethod { get; set; }
    public required byte StartMethodId { get; set; }
    public required string RequestedBy { get; set; }
    public JobStatus? JobStatus { get; set; }
    public required byte JobStatusId { get; set; }
    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; }
}
