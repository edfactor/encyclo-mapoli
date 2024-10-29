using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Services;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CalendarRecordEndpoint : Endpoint<CalendarRequestDto, CalendarResponseDto>
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

    public override Task<CalendarResponseDto> ExecuteAsync(CalendarRequestDto req, CancellationToken ct)
    {
        return _calendarService.GetYearStartAndEndAccountingDates(req.ProfitYear, ct);
    }
}
