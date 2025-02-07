using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordEndpoint : Endpoint<YearRequest, CalendarResponseDto>
{
    private readonly ICalendarService _calendarService;

    public CalendarRecordEndpoint(ICalendarService calendarService)
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
    }

    public override Task<CalendarResponseDto> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, ct);
    }
}
