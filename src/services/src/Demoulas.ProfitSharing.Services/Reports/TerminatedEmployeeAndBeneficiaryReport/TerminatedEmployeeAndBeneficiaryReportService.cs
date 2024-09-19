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

    public async Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct)
    {
        string report = "";

        await dataContextFactory.UseWritableContext(ctx =>
        {
            TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx);
            report = reportGenerator.CreateReport(startDate, endDate, profitSharingYear);
            return Task.CompletedTask;
        }, ct);

        return report;
    }



}
