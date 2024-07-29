namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class Job
{
    public enum JobStatus
    {
        Pending,
        Running,
        Completed,
        Failed
    }

    public int Id { get; set; }
    public required string JobType { get; set; }
    public required string StartMethod { get; set; }
    public required string RequestedBy { get; set; }
    public JobStatus Status { get; set; }
    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; }
}
