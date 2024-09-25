using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService(
    IProfitSharingDataContextFactory _dataContextFactory,
    ILoggerFactory factory)
    : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly ILogger<TerminatedEmployeeAndBeneficiaryReportService> _logger = factory.CreateLogger<TerminatedEmployeeAndBeneficiaryReportService>();

    public static DateOnly? useThisForTodaysDateWhenTesting { get; set; }

    public async Task<string> GetReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear, CancellationToken ct)
    {
        string report = "";

        await _dataContextFactory.UseWritableContext(ctx =>
        {
            TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx, useThisForTodaysDateWhenTesting);
            report = reportGenerator.CreateTextReport(startDate, endDate, profitSharingYear);
            return Task.CompletedTask;
        }, ct);

        return report;
    }

    public Task<TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>> GetReport(TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>? data = null;
        
        _dataContextFactory.UseWritableContext(ctx =>
        {
           TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_logger, ctx, useThisForTodaysDateWhenTesting);
           data = reportGenerator.CreateData(req.StartDate, req.EndDate, req.ProfitShareYear);
           return Task.CompletedTask;
        }, ct);

        return Task.FromResult(data!);
    }
}
