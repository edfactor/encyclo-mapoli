using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // for Result<T>
using Demoulas.ProfitSharing.Common.Extensions; // for ToHttpResult / ToResultOrNotFound
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordEndpoint : ProfitSharingEndpoint<YearRequest, Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly ICalendarService _calendarService;

    public CalendarRecordEndpoint(ICalendarService calendarService) : base(Navigation.Constants.Inquiries)
    {
        _calendarService = calendarService;
    }

    public override void Configure()
    {
        Get("calendar/accounting-year");
        Summary(s =>
        {
            s.Summary = "Gets the starting and ending dates for a given ProfitYear";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<CalendarResponseDto>
                {
                    new CalendarResponseDto { FiscalBeginDate = new DateOnly(2024, 01, 07), FiscalEndDate = new DateOnly(2025, 01, 05)}
                }
            } };
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        try
        {
            // Service currently returns a raw DTO; wrap into domain Result pattern for consistency
            var dto = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, ct).ConfigureAwait(false);
            return dto
                .ToResultOrNotFound(Error.CalendarYearNotFound)
                .ToHttpResult(Error.CalendarYearNotFound);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
