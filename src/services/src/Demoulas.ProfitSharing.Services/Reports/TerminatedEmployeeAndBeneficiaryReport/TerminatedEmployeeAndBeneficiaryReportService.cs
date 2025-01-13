using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly TotalService _totalService;

    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(ProfitYearRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryReport reportGenerator = new(_dataContextFactory, _calendarService, _totalService);
        return reportGenerator.CreateDataAsync(req, ct);
    }
}
