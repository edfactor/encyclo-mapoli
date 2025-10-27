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
    private readonly ILogger<TerminatedEmployeeReportService> _logger;
    private readonly TotalService _totalService;

    public TerminatedEmployeeService(IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        ILogger<TerminatedEmployeeReportService> logger,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
        _calendarService = calendarService;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(StartAndEndDateRequest req, CancellationToken ct)
    {
        TerminatedEmployeeReportService reportServiceGenerator = new(_dataContextFactory, _totalService, _demographicReaderService, _logger, _calendarService);
        return reportServiceGenerator.CreateDataAsync(req, ct);
    }
}
