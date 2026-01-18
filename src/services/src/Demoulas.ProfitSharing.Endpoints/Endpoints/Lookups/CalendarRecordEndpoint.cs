using Demoulas.ProfitSharing.Common.Contracts; // for Result<T>
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
// for ToHttpResult / ToResultOrNotFound
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

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
    }

    protected override async Task<Results<Ok<CalendarResponseDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        YearRequest req,
        CancellationToken ct)
    {
        var result = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, ct);
        return result.ToResultOrNotFound(Error.CalendarYearNotFound)
            .ToHttpResult(Error.CalendarYearNotFound);
    }
}
