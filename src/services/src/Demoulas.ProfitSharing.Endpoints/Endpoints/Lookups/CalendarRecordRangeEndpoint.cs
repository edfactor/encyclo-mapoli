using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordRangeEndpoint : ProfitSharingEndpoint<YearRangeRequest, Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CalendarRecordRangeEndpoint> _logger;

    public CalendarRecordRangeEndpoint(ICalendarService calendarService, ILogger<CalendarRecordRangeEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("calendar/accounting-range");
        Summary(s =>
        {
            s.Summary = "Gets the starting and ending accounting dates for a given profit year range.";
            s.Description = "Returns the fiscal begin and end dates for the specified start and end profit years. " +
                            "Both years must be provided as query string parameters. " +
                            "Example: /calendar/accounting-range?BeginProfitYear=2024&EndProfitYear=2025";
            s.ExampleRequest = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, new CalendarResponseDto
                    {
                        FiscalBeginDate = new DateOnly(2024, 01, 07),
                        FiscalEndDate = new DateOnly(2025, 01, 05)
                    }
                }
            };
            s.Responses[400] = "Bad Request. Both BeginProfitYear and EndProfitYear must be valid years.";
            s.Responses[404] = "Not Found. No accounting dates found for the specified years.";
        });
        Group<LookupGroup>();

        // Output caching: Accounting calendar dates are stable reference data - excellent caching candidate
        // Cache disabled in test environments to ensure test data freshness
        if (!Env.IsTestEnvironment())
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(10); // Moderate duration - reference data changes infrequently
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override Task<Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(YearRangeRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var startTask = _calendarService.GetYearStartAndEndAccountingDatesAsync(req.BeginProfitYear, ct);
            var endTask = _calendarService.GetYearStartAndEndAccountingDatesAsync(req.EndProfitYear, ct);

            await Task.WhenAll(startTask, endTask);

            var start = await startTask;
            var end = await endTask;

            // Record cache metrics
            var cacheStatus = HttpContext.Response.Headers.ContainsKey("X-Cache") ? "hit" : "miss";
            if (cacheStatus == "hit")
            {
                EndpointTelemetry.CacheHitsTotal.Add(1,
                    new KeyValuePair<string, object?>("cache_type", "output-cache"),
                    new KeyValuePair<string, object?>("endpoint", "CalendarRecordRangeEndpoint"));
            }
            else
            {
                EndpointTelemetry.CacheMissesTotal.Add(1,
                    new KeyValuePair<string, object?>("cache_type", "output-cache"),
                    new KeyValuePair<string, object?>("endpoint", "CalendarRecordRangeEndpoint"));
            }

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "calendar-range-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            // Basic not-found semantics: if either side returns default dates (00/00) treat as not found.
            if (start.FiscalBeginDate == default || end.FiscalEndDate == default)
            {
                _logger.LogWarning("Calendar year range lookup failed for years {BeginYear}-{EndYear}, cache status: {CacheStatus} (correlation: {CorrelationId})",
                    req.BeginProfitYear, req.EndProfitYear, cacheStatus, HttpContext.TraceIdentifier);

                return Result<CalendarResponseDto>.Failure(Error.CalendarYearNotFound)
                    .ToHttpResult(Error.CalendarYearNotFound);
            }

            _logger.LogInformation("Calendar range lookup completed for years {BeginYear}-{EndYear}, cache status: {CacheStatus} (correlation: {CorrelationId})",
                req.BeginProfitYear, req.EndProfitYear, cacheStatus, HttpContext.TraceIdentifier);

            var dto = new CalendarResponseDto { FiscalBeginDate = start.FiscalBeginDate, FiscalEndDate = end.FiscalEndDate };
            return Result<CalendarResponseDto>.Success(dto).ToHttpResult();
        });
    }
}
