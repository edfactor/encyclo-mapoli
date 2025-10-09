using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

public class TerminatedEmployeeService : ITerminatedEmployeeService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<TerminatedEmployeeReportService> _logger;

    public TerminatedEmployeeService(IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        ILogger<TerminatedEmployeeReportService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
    }


    public Task<TerminatedEmployeeAndBeneficiaryResponse> GetReportAsync(StartAndEndDateRequest req, CancellationToken ct)
    {
        TerminatedEmployeeReportService reportServiceGenerator = new(_dataContextFactory, _totalService, _demographicReaderService, _logger);
        return reportServiceGenerator.CreateDataAsync(req, ct);
    }


}
