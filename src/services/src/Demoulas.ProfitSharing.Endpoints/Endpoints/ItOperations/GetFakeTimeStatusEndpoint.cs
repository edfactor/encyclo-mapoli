using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to get the current fake time status and configuration.
/// This allows IT Operations to monitor whether fake time is active and its settings.
/// </summary>
public sealed class GetFakeTimeStatusEndpoint : ProfitSharingResponseEndpoint<FakeTimeStatusResponse>
{
    private readonly TimeProvider _timeProvider;
    private readonly FakeTimeConfiguration _fakeTimeConfig;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<GetFakeTimeStatusEndpoint> _logger;

    public GetFakeTimeStatusEndpoint(
        TimeProvider timeProvider,
        FakeTimeConfiguration fakeTimeConfig,
        IHostEnvironment hostEnvironment,
        ILogger<GetFakeTimeStatusEndpoint> logger) : base(Navigation.Constants.FakeTimeManagement)
    {
        _timeProvider = timeProvider;
        _fakeTimeConfig = fakeTimeConfig;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("itdevops/fake-time/status");
        Summary(s =>
        {
            s.Summary = "Gets the current fake time status and configuration";
            s.Description = "Returns information about whether fake time is enabled, the current fake time if active, " +
                "and configuration details. SECURITY: Fake time is never allowed in Production environments. " +
                "This endpoint is accessible to all authenticated users for displaying the fake time banner.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new FakeTimeStatusResponse
                    {
                        IsActive = true,
                        IsAllowed = true,
                        CurrentFakeDateTime = "2025-12-15T10:00:00.0000000-05:00",
                        ConfiguredDateTime = "2025-12-15T10:00:00",
                        TimeZone = "Eastern Standard Time",
                        AdvanceTime = false,
                        Environment = "Development",
                        RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                        Message = "Fake time is active. System is operating at December 15, 2025."
                    }
                }
            };
        });
        // Do NOT use Group<ItDevOpsGroup>() - this endpoint must be accessible to all authenticated users
        // The ItDevOpsGroup applies CanFreezeDemographics policy which is too restrictive for status checking
    }

    public override Task<FakeTimeStatusResponse> ExecuteAsync(CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var isProduction = _hostEnvironment.IsProduction();
            var isFakeTimeActive = _timeProvider.IsFakeTime();
            var realNow = DateTimeOffset.Now;

            var response = new FakeTimeStatusResponse
            {
                IsActive = isFakeTimeActive,
                IsAllowed = !isProduction,
                ConfiguredDateTime = _fakeTimeConfig.FixedDateTime,
                TimeZone = _fakeTimeConfig.TimeZone,
                AdvanceTime = _fakeTimeConfig.AdvanceTime,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O")
            };

            if (isFakeTimeActive)
            {
                var currentFakeTime = _timeProvider.GetLocalNow();
                response.CurrentFakeDateTime = currentFakeTime.ToString("O");
                response.Message = $"Fake time is active. System is operating at {currentFakeTime:MMMM d, yyyy h:mm tt}.";
            }
            else if (isProduction)
            {
                response.Message = "Fake time is disabled. This is a Production environment where fake time is never allowed.";
            }
            else
            {
                response.Message = "Fake time is not active. System is using real time.";
            }

            // Record telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "fake-time-status-query"),
                new("endpoint", nameof(GetFakeTimeStatusEndpoint)),
                new("is_active", isFakeTimeActive.ToString().ToLowerInvariant()));

            _logger.LogInformation(
                "Fake time status queried. Active: {IsActive}, Allowed: {IsAllowed}, Environment: {Environment} (correlation: {CorrelationId})",
                isFakeTimeActive, !isProduction, _hostEnvironment.EnvironmentName, HttpContext.TraceIdentifier);

            return await Task.FromResult(response);
        });
    }
}
