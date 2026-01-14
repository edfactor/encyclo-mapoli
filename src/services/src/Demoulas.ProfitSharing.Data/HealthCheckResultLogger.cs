using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Data;

public class HealthCheckResultLogger : IHealthCheckPublisher
{
    private readonly ILogger<HealthCheckResultLogger> _logger;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public HealthCheckResultLogger(ILogger<HealthCheckResultLogger> logger, IProfitSharingDataContextFactory dataContextFactory)
    {
        _logger = logger;
        _dataContextFactory = dataContextFactory;
    }

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Health status: {Status}", report.Status);
        List<HealthCheckStatusHistory> histories = new();
        foreach (var entry in report.Entries)
        {
            _logger.LogInformation("  {Key}: {Status} - {Description}", entry.Key, entry.Value.Status, entry.Value.Description);
            histories.Add(new HealthCheckStatusHistory
            {
                Key = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description,
                Exception = entry.Value.Exception?.Message,
                Duration = entry.Value.Duration
            });
        }

        return _dataContextFactory.UseWritableContext(ctx =>
        {
            ctx.HealthCheckStatusHistories.AddRange(histories);
            return ctx.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}

