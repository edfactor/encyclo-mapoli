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
    private readonly ContributionService _contributionService;

    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory, 
        ICalendarService calendarService,
        TotalService totalService,
        ContributionService contributionService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _contributionService = contributionService;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(ProfitYearRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryReport reportGenerator = new (_dataContextFactory, _calendarService, _totalService, _contributionService);
        return reportGenerator.CreateDataAsync(req, ct);
    }
}
