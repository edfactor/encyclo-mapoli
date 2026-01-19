using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to get the current user's fake time status.
/// This is user-specific - each user can have their own fake time settings.
/// </summary>
public sealed class GetMyFakeTimeEndpoint : ProfitSharingResponseEndpoint<UserFakeTimeStatusResponse>
{
    private readonly IUserFakeTimeStorage _userFakeTimeStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _hostEnvironment;

    public GetMyFakeTimeEndpoint(
        IUserFakeTimeStorage userFakeTimeStorage,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment hostEnvironment) : base(Navigation.Constants.FakeTimeManagement)
    {
        _userFakeTimeStorage = userFakeTimeStorage;
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
    }

    public override void Configure()
    {
        Get("my-fake-time");
        Summary(s =>
        {
            s.Summary = "Gets the current user's fake time settings";
            s.Description = "Returns the fake time configuration specific to the authenticated user. " +
                "Each user can have their own fake time setting independent of the global server time and other users. " +
                "SECURITY: Per-user fake time is never allowed in Production environments.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    StatusCodes.Status200OK,
                    new UserFakeTimeStatusResponse
                    {
                        IsActive = true,
                        IsAllowed = true,
                        CurrentFakeDateTime = "2025-12-15T10:00:00.0000000-05:00",
                        ConfiguredDateTime = "2025-12-15T10:00:00",
                        TimeZone = "Eastern Standard Time",
                        AdvanceTime = true,
                        RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                        UserId = "user@example.com",
                        Message = "Your fake time is active. You are operating at December 15, 2025."
                    }
                }
            };
        });
        // No group - accessible to all authenticated users to check their own status
    }

    protected override Task<UserFakeTimeStatusResponse> HandleRequestAsync(CancellationToken ct)
    {
        var isProduction = _hostEnvironment.IsProduction();
        var realNow = DateTimeOffset.Now;
        var userId = GetUserId();

        if (isProduction)
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = false,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = "Per-user fake time is disabled. This is a Production environment."
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

        var settings = _userFakeTimeStorage.GetSettings(userId);

        if (settings is null)
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = true,
                RealDateTime = realNow.ToString("O"),
                UserId = userId,
                Message = "You are using real system time. No fake time settings configured."
            });
        }

        var currentFakeTime = settings.GetCurrentFakeTime();

        return Task.FromResult(new UserFakeTimeStatusResponse
        {
            IsActive = true,
            IsAllowed = true,
            CurrentFakeDateTime = currentFakeTime.ToString("O"),
            ConfiguredDateTime = settings.Configuration.FixedDateTime,
            TimeZone = settings.Configuration.TimeZone,
            AdvanceTime = settings.Configuration.AdvanceTime,
            RealDateTime = realNow.ToString("O"),
            UserId = userId,
            Message = $"Your fake time is active. You are operating at {currentFakeTime:MMMM d, yyyy h:mm tt}."
        });
    }

    private string? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        // Try standard claims first
        var userId = user?.FindFirst("sub")?.Value
            ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? user?.Identity?.Name;

        return userId;
    }
}
