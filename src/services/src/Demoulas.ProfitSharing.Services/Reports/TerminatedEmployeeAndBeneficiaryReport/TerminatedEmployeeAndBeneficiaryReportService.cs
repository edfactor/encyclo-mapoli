using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;

    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory, ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReport(ProfitYearRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_dataContextFactory, _calendarService);
        return reportGenerator.CreateData(req, ct);
    }
}
