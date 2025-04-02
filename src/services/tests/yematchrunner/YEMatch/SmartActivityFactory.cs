using System.Net.Http.Headers;
using System.Text;
using YEMatch.YEMatch;
using HttpMethod = System.Net.Http.HttpMethod;

namespace YEMatch;

public static class SmartActivityFactory
{
    private static readonly short profitYear = 2024;

    public static List<Activity> CreateActivities(string dataDirectory)
    {
        var httpClient = new HttpClient { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");
        var client = new ApiClient(httpClient);

        return
        [
            new SmartActivity(A0_Initialize_Database_with_Obfuscated_data, client, "A0", "Initialize Database with Obfuscated data"),
            new SmartActivity(A1_Profit_Sharing_Clean_up_Reports, client, "A1", "Profit Sharing Clean Up Reports"),
            new SmartActivity(A2_Military_and_Rehire, client, "A2", "Military and Rehire"),
            new SmartActivity(A3_Prof_Termination, client, "A3", "Prof Termination"),
            new SmartActivity(A4_Prof_Share_Loan_Balance_QPAY129, client, "A4", "Prof Share Loan Balance (Distributions and Forfeitures QPAY129)"),
            new SmartActivity(A5_Extract_Excutive_Hours_and_Dollars, client, "A5", "Extract Executive Hours and Dollars"),
            new SmartActivity(A6_Clear_Executive_Hours_and_Dollars, client, "A6", "Clear Executive Hours and Dollars"),
            new SmartActivity(A7_Ready_Screen_008_09, client, "A7", "Ready Screen 008-09 (Enter exec hours)"),
            new SmartActivity(A8_PROFIT_SHARE_REPORT_PAY426, client, "A8", "Profit Share Report (duplicated in frozen section) (pay426)"),
            new SmartActivity(A9_YE_ORACLE_HCM_Payroll_Processing, client, "A9", "YE Oracle HCM Payroll Processing"),
            new SmartActivity(A10_Load_Orcle_PAYRPROFIT, client, "A10", "Load Oracle PAYPROFT (weekly job)"),
            new SmartActivity(A11_Profit_Sharing_YTD_Wages_Extract, client, "A11", "Profit sharing YTD Wages Extract"),
            new SmartActivity(A12_PROF_LOAD_YREND_DEMO_PROFITSHARE, client, "A12", "PROF LOAD YREND DEMO PROFSHARE"),
            new SmartActivity(A13A_PAYPROFIT_SHIFT, client, "A13A", "PAYPROFIT SHIFT (shift three columns)"),
            new SmartActivity(A13B_PAYPROFIT_SHIFT, client, "A13B", "PAYPROFIT SHIFT (shift three columns)"),
            new SmartActivity(A14_ZERO_PY_PD_PAYPROFIT, client, "A14", "ZERO-PY-PD-PAYPROFIT (zero three columns)"),
            new SmartActivity(A15_Profit_sharing_YTD_Wages_Extract, client, "A15", "Profit sharing YTD Wages Extract"),
            new SmartActivity(A16_READY_screen_008_09, client, "A16", "READY screen 008-09"),
            new SmartActivity(A17_Profit_Share_Report_edit_run, client, "A17", "Profit Share Report (Edit Run)"),
            new SmartActivity(A18_Profit_Share_Report_final_run, client, "A18", "Profit Share Report (Final Run)"),
            new SmartActivity(A19_Get_Eligible_Employees, client, "A19", "Get Eligible Employees"),
            new SmartActivity(A20_Profit_Forfeit_PAY443, client, "A20", "Profit Share Forfeit (pay443)"),
            new SmartActivity(A21_Profit_Share_Update_PAY444, client, "A21", "Profit Share Update (pay444)"),
            new SmartActivity(A22_Profit_Share_Edit_PAY477, client, "A22", "Profit Share Edit (pay447)"),
            new SmartActivity(A23_Profit_Master_Update, client, "A23", "Profit Master Update"),
            new SmartActivity(A24_PROF_PAYMASTER_UPD, client, "A24", "PROF PAYMASTER UPD"),
            new SmartActivity(A25_Prof_Share_Report_By_Age, client, "A25", "Prof Share Report By Age (prof130 prof130b prof130v prof130y)"),
            new SmartActivity(A26_Prof_Share_Gross_Rpt_QPAY501, client, "A26", "Prof Share Gross Rpt (qpay501)"),
            new SmartActivity(A27_Prof_Share_by_Store, client, "A27", "Prof Share by Store (qpay066-undr21 qpay066ta-undr21 qpay066ta \"newlabels report\" labels labelsnew)"),
            new SmartActivity(A28_Print_Profit_Certs, client, "A28", "Print Profit Certs (paycert)"),
            new SmartActivity(A29_Save_Prof_Paymstr, client, "A29", "Save Prof Paymstr final job to backup tables to tape")
        ];
    }

    private static async Task<Outcome> A0_Initialize_Database_with_Obfuscated_data(ApiClient apiClient, string aname, string name)
    {
        // Quick authentication sanity check
        var r = await apiClient.DemoulasCommonApiEndpointsAppVersionInfoEndpointAsync(null);
        // Might be nice to also include the database version. What database is used.  Wall clock time.
        Console.WriteLine($"{SmartActivity.smartPrefix}Connected to SMART build:" + r.BuildNumber + " git-hash:" + r.ShortGitHash);

        // Consider using CLI tool for reset the smart schema to stock 

        var res = ScriptRunner.Run("import-bh"); // Good enough and fast
        if (res != 0)
        {
            return new Outcome(aname, name, "", OutcomeStatus.Error, "Problem setting up database\n", null, true);
        }

        return new Outcome(aname, name, "", OutcomeStatus.Ok, "Database setup complete.\n", null, true);
    }

    private static async Task<Outcome> A1_Profit_Sharing_Clean_up_Reports(ApiClient apiClient, string aname, string name)
    {
        var sb = new StringBuilder();

        var r6 = await apiClient.ReportsYearEndCleanupNegativeEtvaForSsNsOnPayProfitEndPointAsync(profitYear, 0, int.MaxValue, null);
        sb.Append($"Negative Etva for Ssns  - records loaded: {r6.Response.Results.Count}\n");

        var r4 =
            await apiClient.ReportsYearEndCleanupGetDuplicateSsNsEndpointAsync(profitYear, int.MaxValue, null);
        sb.Append($"Duplicate Ssns  - records loaded: {r4.Response.Results.Count}\n");

        var r = await apiClient
            .ReportsYearEndCleanupDemographicBadgesNotInPayProfitEndpointAsync(0, int.MaxValue, null);
        sb.Append($"Badges Not In PayProfit - records loaded: {r.Response.Results.Count}\n");

        var r3 = await apiClient
            .ReportsYearEndCleanupDuplicateNamesAndBirthdaysEndpointAsync(profitYear, 0, int.MaxValue, null);
        sb.Append($"Duplicate Names And Birthdays - records loaded: {r3.Response.Results.Count}\n");

        var r5 = await apiClient
            .ReportsYearEndCleanupNamesMissingCommasEndpointAsync(0, int.MaxValue, null);
        sb.Append($"Missing Commas - records loaded: {r5.Response.Results.Count}\n");

        return new Outcome(aname, name, "", OutcomeStatus.Ok, sb.ToString(), null, true);
    }

    private static async Task<Outcome> A2_Military_and_Rehire(ApiClient apiClient, string aname, string name)
    {
        var sb = new StringBuilder();

        var result = await apiClient
            .ReportsYearEndMilitaryEmployeesOnMilitaryLeaveEndpointAsync(0, int.MaxValue, null);
        sb.Append($"Employees On Military Leave - records loaded: {result.Response.Results.Count}\n");

        var r2 = await apiClient
            .ReportsYearEndMilitaryMilitaryAndRehireForfeituresEndpointAsync(
                profitYear.ToString(), profitYear, 0, int.MaxValue, null);
        sb.Append($"Military And Rehire Forfeitures - records loaded: {r2.Response.Results.Count}\n");

        var r3 = await apiClient
            .ReportsYearEndMilitaryMilitaryAndRehireProfitSummaryEndpointAsync(
                profitYear.ToString(), profitYear, 0, int.MaxValue, null);
        sb.Append($"Military And Rehire Profit Summary - records loaded: {r3.Response.Results.Count}\n");

        return Ok(aname, name, sb);
    }

    private static Outcome Ok(string aname, string name, StringBuilder sb)
    {
        return new Outcome(aname, name, "", OutcomeStatus.Ok, sb.ToString(), null, true);
    }

    private static Outcome Ok(string aname, string name, string str)
    {
        return new Outcome(aname, name, "", OutcomeStatus.Ok, str, null, true);
    }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async Task<Outcome> A3_Prof_Termination(ApiClient apiClient, string aname, string name)
    {
        var result = await apiClient
            .ReportsYearEndTerminatedEmployeeAndBeneficiaryTerminatedEmployeeAndBeneficiaryDataEndpointAsync(
                profitYear, 0, int.MaxValue, null);

        return Ok(aname, name, $"Records Loaded {result.Response.Results.Count}");
    }

    private static async Task<Outcome> A4_Prof_Share_Loan_Balance_QPAY129(ApiClient apiClient, string aname, string name)
    {
        var sb = new StringBuilder();

        var r2 = await apiClient
            .ReportsYearEndCleanupDistributionsAndForfeitureEndpointAsync(1, 12,
                true, profitYear, 0, int.MaxValue, null);
        sb.Append($"Records Loaded {r2.Response.Results.Count}\n");

        return Ok(aname, name, sb);
    }

    private static async Task<Outcome> A5_Extract_Excutive_Hours_and_Dollars(ApiClient apiClient, string aname, string name)
    {
        var r2 = await apiClient
            .ReportsYearEndExecutiveHoursAndDollarsExecutiveHoursAndDollarsEndpointAsync(null, null, "", true, false, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r2.Response.Results.Count}\n");
    }

    private static async Task<Outcome> A6_Clear_Executive_Hours_and_Dollars(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static Outcome NOP(string aname, string name)
    {
        return new Outcome(aname, name, "", OutcomeStatus.NoOperation, "", null, true);
    }

    private static async Task<Outcome> A7_Ready_Screen_008_09(ApiClient apiClient, string aname, string name)
    {
        return TBD(name, name, "Should update some executives hours and dollars");
    }

    private static Outcome TBD(string aname, string name, string msg = "")
    {
        return new Outcome(aname, name, "", OutcomeStatus.ToBeDone, msg, null, true);
    }

    // ort(bool isYearEnd, int? minimumAgeInclusive, int? maximumAgeInclusive, decimal? minimumHoursInclusive, decimal? maximumHoursInclusive, bool includeActiveEmployees,
    // bool includeInactiveEmployees, bool includeEmployeesTerminatedThisYear, bool includeTerminatedEmployees, bool includeBeneficiaries, bool includeEmployeesWithPriorProfitSharingAmounts,
    // bool includeEmployeesWithNoPriorProfitSharingAmounts, int profitYear, int? skip, int? take, Impersonation8? impersonation)

    private static async Task<Outcome> A8_PROFIT_SHARE_REPORT_PAY426(ApiClient apiClient, string aname, string name)
    {
#if false
        // Maybe broken?   Need to follow, https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/70582342/Clean+Up+Part#A8%3A-Profit-Share-Report

        var r = await apiClient.ReportsYearEndProfitShareReportYearEndProfitSharingReport(false /*false=Use Demographic data, not frozen*/, 0, 200,
            0, Decimal.MaxValue, true, true, true, true, true,
            true, true, profitYear, 0, 10, null);

        return new Outcome(aname, name, "OK", $"Records Loaded = {r.Response.Results.Count}");

#else
        return TBD(aname, name, "Summary report not yet complete.");

#endif
    }

    private static async Task<Outcome> A9_YE_ORACLE_HCM_Payroll_Processing(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static async Task<Outcome> A10_Load_Orcle_PAYRPROFIT(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static async Task<Outcome> A11_Profit_Sharing_YTD_Wages_Extract(ApiClient apiClient, string aname, string name)
    {
        var r = await apiClient.ReportsYearEndWagesCurrentYearWagesEndpointAsync(profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Record Count: {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A12_PROF_LOAD_YREND_DEMO_PROFITSHARE(ApiClient apiClient, string aname, string name)
    {
#if false
        var req = new SetFrozenStateRequest();
        req.AsOfDateTime = DateTimeOffset.Now;
        var r = await apiClient.DemographicsFreezeDemographicsEndpointAsync(null, req);
        return new Outcome(aname, name, "OK", $"Frozen at {r.AsOfDateTime} by {r.FrozenBy} isActive:{r.IsActive}", "");
#endif
        return TBD(aname, name);
    }

    private static async Task<Outcome> A13A_PAYPROFIT_SHIFT(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static async Task<Outcome> A13B_PAYPROFIT_SHIFT(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static async Task<Outcome> A14_ZERO_PY_PD_PAYPROFIT(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static Task<Outcome> A15_Profit_sharing_YTD_Wages_Extract(ApiClient apiClient, string aname, string name)
    {
        return A11_Profit_Sharing_YTD_Wages_Extract(apiClient, aname, name);
    }

    private static async Task<Outcome> A16_READY_screen_008_09(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name, "Enter executive hours and dollars (Second chance)");
    }

    private static async Task<Outcome> A17_Profit_Share_Report_edit_run(ApiClient apiClient, string aname, string name)
    {
        var sb = new StringBuilder();
        foreach (var kvp in Pay426NCriteria._reportCriteria.OrderBy(kvp => kvp.Key))
        {
            var criteria = kvp.Value;
            var response = await apiClient.ReportsYearEndProfitShareReportYearEndProfitSharingReportEndpointAsync(
                criteria.IsYearEnd,
                criteria.MinimumAgeInclusive,
                criteria.MaximumAgeInclusive,
                criteria.MinimumHoursInclusive,
                criteria.MaximumHoursInclusive,
                criteria.IncludeActiveEmployees,
                criteria.IncludeInactiveEmployees,
                criteria.IncludeEmployeesTerminatedThisYear,
                criteria.IncludeTerminatedEmployees,
                criteria.IncludeBeneficiaries,
                criteria.IncludeEmployeesWithPriorProfitSharingAmounts,
                criteria.IncludeEmployeesWithNoPriorProfitSharingAmounts,
                true,
                true,
                criteria.ProfitYear,
                0,
                int.MaxValue,
                null
            );
            if (sb.Length != 0)
            {
                sb.Append(", ");
            }

            sb.Append(kvp.Key[^2..] + "=" + response.Response.Results.Count);
        }

        return Ok(aname, name, sb.ToString());
    }

    private static async Task<Outcome> A18_Profit_Share_Report_final_run(ApiClient apiClient, string aname, string name)
    {
        // Translated from the swagger curl example

        var httpClient = new HttpClient { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");
#pragma warning disable S1075
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5298/api/yearend/final")
#pragma warning restore S1075
        {
            Content = new StringContent("{ \"ProfitYear\" : " + profitYear + "}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);

        return Ok(aname, name, "");
    }

    private static async Task<Outcome> A19_Get_Eligible_Employees(ApiClient apiClient, string aname, string name)
    {
        var r = await apiClient.ReportsYearEndEligibilityGetEligibleEmployeesEndpointAsync(profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A20_Profit_Forfeit_PAY443(ApiClient apiClient, string aname, string name)
    {
        var r = await apiClient.ReportsYearEndFrozenForfeituresAndPointsForYearEndpointAsync(true, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A21_Profit_Share_Update_PAY444(ApiClient apiClient, string aname, string name)
    {
#if true
        var r = await apiClient.ReportsYearEndProfitShareUpdateProfitShareUpdateEndpointAsync(15, 4, 5, 0,
            30_000, 0, 0, 0, 0, 0, 0, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
#else
        return ToBeDone(aname, name,   "OFF - Too sluggish - n+1 query");
#endif
    }

    private static async Task<Outcome> A22_Profit_Share_Edit_PAY477(ApiClient apiClient, string aname, string name)
    {
#if true
        var r = await apiClient.ReportsYearEndProfitShareEditEndpointAsync(15, 4, 5, 0,
            30_000, 0, 0, 0, 0, 0, 0, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
#else
        return ToBeDone(aname, name,  "OFF - Too sluggish - n+1 query");
#endif
    }

    private static async Task<Outcome> A23_Profit_Master_Update(ApiClient apiClient, string aname, string name)
    {
        // Awards profit sharing.    
        var r = await apiClient.ReportsYearEndProfitMasterProfitMasterUpdateEndpointAsync(15, 4, 5, 0,
            30_000, 0, 0, 0, 0, 0, 0, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"BeneficiariesEffected: {r.BeneficiariesEffected}, EmployeesEffected: {r.EmployeesEffected}");
    }

    private static async Task<Outcome> A24_PROF_PAYMASTER_UPD(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name);
    }

    private static async Task<Outcome> A25_Prof_Share_Report_By_Age(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name);
    }

    private static async Task<Outcome> A26_Prof_Share_Gross_Rpt_QPAY501(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name);
    }

    private static async Task<Outcome> A27_Prof_Share_by_Store(ApiClient apiClient, string aname, string name)
    {
        var r = await apiClient.ReportsYearEndBreakdownEndpointAsync(false, null, profitYear, 0, int.MaxValue, null);
        return Ok(aname, name, $"records returned = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A28_Print_Profit_Certs(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name);
    }

    private static async Task<Outcome> A29_Save_Prof_Paymstr(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }
}
