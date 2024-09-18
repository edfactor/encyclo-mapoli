using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService(
    IProfitSharingDataContextFactory dataContextFactory,
    ILoggerFactory factory)
    : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly ILogger<TerminatedEmployeeAndBeneficiaryReportService> _logger = factory.CreateLogger<TerminatedEmployeeAndBeneficiaryReportService>();

    public Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct)
    {
        return (Task<string>)dataContextFactory.UseWritableContext(ctx =>
        {
            TerminatedEmployeeAndBeneficiaryReport terminatedEmployeeAndBeneficiaryReport = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx);
            string report = terminatedEmployeeAndBeneficiaryReport.CreateReport(startDate, endDate, profitSharingYear);
            return Task.FromResult(report);
        }, ct);
    }

}
