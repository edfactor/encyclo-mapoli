using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to validate fake time configuration settings.
/// Note: Actual configuration changes require updating appsettings and restarting the application.
/// This endpoint validates the proposed configuration and provides guidance.
/// </summary>
public sealed class ValidateFakeTimeEndpoint : ProfitSharingEndpoint<SetFakeTimeRequest, FakeTimeStatusResponse>
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<ValidateFakeTimeEndpoint> _logger;

    public ValidateFakeTimeEndpoint(
        IHostEnvironment hostEnvironment,
        ILogger<ValidateFakeTimeEndpoint> logger) : base(Navigation.Constants.FakeTimeManagement)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
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

    public override Task<FakeTimeStatusResponse> ExecuteAsync(SetFakeTimeRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var isProduction = _hostEnvironment.IsProduction();
            var realNow = DateTimeOffset.Now;

            // Security check: Never allow in production
            if (isProduction)
            {
                _logger.LogWarning(
                    "Fake time configuration validation rejected - Production environment (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);

                return new FakeTimeStatusResponse
                {
                    IsActive = false,
                    IsAllowed = false,
                    Environment = _hostEnvironment.EnvironmentName,
                    RealDateTime = realNow.ToString("O"),
                    Message = "SECURITY: Fake time is not allowed in Production environments. This request has been rejected."
                };
            }

            // Validate the proposed configuration
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
                _logger.LogWarning(
                    "Fake time configuration validation failed: {Errors} (correlation: {CorrelationId})",
                    string.Join("; ", validationErrors), HttpContext.TraceIdentifier);

                return new FakeTimeStatusResponse
                {
                    IsActive = false,
                    IsAllowed = true,
                    ConfiguredDateTime = req.FixedDateTime,
                    TimeZone = req.TimeZone,
                    AdvanceTime = req.AdvanceTime,
                    Environment = _hostEnvironment.EnvironmentName,
                    RealDateTime = realNow.ToString("O"),
                    Message = $"Configuration validation failed: {string.Join("; ", validationErrors)}"
                };
            }

            // Configuration is valid
            var response = new FakeTimeStatusResponse
            {
                IsActive = false, // Not yet active - requires restart
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

            // Record telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "fake-time-config-validate"),
                new("endpoint", nameof(ValidateFakeTimeEndpoint)),
                new("proposed_enabled", req.Enabled.ToString().ToLowerInvariant()));

            _logger.LogInformation(
                "Fake time configuration validated. Proposed Enabled: {Enabled}, DateTime: {DateTime}, TimeZone: {TimeZone} (correlation: {CorrelationId})",
                req.Enabled, req.FixedDateTime ?? "not set", req.TimeZone ?? "system default", HttpContext.TraceIdentifier);

            return await Task.FromResult(response);
        });
    }
}
