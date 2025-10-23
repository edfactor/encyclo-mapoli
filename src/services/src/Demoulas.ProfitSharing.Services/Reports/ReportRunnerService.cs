using System.Diagnostics;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
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
    private readonly ICleanupReportService _cleanupReportService;
    private readonly IDuplicateNamesAndBirthdaysService _duplicateNamesAndBirthdaysService;
    private readonly IPayrollDuplicateSsnReportService _payrollDuplicateSsnReportService;
    private readonly INegativeEtvaReportService _negativeEtvaReportService;
    private readonly IGetEligibleEmployeesService _getEligibleEmployeesService;
    private readonly IBreakdownService _breakdownService;
    private readonly IFrozenReportService _frozenReportService;
    private readonly IMasterInquiryService _masterInquiryService;
    private readonly IAdhocBeneficiariesReport _adhocBeneficiariesReport;


    public ReportRunnerService(ITerminatedEmployeeService terminatedEmployeeService,
        IProfitShareUpdateService profitShareUpdateService,
        IUnforfeitService unForfeitService,
        ICleanupReportService cleanupReportService,
        IDuplicateNamesAndBirthdaysService duplicateNamesAndBirthdaysService,
        IPayrollDuplicateSsnReportService payrollDuplicateSsnReportService,
        INegativeEtvaReportService negativeEtvaReportService,
        IGetEligibleEmployeesService getEligibleEmployeesService,
        IBreakdownService breakdownService,
        IFrozenReportService frozenReportService,
        IMasterInquiryService masterInquiryService,
        IAdhocBeneficiariesReport adhocBeneficiariesReport)
    {
        _terminatedEmployeeService = terminatedEmployeeService;
        _profitShareUpdateService = profitShareUpdateService;
        _unForfeitService = unForfeitService;
        _cleanupReportService = cleanupReportService;
        _duplicateNamesAndBirthdaysService = duplicateNamesAndBirthdaysService;
        _payrollDuplicateSsnReportService = payrollDuplicateSsnReportService;
        _negativeEtvaReportService = negativeEtvaReportService;
        _getEligibleEmployeesService = getEligibleEmployeesService;
        _breakdownService = breakdownService;
        _frozenReportService = frozenReportService;
        _masterInquiryService = masterInquiryService;
        _adhocBeneficiariesReport = adhocBeneficiariesReport;
        short wallClockYear = (short)DateTime.Now.Year;

        _reports = new Dictionary<string, Func<CancellationToken, Task<Dictionary<string, object>>>>
        {
            ["demographicBadgesNotInPayProfit"] = async ct => await Handle("demographicBadgesNotInPayProfit", ct, async () =>
            {
                var r = await _cleanupReportService.GetDemographicBadgesNotInPayProfitAsync(new ProfitYearRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["duplicateNamesAndBirthdays"] = async ct => await Handle("duplicateNamesAndBirthdays", ct, async () =>
            {
                var r = await _duplicateNamesAndBirthdaysService.GetDuplicateNamesAndBirthdaysAsync(new ProfitYearRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["negativeEtvaReportService"] = async ct => await Handle("negativeEtvaReportService", ct, async () =>
            {
                var r = await _negativeEtvaReportService.GetNegativeETVAForSsNsOnPayProfitResponseAsync(new ProfitYearRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["distributionsAndForfeiture"] = async ct => await Handle("distributionsAndForfeiture", ct, async () =>
            {
                var result = await _cleanupReportService.GetDistributionsAndForfeitureAsync(new DistributionsAndForfeituresRequest
                {
                    StartDate = new DateOnly(wallClockYear, 1, 1),
                    EndDate = new DateOnly(wallClockYear, 12, 31)
                }, ct);

                if (result.IsError)
                {
                    return (0, 0); // Return empty counts when validation fails
                }

                var r = result.Value!;
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["duplicateSsns"] = async ct => await Handle("duplicateSsns", ct, async () =>
            {
                var r = await _payrollDuplicateSsnReportService.GetDuplicateSsnAsync(new(), ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
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
            }),
            ["eligibleEmployees"] = async ct => await Handle("eligibleEmployees", ct, async () =>
            {
                var r = await _getEligibleEmployeesService.GetEligibleEmployeesAsync(new ProfitYearRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["breakdownByStore"] = async ct => await Handle("breakdownByStore", ct, async () =>
            {
                var r = await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["balanceByAge"] = async ct => await Handle("balanceByAge", ct, async () =>
            {
                var r = await _frozenReportService.GetBalanceByAgeYearAsync(new FrozenReportsByAgeRequest { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
            }),
            ["masterInquirySearch"] = async ct => await Handle("masterInquirySearch", ct, async () =>
            {
                var r = await _masterInquiryService.GetMembersAsync(new MasterInquiryRequest { ProfitYear = wallClockYear }, ct);
                return (r.Total, r.Results.Count());
            }),
            ["adhocBeneficiaries"] = async ct => await Handle("adhocBeneficiaries", ct, async () =>
            {
                var r = await _adhocBeneficiariesReport.GetAdhocBeneficiariesReportAsync(new AdhocBeneficiariesReportRequest(true) { ProfitYear = wallClockYear }, ct);
                return (r.Response.Total, r.Response.Results.Count());
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
