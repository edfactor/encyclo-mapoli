using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Endpoint to clear/remove the current user's fake time settings.
/// After this, the user will use real system time.
/// </summary>
public sealed class DeleteMyFakeTimeEndpoint : ProfitSharingResponseEndpoint<UserFakeTimeStatusResponse>
{
    private readonly IUserFakeTimeStorage _userFakeTimeStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<DeleteMyFakeTimeEndpoint> _logger;

    public DeleteMyFakeTimeEndpoint(
        IUserFakeTimeStorage userFakeTimeStorage,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment hostEnvironment,
        ILogger<DeleteMyFakeTimeEndpoint> logger) : base(Navigation.Constants.FakeTimeManagement)
    {
        _userFakeTimeStorage = userFakeTimeStorage;
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("my-fake-time");
        Summary(s =>
        {
            s.Summary = "Removes the current user's fake time settings";
            s.Description = "Clears any fake time configuration for the authenticated user, returning them to real system time. " +
                "This is equivalent to setting Enabled=false via the POST endpoint.";
        });
        // No group - accessible to all authenticated users
    }

    protected override Task<UserFakeTimeStatusResponse> HandleRequestAsync(CancellationToken ct)
    {
        var realNow = DateTimeOffset.Now;
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult(new UserFakeTimeStatusResponse
            {
                IsActive = false,
                IsAllowed = !_hostEnvironment.IsProduction(),
                RealDateTime = realNow.ToString("O"),
                Message = "Unable to identify current user."
            });
        }

        var hadSettings = _userFakeTimeStorage.RemoveSettings(userId);

        if (hadSettings)
        {
            _logger.LogInformation("User {UserId} removed their fake time settings", userId);
        }

        return Task.FromResult(new UserFakeTimeStatusResponse
        {
            IsActive = false,
            IsAllowed = !_hostEnvironment.IsProduction(),
            RealDateTime = realNow.ToString("O"),
            UserId = userId,
            Message = hadSettings
                ? "Fake time settings removed. You are now using real system time."
                : "No fake time settings were configured."
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
