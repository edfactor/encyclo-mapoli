using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.Extensions.Hosting;

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

    public GetFakeTimeStatusEndpoint(
        TimeProvider timeProvider,
        FakeTimeConfiguration fakeTimeConfig,
        IHostEnvironment hostEnvironment) : base(Navigation.Constants.FakeTimeManagement)
    {
        _timeProvider = timeProvider;
        _fakeTimeConfig = fakeTimeConfig;
        _hostEnvironment = hostEnvironment;
    }

    public override void Configure()
    {
        Get("it-devops/fake-time/status");
        Summary(s =>
        {
            s.Summary = "Gets the current fake time status and configuration";
            s.Description = "Returns information about whether fake time is enabled, the current fake time if active, " +
                "configuration details, and whether runtime switching is supported. " +
                "SECURITY: Fake time is never allowed in Production environments. " +
                "This endpoint is accessible to all authenticated users for displaying the fake time banner.";
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
                        Message = "Fake time is active. System is operating at December 15, 2025."
                    }
                }
            };
        });
        // Do NOT use Group<ItDevOpsGroup>() - this endpoint must be accessible to all authenticated users
        // The ItDevOpsGroup applies CanFreezeDemographics policy which is too restrictive for status checking
    }

    protected override Task<FakeTimeStatusResponse> HandleRequestAsync(CancellationToken ct)
    {
        var isProduction = _hostEnvironment.IsProduction();
        var isFakeTimeActive = _timeProvider.IsFakeTime();
        var isRuntimeSwitchingEnabled = _timeProvider.IsRuntimeSwitchingEnabled();
        var realNow = DateTimeOffset.Now;

        var response = new FakeTimeStatusResponse
        {
            IsActive = isFakeTimeActive,
            IsAllowed = !isProduction,
            IsRuntimeSwitchingEnabled = isRuntimeSwitchingEnabled,
            ConfiguredDateTime = _fakeTimeConfig.FixedDateTime,
            TimeZone = _fakeTimeConfig.TimeZone,
            AdvanceTime = _fakeTimeConfig.AdvanceTime,
            Environment = _hostEnvironment.EnvironmentName,
            RealDateTime = realNow.ToString("O")
        };

        // If using switchable provider, get the current configuration from it
        var switchable = _timeProvider.AsSwitchable();
        if (switchable?.CurrentConfiguration is not null)
        {
            response.ConfiguredDateTime = switchable.CurrentConfiguration.FixedDateTime;
            response.TimeZone = switchable.CurrentConfiguration.TimeZone;
            response.AdvanceTime = switchable.CurrentConfiguration.AdvanceTime;
        }

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

        return Task.FromResult(response);
    }
}
