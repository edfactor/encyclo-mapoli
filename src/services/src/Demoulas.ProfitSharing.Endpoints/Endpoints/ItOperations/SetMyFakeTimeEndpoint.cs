using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to set the current user's fake time settings.
/// Each user can have their own independent fake time for testing.
/// </summary>
public sealed class SetMyFakeTimeEndpoint : ProfitSharingEndpoint<SetFakeTimeRequest, UserFakeTimeStatusResponse>
{
    private readonly IUserFakeTimeStorage _userFakeTimeStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<SetMyFakeTimeEndpoint> _logger;

    public SetMyFakeTimeEndpoint(
        IUserFakeTimeStorage userFakeTimeStorage,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment hostEnvironment,
        ILogger<SetMyFakeTimeEndpoint> logger) : base(Navigation.Constants.FakeTimeManagement)
    {
        _userFakeTimeStorage = userFakeTimeStorage;
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("my-fake-time");
        Summary(s =>
        {
            s.Summary = "Sets the current user's fake time settings";
            s.Description = "Allows each authenticated user to set their own fake time independently. " +
                "This is useful for testing time-sensitive functionality without affecting other users. " +
                "Set Enabled=false to return to real system time. " +
                "SECURITY: Per-user fake time is never allowed in Production environments.";
            s.ExampleRequest = new SetFakeTimeRequest
            {
                Enabled = true,
                FixedDateTime = "2025-12-15T10:00:00",
                TimeZone = "Eastern Standard Time",
                AdvanceTime = true
            };
        });
        // No group - accessible to all authenticated users to set their own time
    }

    protected override Task<UserFakeTimeStatusResponse> HandleRequestAsync(SetFakeTimeRequest req, CancellationToken ct)
    {
        var isProduction = _hostEnvironment.IsProduction();
        var realNow = DateTimeOffset.Now;
        var userId = GetUserId();

        if (isProduction)
        {
            _logger.LogWarning("Attempted to set per-user fake time in Production environment by user {UserId}", userId);

            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = false,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = "Per-user fake time is not allowed in Production environments."
            });
        }

        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                RealDateTime = realNow.ToString("O"),
                Message = "Unable to identify current user. Per-user fake time requires authentication."
            });
        }

        if (!req.Enabled)
        {
            // Disable fake time for this user
            _userFakeTimeStorage.RemoveSettings(userId);
            _logger.LogInformation("User {UserId} disabled their fake time setting", userId);

            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = "Fake time disabled. You are now using real system time."
            });
        }

        // Validate the configuration
        if (string.IsNullOrWhiteSpace(req.FixedDateTime))
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = "FixedDateTime is required when enabling fake time."
            });
        }

        var config = new FakeTimeConfiguration
        {
            Enabled = true,
            FixedDateTime = req.FixedDateTime,
            TimeZone = req.TimeZone ?? TimeZoneInfo.Local.Id,
            AdvanceTime = req.AdvanceTime
        };

        var validationErrors = config.Validate();
        if (validationErrors.Count > 0)
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = $"Configuration validation failed: {string.Join("; ", validationErrors)}"
            });
        }

        // Create and store user settings
        var settings = new UserFakeTimeSettings { UserId = userId, Configuration = config };
        _userFakeTimeStorage.SetSettings(settings);

        _logger.LogInformation(
            "User {UserId} enabled fake time: {FixedDateTime}, AdvanceTime={AdvanceTime}",
            userId, config.FixedDateTime, config.AdvanceTime);

        var currentFakeTime = settings.GetCurrentFakeTime();

        return Task.FromResult(new UserFakeTimeStatusResponse
        {
            IsActive = true,
            IsAllowed = true,
            CurrentFakeDateTime = currentFakeTime.ToString("O"),
            ConfiguredDateTime = config.FixedDateTime,
            TimeZone = config.TimeZone,
            AdvanceTime = config.AdvanceTime,
            RealDateTime = realNow.ToString("O"),
            UserId = userId,
            Message = $"Fake time activated. You are now operating at {currentFakeTime:MMMM d, yyyy h:mm tt}."
        });
    }

    private string? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        return user?.FindFirst("sub")?.Value
            ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? user?.Identity?.Name;
    }
}
