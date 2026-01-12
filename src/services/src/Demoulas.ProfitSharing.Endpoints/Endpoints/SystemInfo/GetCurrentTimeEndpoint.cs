using Demoulas.ProfitSharing.Common.Contracts.Response.SystemInfo;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.SystemInfo;

/// <summary>
/// Endpoint to retrieve the current server time.
/// This endpoint is used by frontend applications to synchronize their time perception
/// with the server, especially when fake time is enabled for testing.
/// </summary>
/// <remarks>
/// GET /api/system/current-time
/// 
/// Returns the server's current UTC and local time, time zone information,
/// and whether fake time is active.
/// </remarks>
public sealed class GetCurrentTimeEndpoint : ProfitSharingResponseEndpoint<CurrentTimeResponse>
{
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<GetCurrentTimeEndpoint> _logger;

    public GetCurrentTimeEndpoint(
        TimeProvider timeProvider,
        ILogger<GetCurrentTimeEndpoint> logger)
        : base(Navigation.Constants.Unknown) // System endpoint without navigation tracking
    {
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("current-time");
        AllowAnonymous(); // Allow without authentication for initial app loading
        Summary(s =>
        {
            s.Summary = "Gets the current server time";
            s.Description = "Returns the server's current UTC and local time, time zone information, " +
                           "and whether fake time is active. Used for frontend synchronization.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new CurrentTimeResponse
                    {
                        UtcNow = new DateTimeOffset(2025, 12, 15, 15, 0, 0, TimeSpan.Zero),
                        LocalNow = new DateTimeOffset(2025, 12, 15, 10, 0, 0, TimeSpan.FromHours(-5)),
                        TimeZoneId = "Eastern Standard Time",
                        TimeZoneDisplayName = "(UTC-05:00) Eastern Time (US & Canada)",
                        IsFakeTime = false,
                        CurrentYear = 2025,
                        CurrentMonth = 12,
                        CurrentDate = "2025-12-15"
                    }
                }
            };
        });
        Group<SystemGroup>();
    }

    public override async Task<CurrentTimeResponse> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var utcNow = _timeProvider.GetUtcNow();
            var localNow = _timeProvider.GetLocalNow();
            var timeZone = _timeProvider.LocalTimeZone;
            var isFakeTime = _timeProvider.IsFakeTime();

            var response = new CurrentTimeResponse
            {
                UtcNow = utcNow,
                LocalNow = localNow,
                TimeZoneId = timeZone.Id,
                TimeZoneDisplayName = timeZone.DisplayName,
                IsFakeTime = isFakeTime,
                CurrentYear = localNow.Year,
                CurrentMonth = localNow.Month,
                CurrentDate = localNow.ToString("yyyy-MM-dd")
            };

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "system-time-query"),
                new("endpoint", "GetCurrentTimeEndpoint"),
                new("is_fake_time", isFakeTime.ToString().ToLowerInvariant()));

            if (isFakeTime)
            {
                _logger.LogDebug(
                    "Returning fake time. UTC: {UtcNow:O}, Local: {LocalNow:O}, TimeZone: {TimeZone} (correlation: {CorrelationId})",
                    utcNow, localNow, timeZone.Id, HttpContext.TraceIdentifier);
            }

            this.RecordResponseMetrics(HttpContext, _logger, response);

            return await Task.FromResult(response);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
