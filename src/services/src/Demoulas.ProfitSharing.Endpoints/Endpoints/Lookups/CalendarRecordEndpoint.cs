using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts; // for Result<T>
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions; // for ToHttpResult / ToResultOrNotFound
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordEndpoint : ProfitSharingEndpoint<YearRequest, Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CalendarRecordEndpoint> _logger;

    public CalendarRecordEndpoint(ICalendarService calendarService, ILogger<CalendarRecordEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _calendarService = calendarService;
        _logger = logger;
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

    public override Task<Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Service currently returns a raw DTO; wrap into domain Result pattern for consistency
            var dto = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, ct).ConfigureAwait(false);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "calendar-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            _logger.LogInformation("Calendar lookup completed for profit year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

            return dto
                .ToResultOrNotFound(Error.CalendarYearNotFound)
                .ToHttpResult(Error.CalendarYearNotFound);
        });
    }
}
