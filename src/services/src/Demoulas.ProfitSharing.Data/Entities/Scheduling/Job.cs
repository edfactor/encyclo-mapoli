namespace Demoulas.ProfitSharing.Data.Entities.Scheduling;

public sealed class Job
{
    public int Id { get; set; }
    public JobType? JobType { get; set; }
    public required byte JobTypeId { get; set; }
    public StartMethod? StartMethod { get; set; }
    public required byte StartMethodId { get; set; }
    public required string RequestedBy { get; set; }
    public JobStatus? JobStatus { get; set; }
    public required byte JobStatusId { get; set; }
    public DateTimeOffset Started { get; set; }
    public DateTimeOffset? Completed { get; set; }
}
