
namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class HealthCheckStatusHistory
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Status { get; set; }
    public required string? Description { get; set; }
    public string? Exception { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
