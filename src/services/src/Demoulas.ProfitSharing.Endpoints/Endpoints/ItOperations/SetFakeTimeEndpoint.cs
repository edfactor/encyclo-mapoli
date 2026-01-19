using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to enable or disable fake time at runtime without restarting the application.
/// SECURITY: Only allowed in non-Production environments with IT Operations permissions.
/// </summary>
public sealed class SetFakeTimeEndpoint : ProfitSharingEndpoint<SetFakeTimeRequest, FakeTimeStatusResponse>
{
    private readonly TimeProvider _timeProvider;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<SetFakeTimeEndpoint> _logger;

    public SetFakeTimeEndpoint(
        TimeProvider timeProvider,
        IHostEnvironment hostEnvironment,
        ILogger<SetFakeTimeEndpoint> logger) : base(Navigation.Constants.FakeTimeManagement)
    {
        _timeProvider = timeProvider;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("fake-time");
        Summary(s =>
        {
            s.Summary = "Enables or disables fake time at runtime";
            s.Description = "Activates or deactivates fake time without requiring an application restart. " +
                "This allows IT Operations to test date-sensitive functionality on-demand. " +
                "SECURITY: This endpoint is only available in non-Production environments and requires IT Operations permissions.";
            s.ExampleRequest = new SetFakeTimeRequest
            {
                Enabled = true,
                FixedDateTime = "2025-12-15T10:00:00",
                TimeZone = "Eastern Standard Time",
                AdvanceTime = false
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new FakeTimeStatusResponse
                    {
                        IsActive = true,
                        IsAllowed = true,
                        IsRuntimeSwitchingEnabled = true,
                        CurrentFakeDateTime = "2025-12-15T10:00:00.0000000-05:00",
                        ConfiguredDateTime = "2025-12-15T10:00:00",
                        TimeZone = "Eastern Standard Time",
                        AdvanceTime = false,
                        Environment = "Development",
                        RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                        Message = "Fake time has been activated. System is now operating at December 15, 2025."
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    protected override Task<FakeTimeStatusResponse> HandleRequestAsync(SetFakeTimeRequest req, CancellationToken ct)
    {
        var isProduction = _hostEnvironment.IsProduction();
        var realNow = DateTimeOffset.Now;

        // SECURITY: Block in Production
        if (isProduction)
        {
            _logger.LogWarning(
                "SECURITY: Attempt to modify fake time settings in Production environment was blocked.");

            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = false,
                IsRuntimeSwitchingEnabled = false,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = "SECURITY: Fake time is not allowed in Production environments. This request has been rejected."
            });
        }

        // Check if runtime switching is supported
        var switchableProvider = _timeProvider.AsSwitchable();
        if (switchableProvider is null)
        {
            _logger.LogWarning(
                "Attempt to modify fake time but runtime switching is not enabled. TimeProvider type: {ProviderType}",
                _timeProvider.GetType().Name);

            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = _timeProvider.IsFakeTime(),
                IsAllowed = true,
                IsRuntimeSwitchingEnabled = false,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = "Runtime fake time switching is not enabled. The application was started without the SwitchableTimeProvider. " +
                    "To enable runtime switching, restart the application."
            });
        }

        if (req.Enabled)
        {
            // Activate fake time
            var config = new FakeTimeConfiguration
            {
                Enabled = true,
                FixedDateTime = req.FixedDateTime,
                TimeZone = req.TimeZone,
                AdvanceTime = req.AdvanceTime
            };

            var validationErrors = config.Validate();
            if (validationErrors.Count > 0)
            {
                return Task.FromResult(new FakeTimeStatusResponse
                {
                    IsActive = switchableProvider.IsFakeTimeActive,
                    IsAllowed = true,
                    IsRuntimeSwitchingEnabled = true,
                    ConfiguredDateTime = req.FixedDateTime,
                    TimeZone = req.TimeZone,
                    AdvanceTime = req.AdvanceTime,
                    Environment = _hostEnvironment.EnvironmentName,
                    RealDateTime = realNow.ToString("O"),
                    Message = $"Configuration validation failed: {string.Join("; ", validationErrors)}"
                });
            }

            var activated = switchableProvider.ActivateFakeTime(config);
            if (!activated)
            {
                return Task.FromResult(new FakeTimeStatusResponse
                {
                    IsActive = switchableProvider.IsFakeTimeActive,
                    IsAllowed = true,
                    IsRuntimeSwitchingEnabled = true,
                    ConfiguredDateTime = req.FixedDateTime,
                    TimeZone = req.TimeZone,
                    AdvanceTime = req.AdvanceTime,
                    Environment = _hostEnvironment.EnvironmentName,
                    RealDateTime = realNow.ToString("O"),
                    Message = "Failed to activate fake time. Check the server logs for details."
                });
            }

            var currentFakeTime = switchableProvider.GetLocalNow();
            _logger.LogWarning(
                "Fake time ACTIVATED via API. Fake time: {FakeTime:O}, AdvanceTime: {AdvanceTime}, TimeZone: {TimeZone}",
                currentFakeTime,
                req.AdvanceTime,
                req.TimeZone ?? "System Default");

            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = true,
                IsAllowed = true,
                IsRuntimeSwitchingEnabled = true,
                CurrentFakeDateTime = currentFakeTime.ToString("O"),
                ConfiguredDateTime = req.FixedDateTime,
                TimeZone = config.GetTimeZone().Id,
                AdvanceTime = req.AdvanceTime,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = $"Fake time has been activated. System is now operating at {currentFakeTime:MMMM d, yyyy h:mm tt}."
            });
        }
        else
        {
            // Deactivate fake time
            var wasFakeActive = switchableProvider.IsFakeTimeActive;
            switchableProvider.DeactivateFakeTime();

            if (wasFakeActive)
            {
                _logger.LogInformation("Fake time DEACTIVATED via API. System now using real time.");
            }

            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                IsRuntimeSwitchingEnabled = true,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = wasFakeActive
                    ? "Fake time has been deactivated. System is now using real time."
                    : "Fake time was already disabled. System is using real time."
            });
        }
    }
}
