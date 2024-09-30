using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<TerminatedEmployeeAndBeneficiaryReportService> _logger;
    private static DateOnly? _todaysDate;

    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory, ILoggerFactory loggerFactory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = loggerFactory.CreateLogger<TerminatedEmployeeAndBeneficiaryReportService>();
    }
    

    public  Task<TerminatedEmployeeAndBeneficiaryResponse>
        GetReport(TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken ct)
    {
        return _dataContextFactory
            .UseReadOnlyContext<TerminatedEmployeeAndBeneficiaryResponse>(ctx =>
            {
                TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx, _todaysDate ?? DateOnly.FromDateTime(DateTime.Today));
                return  reportGenerator.CreateData(req.StartDate, req.EndDate, req.ProfitShareYear);
            });
    }

    // Used only when testing to fix the Age of members
    public static void SetTodayDateForTestingOnly(DateOnly todaysDate)
    {
        _todaysDate = todaysDate;
    }
}
