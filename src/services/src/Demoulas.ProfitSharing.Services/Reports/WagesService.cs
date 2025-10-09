using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public class WagesService : IWagesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;

    public WagesService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    public async Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesReportAsync(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.CurrentIncomeYear != 0 && p.ProfitYear == request.ProfitYear)
                .Select(p => new WagesCurrentYearResponse
                {
                    BadgeNumber = p.Demographic!.BadgeNumber,
                    HoursCurrentYear = p.CurrentHoursYear,
                    IncomeCurrentYear = p.CurrentIncomeYear,
                    StoreNumber = p.Demographic.StoreNumber,
                    IsExecutive = p.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly
                })
                .ToPaginationResultsAsync(request, cancellationToken);

        }, cancellationToken);

        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        return new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"YTD Wages Extract (PROF-DOLLAR-EXTRACT) - {request.ProfitYear}",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            Response = await result
        };
    }
}
