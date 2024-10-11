using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<TerminatedEmployeeAndBeneficiaryReportService> _logger;
    
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
                TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx);
                return  reportGenerator.CreateData(req);
            });
    }
}
