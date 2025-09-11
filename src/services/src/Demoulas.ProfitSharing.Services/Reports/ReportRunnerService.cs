using System.Diagnostics;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports;

public class ReportRunnerService : IReportRunnerService
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;
    private readonly Dictionary<string, Func<CancellationToken, Task<Dictionary<string, object>>>> _reports;
    private readonly ITerminatedEmployeeService _terminatedEmployeeService;
    private readonly IUnforfeitService _unForfeitService;

    public ReportRunnerService(
        ITerminatedEmployeeService terminatedEmployeeService,
        IProfitShareUpdateService profitShareUpdateService,
        IUnforfeitService unForfeitService)
    {
        _terminatedEmployeeService = terminatedEmployeeService;
        _profitShareUpdateService = profitShareUpdateService;
        _unForfeitService = unForfeitService;

        _reports = new Dictionary<string, Func<CancellationToken, Task<Dictionary<string, object>>>>
        {
            ["terminations"] = async ct => await Handle("terminations", ct, async () =>
            {
                TerminatedEmployeeAndBeneficiaryResponse result = await _terminatedEmployeeService
                    .GetReportAsync(StartAndEndDateRequest.RequestExample(), ct);
                return (result.Response.Total, result.Response.Results.Count());
            }),
            ["profitShareUpdate"] = async ct => await Handle("profitShareUpdate", ct, async () =>
            {
                ProfitShareUpdateResponse result = await _profitShareUpdateService
                    .ProfitShareUpdate(ProfitShareUpdateRequest.RequestExample(), ct);
                return (result.Response.Total, result.Response.Results.Count());
            }),
            ["unforfeit"] = async ct => await Handle("unforfeit", ct, async () =>
            {
                ReportResponseBase<UnforfeituresResponse> result = await _unForfeitService
                    .FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(
                        StartAndEndDateRequest.RequestExample(), ct);
                return (result.Response.Total, result.Response.Results.Count());
            })
        };

        // Special "all" entry runs all
        _reports["all"] = async ct =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Dictionary<string, object>> allResults = new();

            foreach (KeyValuePair<string, Func<CancellationToken, Task<Dictionary<string, object>>>> kvp in _reports.Where(r => r.Key != "all"))
            {
                allResults.Add(await kvp.Value(ct));
            }

            sw.Stop();

            return new Dictionary<string, object> { ["name"] = "all", ["reports"] = allResults, ["tookSeconds"] = sw.Elapsed.TotalSeconds };
        };
    }

    public async Task<Dictionary<string, object>> IncludeReportInformation(
        string? reportSelector, CancellationToken cancellationToken)
    {
        if (reportSelector != null && _reports.TryGetValue(reportSelector, out Func<CancellationToken, Task<Dictionary<string, object>>>? runner))
        {
            return await runner(cancellationToken);
        }

        return new Dictionary<string, object> { ["error"] = "report not found" };
    }

    private async Task<Dictionary<string, object>> Handle(
        string reportSelector,
        CancellationToken cancellationToken,
        Func<Task<(long Total, int)>> func)
    {
        Dictionary<string, object> data = new() { ["name"] = reportSelector };

        Stopwatch took = Stopwatch.StartNew();
        try
        {
            (long total, int pageCount) = await func();
            data["results.total"] = total;
            data["results.page.count"] = pageCount;
        }
        catch (Exception ex)
        {
            data["error"] = ex.ToString();
        }

        took.Stop();
        data["tookSeconds"] = took.Elapsed.TotalSeconds;
        return data;
    }
}
