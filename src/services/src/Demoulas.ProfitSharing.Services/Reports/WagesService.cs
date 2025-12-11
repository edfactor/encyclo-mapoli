using Demoulas.Common.Contracts.Contracts.Response;
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
    private readonly ICalendarService _calendar_service;
    private readonly IDemographicReaderService _demographicReaderService;

    public WagesService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendar_service = calendarService;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesReportAsync(WagesCurrentYearRequest request, CancellationToken cancellationToken)
    {
        var calInfo = await _calendar_service.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

        var result = await _dataContextFactory.UseReadOnlyContext(async c =>
        {
            // Get demographics query (uses frozen data if UseFrozenData flag is true)
            var demographics = await _demographicReaderService.BuildDemographicQuery(c, useFrozenData: request.UseFrozenData);

            // Filter pay profits for the profit year and non-zero income
            var payProfitsForYear = c.PayProfits
                .Where(p => p.CurrentIncomeYear != 0 && p.ProfitYear == request.ProfitYear);

            // Join demographics with PayProfits to get wage data as a lightweight projection
            var joinQuery = from d in demographics
                            join pp in payProfitsForYear
                                on d.Id equals pp.DemographicId
                            select new
                            {
                                d.BadgeNumber,
                                Hours = pp.CurrentHoursYear,
                                Income = pp.CurrentIncomeYear,
                                d.StoreNumber,
                                IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                            };

            // Compute totals across the entire (filtered) dataset
            var totalHours = await joinQuery.SumAsync(x => x.Hours, cancellationToken);
            var totalIncome = await joinQuery.SumAsync(x => x.Income, cancellationToken);

            // Project into participant DTO
            var participantProjection = joinQuery.Select(x => new WagesCurrentYearParticipant
            {
                BadgeNumber = x.BadgeNumber,
                HoursCurrentYear = x.Hours,
                IncomeCurrentYear = x.Income,
                StoreNumber = x.StoreNumber,
                IsExecutive = x.IsExecutive
            });

            // Get paginated participants using existing pagination helper
            var paginatedParticipants = await participantProjection.ToPaginationResultsAsync(request, cancellationToken);

            // Build container response with totals and participant list (paginated results placed in Participants)
            var container = new WagesCurrentYearResponse
            {
                TotalHoursCurrentYearWages = totalHours,
                TotalIncomeCurrentYearWages = totalIncome,
                Participants = paginatedParticipants.Results.ToList()
            };

            // Return a paginated response containing the single container (keeps existing ReportResponseBase shape)
            return new PaginatedResponseDto<WagesCurrentYearResponse>
            {
                Results = new List<WagesCurrentYearResponse> { container }
            };
        }, cancellationToken);

        // Add " - Archive" suffix when using frozen data
        var reportNameSuffix = request.UseFrozenData ? " - Archive" : string.Empty;

        return new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"YTD Wages Extract (PROF-DOLLAR-EXTRACT) - {request.ProfitYear}{reportNameSuffix}",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            Response = result
        };
    }
}
