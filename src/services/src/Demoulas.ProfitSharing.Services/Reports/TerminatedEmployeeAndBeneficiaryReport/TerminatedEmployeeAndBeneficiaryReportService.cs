using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeAndBeneficiaryReportService : ITerminatedEmployeeAndBeneficiaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    
    public TerminatedEmployeeAndBeneficiaryReportService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReport(ProfitYearRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryReport reportGenerator = new TerminatedEmployeeAndBeneficiaryReport(_dataContextFactory);
        return reportGenerator.CreateData(req, ct);
    }
}
