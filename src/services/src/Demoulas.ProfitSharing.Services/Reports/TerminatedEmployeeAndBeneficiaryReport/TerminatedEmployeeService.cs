using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeService : ITerminatedEmployeeService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly TotalService _totalService;
    private readonly IYearEndService _yearEndService;

    public TerminatedEmployeeService(IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        ILoggerFactory loggerFactory,
        ICalendarService calendarService,
        IYearEndService yearEndService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _loggerFactory = loggerFactory;
        _calendarService = calendarService;
        _yearEndService = yearEndService;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(StartAndEndDateRequest req, CancellationToken ct)
    {
        var logger = _loggerFactory.CreateLogger<TerminatedEmployeeReportService>();
        TerminatedEmployeeReportService reportServiceGenerator = new(_dataContextFactory, _totalService, _demographicReaderService, logger, _calendarService, _yearEndService);

        // Convert StartAndEndDateRequest to FilterableStartAndEndDateRequest
        var filterableRequest = new FilterableStartAndEndDateRequest
        {
            BeginningDate = req.BeginningDate,
            EndingDate = req.EndingDate,
            Skip = req.Skip,
            Take = req.Take,
            SortBy = req.SortBy,
            IsSortDescending = req.IsSortDescending
        };

        return reportServiceGenerator.CreateDataAsync(filterableRequest, ct);
    }
}
