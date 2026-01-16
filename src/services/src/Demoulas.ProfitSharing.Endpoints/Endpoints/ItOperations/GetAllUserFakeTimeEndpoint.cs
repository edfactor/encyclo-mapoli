using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Admin endpoint to list all users with fake time settings.
/// Restricted to IT DevOps role.
/// </summary>
public sealed class GetAllUserFakeTimeEndpoint : ProfitSharingResponseEndpoint<AllUserFakeTimeResponse>
{
    private readonly IUserFakeTimeStorage _userFakeTimeStorage;
    private readonly IHostEnvironment _hostEnvironment;

    public GetAllUserFakeTimeEndpoint(
        IUserFakeTimeStorage userFakeTimeStorage,
        IHostEnvironment hostEnvironment) : base(Navigation.Constants.FakeTimeManagement)
    {
        _userFakeTimeStorage = userFakeTimeStorage;
        _hostEnvironment = hostEnvironment;
    }

    public override void Configure()
    {
        Get("fake-time/users");
        Group<ItDevOpsGroup>();
        Summary(s =>
        {
            s.Summary = "Lists all users with fake time settings";
            s.Description = "Returns a list of all users who have configured per-user fake time settings. " +
                "Useful for administrators to monitor testing activities. " +
                "SECURITY: Restricted to IT DevOps role.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    StatusCodes.Status200OK,
                    new AllUserFakeTimeResponse
                    {
                        TotalUsers = 2,
                        Users = new List<UserFakeTimeStatusResponse>
                        {
                            new()
                            {
                                IsActive = true,
                                IsAllowed = true,
                                CurrentFakeDateTime = "2025-12-14T10:00:00.0000000-05:00",
                                ConfiguredDateTime = "2025-12-14T10:00:00",
                                TimeZone = "Eastern Standard Time",
                                AdvanceTime = true,
                                RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                                UserId = "mike@example.com",
                                Message = "Fake time active at December 14, 2025."
                            },
                            new()
                            {
                                IsActive = true,
                                IsAllowed = true,
                                CurrentFakeDateTime = "2025-12-22T10:00:00.0000000-05:00",
                                ConfiguredDateTime = "2025-12-22T10:00:00",
                                TimeZone = "Eastern Standard Time",
                                AdvanceTime = false,
                                RealDateTime = "2026-01-05T14:30:00.0000000-05:00",
                                UserId = "penelope@example.com",
                                Message = "Fake time active at December 22, 2025."
                            }
                        }
                    }
                }
            };
        });
    }

    protected override Task<AllUserFakeTimeResponse> HandleRequestAsync(CancellationToken ct)
    {
        var realNow = DateTimeOffset.Now;
        var isAllowed = !_hostEnvironment.IsProduction();

        var allSettings = _userFakeTimeStorage.GetAllSettings();
        var userResponses = new List<UserFakeTimeStatusResponse>();

        foreach (var settings in allSettings)
        {
            var currentFakeTime = settings.GetCurrentFakeTime();

            userResponses.Add(new UserFakeTimeStatusResponse
            {
                IsActive = true,
                IsAllowed = isAllowed,
                CurrentFakeDateTime = currentFakeTime.ToString("O"),
                ConfiguredDateTime = settings.Configuration.FixedDateTime,
                TimeZone = settings.Configuration.TimeZone,
                AdvanceTime = settings.Configuration.AdvanceTime,
                RealDateTime = realNow.ToString("O"),
                UserId = settings.UserId,
                Message = $"Fake time active at {currentFakeTime:MMMM d, yyyy h:mm tt}."
            });
        }

        return Task.FromResult(new AllUserFakeTimeResponse
        {
            TotalUsers = userResponses.Count,
            Users = userResponses
        });
    }
}
