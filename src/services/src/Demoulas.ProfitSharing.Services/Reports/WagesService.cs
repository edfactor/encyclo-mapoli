using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public class WagesService : IWagesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;

    public WagesService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesReportAsync(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        
        var result = await _dataContextFactory.UseReadOnlyContext(async c =>
        {
            // Get demographics query (currently uses live data, but structured to support frozen data in future)
            var demographics = await _demographicReaderService.BuildDemographicQuery(c, useFrozenData: true);

            // Join demographics with PayProfits to get wage data
            var query = from d in demographics
                        join pp in c.PayProfits.Where(p => p.CurrentIncomeYear != 0 && p.ProfitYear == request.ProfitYear)
                            on d.Id equals pp.DemographicId
                        select new WagesCurrentYearResponse
                        {
                            BadgeNumber = d.BadgeNumber,
                            HoursCurrentYear = pp.CurrentHoursYear,
                            IncomeCurrentYear = pp.CurrentIncomeYear,
                            StoreNumber = d.StoreNumber,
                            IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                        };

            return await query.ToPaginationResultsAsync(request, cancellationToken);
        }, cancellationToken);

        return new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"YTD Wages Extract (PROF-DOLLAR-EXTRACT) - {request.ProfitYear}",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            Response = result
        };
    }
}
