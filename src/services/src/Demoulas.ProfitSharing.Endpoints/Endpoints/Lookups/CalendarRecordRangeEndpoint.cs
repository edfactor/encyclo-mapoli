using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordRangeEndpoint : ProfitSharingEndpoint<YearRangeRequest, CalendarResponseDto>
{
    private readonly ICalendarService _calendarService;

    public CalendarRecordRangeEndpoint(ICalendarService calendarService) : base(Navigation.Constants.Inquiries)
    {
        _calendarService = calendarService;
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

        if (!Env.IsTestEnvironment())
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<CalendarResponseDto> ExecuteAsync(YearRangeRequest req, CancellationToken ct)
    {
        var startTask = _calendarService.GetYearStartAndEndAccountingDatesAsync(req.BeginProfitYear, ct);
        var endTask = _calendarService.GetYearStartAndEndAccountingDatesAsync(req.EndProfitYear, ct);

        await Task.WhenAll(startTask, endTask);
        
        var start = await startTask;
        var end = await endTask;

        return new CalendarResponseDto { FiscalBeginDate = start.FiscalBeginDate, FiscalEndDate = end.FiscalEndDate };
    }
}
