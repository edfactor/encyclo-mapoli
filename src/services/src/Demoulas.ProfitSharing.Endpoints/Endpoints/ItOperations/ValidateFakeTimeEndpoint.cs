using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to validate fake time configuration settings.
/// Note: Actual configuration changes require updating appsettings and restarting the application.
/// This endpoint validates the proposed configuration and provides guidance.
/// </summary>
public sealed class ValidateFakeTimeEndpoint : ProfitSharingEndpoint<SetFakeTimeRequest, FakeTimeStatusResponse>
{
    private readonly IHostEnvironment _hostEnvironment;

    public ValidateFakeTimeEndpoint(IHostEnvironment hostEnvironment) : base(Navigation.Constants.FakeTimeManagement)
    {
        _hostEnvironment = hostEnvironment;
    }

    public override void Configure()
    {
        Post("fake-time/validate");
        Summary(s =>
        {
            s.Summary = "Validates fake time configuration settings";
            s.Description = "Validates the proposed fake time configuration and returns what would be applied. " +
                "Note: To actually enable fake time, update the FakeTime section in appsettings.json and restart the application. " +
                "SECURITY: Fake time is never allowed in Production environments.";
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
                        IsActive = false,
                        IsAllowed = true,
                        ConfiguredDateTime = "2025-12-15T10:00:00",
                        TimeZone = "Eastern Standard Time",
                        AdvanceTime = false,
                        Environment = "Development",
                        RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                        Message = "Configuration is valid. To apply, update appsettings.json and restart the application."
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

        if (isProduction)
        {
            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = false,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = "SECURITY: Fake time is not allowed in Production environments. This request has been rejected."
            });
        }

        var proposedConfig = new FakeTimeConfiguration
        {
            Enabled = req.Enabled,
            FixedDateTime = req.FixedDateTime,
            TimeZone = req.TimeZone,
            AdvanceTime = req.AdvanceTime
        };

        var validationErrors = proposedConfig.Validate();

        if (validationErrors.Count > 0)
        {
            return Task.FromResult(new FakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                ConfiguredDateTime = req.FixedDateTime,
                TimeZone = req.TimeZone,
                AdvanceTime = req.AdvanceTime,
                Environment = _hostEnvironment.EnvironmentName,
                RealDateTime = realNow.ToString("O"),
                Message = $"Configuration validation failed: {string.Join("; ", validationErrors)}"
            });
        }

        var response = new FakeTimeStatusResponse
        {
            IsActive = false,
            IsAllowed = true,
            ConfiguredDateTime = req.FixedDateTime,
            TimeZone = req.TimeZone ?? TimeZoneInfo.Local.Id,
            AdvanceTime = req.AdvanceTime,
            Environment = _hostEnvironment.EnvironmentName,
            RealDateTime = realNow.ToString("O")
        };

        if (req.Enabled)
        {
            var parsedTime = proposedConfig.GetParsedFixedDateTime();
            var timeZone = proposedConfig.GetTimeZone();
            response.Message = $"Configuration is valid. To enable fake time at {parsedTime:MMMM d, yyyy h:mm tt} ({timeZone.Id}), " +
                "update the FakeTime section in appsettings.json and restart the application.";
        }
        else
        {
            response.Message = "Configuration is valid. To disable fake time, set FakeTime.Enabled to false in appsettings.json and restart the application.";
        }

        return Task.FromResult(response);
    }
}
