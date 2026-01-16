using Demoulas.ProfitSharing.Common.Contracts.Response.SystemInfo;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

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

    public GetCurrentTimeEndpoint(
        TimeProvider timeProvider)
        : base(Navigation.Constants.Unknown) // System endpoint without navigation tracking
    {
        _timeProvider = timeProvider;
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

    protected override Task<CurrentTimeResponse> HandleRequestAsync(CancellationToken ct)
    {
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

        return Task.FromResult(response);
    }
}
