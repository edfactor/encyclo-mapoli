using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<TerminatedEmployeeAndBeneficiaryReportService> _logger;

    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory, ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = factory.CreateLogger<TerminatedEmployeeAndBeneficiaryReportService>();
    }
    public async Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct)
    {

       return await _dataContextFactory.UseWritableContext(async ctx =>
        {
            // TerminatedEmployeeAndBeneficiaryReport is not async, so we add this into satisfy the method signature.
            await Task.Delay(1, ct);

            TerminatedEmployeeAndBeneficiaryReport terminatedEmployeeAndBeneficiaryReport = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx);
            string report = terminatedEmployeeAndBeneficiaryReport.CreateReport( startDate, endDate, profitSharingYear);
            return report;
        }, ct);

    }
}
