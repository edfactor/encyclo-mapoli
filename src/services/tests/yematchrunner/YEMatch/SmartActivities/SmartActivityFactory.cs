using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YEMatch.YEMatch.Activities;
using HttpMethod = System.Net.Http.HttpMethod;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable S125
#pragma warning disable S1075
#pragma warning disable CA2211
#pragma warning disable S2223
#pragma warning disable S1104

namespace YEMatch.YEMatch.SmartActivities;

public static class SmartActivityFactory
{
    private static readonly short _profitYear = 2024;

    public static ApiClient? Client;

    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Executive-Administrator");
        Client = new ApiClient(httpClient);

        return
        [
            new SmartActivity(A0_Initialize_Database_with_Obfuscated_data, Client, "A0", "Initialize Database with Obfuscated data"),
            new SmartActivity(A1_Profit_Sharing_Clean_up_Reports, Client, "A1", "Profit Sharing Clean Up Reports"),
            new SmartActivity(A2_Military_and_Rehire, Client, "A2", "Military and Rehire"),
            new SmartActivity(A3_Prof_Termination, Client, "A3", "Prof Termination"),
            new SmartActivity(A4_Prof_Share_Loan_Balance_QPAY129, Client, "A4", "Prof Share Loan Balance (Distributions and Forfeitures QPAY129)"),
            new SmartActivity(A5_Extract_Excutive_Hours_and_Dollars, Client, "A5", "Extract Executive Hours and Dollars"),
            new SmartActivity(A6_Clear_Executive_Hours_and_Dollars, Client, "A6", "Clear Executive Hours and Dollars"),
            new SmartActivity(A7_Ready_Screen_008_09, Client, "A7", "Ready Screen 008-09 (Enter exec hours)"),
            new SmartActivity(A8_PROFIT_SHARE_REPORT_PAY426, Client, "A8", "Profit Share Report (duplicated in frozen section) (pay426)"),
            new SmartActivity(A9_YE_ORACLE_HCM_Payroll_Processing, Client, "A9", "YE Oracle HCM Payroll Processing"),
            new SmartActivity(A10_Load_Orcle_PAYRPROFIT, Client, "A10", "Load Oracle PAYPROFT (weekly job)"),
            new SmartActivity(A11_Profit_Sharing_YTD_Wages_Extract, Client, "A11", "Profit sharing YTD Wages Extract"),
            new SmartActivity(A12_PROF_LOAD_YREND_DEMO_PROFITSHARE, Client, "A12", "PROF LOAD YREND DEMO PROFSHARE"),
            new SmartActivity(A13A_PAYPROFIT_SHIFT, Client, "A13A", "PAYPROFIT SHIFT (shift three columns)"),
            new SmartActivity(A13B_PAYPROFIT_SHIFT, Client, "A13B", "PAYPROFIT SHIFT (shift three columns)"),
            new SmartActivity(A14_ZERO_PY_PD_PAYPROFIT, Client, "A14", "ZERO-PY-PD-PAYPROFIT (zero three columns)"),
            new SmartActivity(A15_Profit_sharing_YTD_Wages_Extract, Client, "A15", "Profit sharing YTD Wages Extract"),
            new SmartActivity(A16_READY_screen_008_09, Client, "A16", "READY screen 008-09"),
            new SmartActivity(A17_Profit_Share_Report_edit_run, Client, "A17", "Profit Share Report (Edit Run)"),
            new SmartActivity(A18_Profit_Share_Report_final_run, Client, "A18", "Profit Share Report (Final Run)"),
            new SmartActivity(A19_Get_Eligible_Employees, Client, "A19", "Get Eligible Employees"),
            new SmartActivity(A20_Profit_Forfeit_PAY443, Client, "A20", "Profit Share Forfeit (pay443)"),
            new SmartActivity(A21_Profit_Share_Update_PAY444, Client, "A21", "Profit Share Update (pay444)"),
            new SmartActivity(A22_Profit_Share_Edit_PAY477, Client, "A22", "Profit Share Edit (pay447)"),
            new SmartActivity(A23_Profit_Master_Update, Client, "A23", "Profit Master Update"),
            new SmartActivity(A24_PROF_PAYMASTER_UPD, Client, "A24", "PROF PAYMASTER UPD"),
            new SmartActivity(A24B_PROF_PAYMASTER_UPD, Client, "A24B", "PROF PAYMASTER UPD part two"), // Do an update to BE.
            new SmartActivity(A25_Prof_Share_Report_By_Age, Client, "A25", "Prof Share Report By Age (prof130 prof130b prof130v prof130y)"),
            new SmartActivity(A26_Prof_Share_Gross_Rpt_QPAY501, Client, "A26", "Prof Share Gross Rpt (qpay501)"),
            new SmartActivity(A27_Prof_Share_by_Store, Client, "A27", "Prof Share by Store (qpay066-undr21 qpay066ta-undr21 qpay066ta \"newlabels report\" labels labelsnew)"),
            new SmartActivity(A28_Print_Profit_Certs, Client, "A28", "Print Profit Certs (paycert)"),
            new SmartActivity(A29_Save_Prof_Paymstr, Client, "A29", "Save Prof Paymstr final job to backup tables to tape")
        ];
    }

    private static async Task<Outcome> A0_Initialize_Database_with_Obfuscated_data(ApiClient apiClient, string aname, string name)
    {
        // We now use the CLI ImportReadyToSmartDB to do the import, so it handles the 2023 rebuild 
        throw new NotImplementedException();

#if false        
        // Quick authentication sanity check
        AppVersionInfo? r = await apiClient.DemoulasCommonApiEndpointsAppVersionInfoEndpointAsync(null);
        // Might be nice to also include the database version. What database is used.  Wall clock time.
        Console.WriteLine(" Connected to SMART build:" + r.BuildNumber + " git-hash:" + r.ShortGitHash);

        // Consider using CLI tool for reset the smart schema to stock 
        int res = ScriptRunner.Run(false, "import-bh"); // Good enough and fast
        if (res != 0)
        {
            return new Outcome(aname, name, "", OutcomeStatus.Error, "Problem setting up database\n", null, true);
        }

        return new Outcome(aname, name, "", OutcomeStatus.Ok, "Database setup complete.\n", null, true);
#endif
    }

    private static async Task<Outcome> A1_Profit_Sharing_Clean_up_Reports(ApiClient apiClient, string aname, string name)
    {
        StringBuilder sb = new();

        ReportResponseBaseOfNegativeEtvaForSsNsOnPayProfitResponse? r6 =
            await apiClient.ReportsYearEndCleanupNegativeEtvaForSsNsOnPayProfitEndPointAsync(_profitYear, null, null, 0, int.MaxValue, null);
        sb.Append($"Negative Etva for Ssns  - records loaded: {r6.Response.Results.Count}\n");

        ReportResponseBaseOfPayrollDuplicateSsnResponseDto? r4 =
            await apiClient.ReportsYearEndCleanupGetDuplicateSsNsEndpointAsync(null, null, _profitYear, int.MaxValue, null);
        sb.Append($"Duplicate Ssns  - records loaded: {r4.Response.Results.Count}\n");

        ReportResponseBaseOfDemographicBadgesNotInPayProfitResponse? r = await apiClient
            .ReportsYearEndCleanupDemographicBadgesNotInPayProfitEndpointAsync(null, null, 0, int.MaxValue, null);
        sb.Append($"Badges Not In PayProfit - records loaded: {r.Response.Results.Count}\n");

        ReportResponseBaseOfDuplicateNamesAndBirthdaysResponse? r3 = await apiClient
            .ReportsYearEndCleanupDuplicateNamesAndBirthdaysEndpointAsync(_profitYear, null, null, 0, int.MaxValue, null);
        sb.Append($"Duplicate Names And Birthdays - records loaded: {r3.Response.Results.Count}\n");

        return new Outcome(aname, name, "", OutcomeStatus.Ok, sb.ToString(), null, true);
    }

    private static async Task<Outcome> A2_Military_and_Rehire(ApiClient apiClient, string aname, string name)
    {
        /*
               StringBuilder sb = new();

                ReportResponseBaseOfEmployeesOnMilitaryLeaveResponse? result = await apiClient
                    .ReportsYearEndMilitaryEmployeesOnMilitaryLeaveEndpointAsync(null, null, 0, int.MaxValue, null);
                sb.Append($"Employees On Military Leave - records loaded: {result.Response.Results.Count}\n");

                StartAndEndDateRequest sedr = new();
                //sedr.ProfitYear = _profitYear;
                sedr.BeginningDate = DateOnly.FromDateTime(DateTime.Parse("2024-01-06", CultureInfo.InvariantCulture));
                sedr.EndingDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-04", CultureInfo.InvariantCulture));

                ReportResponseBaseOfRehireForfeituresResponse? r2 = await apiClient.ReportsYearEndMilitaryRehireForfeituresEndpointAsync(null);
                sb.Append($"Military And Rehire Forfeitures - records loaded: {r2.Response.Results.Count}\n");
        */
        return TBD(name, name, "Changed to use POST, invokation requires update");
    }

    private static Outcome Ok(string aname, string name, StringBuilder sb)
    {
        return new Outcome(aname, name, "", OutcomeStatus.Ok, sb.ToString(), null, true);
    }

    private static Outcome Ok(string aname, string name, string str)
    {
        return new Outcome(aname, name, "", OutcomeStatus.Ok, str, null, true);
    }

    // NSwagger does not handle POST requests well.  Its definition of the Request is missing the "profitYear" share parameter
    public class StartAndEndDateRequestLocal : StartAndEndDateRequest
    {
        public DateOnly BeginningDate { get; set; }
        public DateOnly EndingDate { get; set; }
    }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async Task<Outcome> A3_Prof_Termination(ApiClient apiClient, string aname, string name)
    {
        StartAndEndDateRequestLocal req = new() { BeginningDate = DateOnly.FromDateTime(DateTime.Now), EndingDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)) };
        TerminatedEmployeeAndBeneficiaryResponse? result = await apiClient.ReportsYearEndTerminatedEmployeesTerminatedEmployeesEndPointAsync(null, req);

        return Ok(aname, name, $"Records Loaded {result.Response.Results.Count}");
    }

    private static async Task<Outcome> A4_Prof_Share_Loan_Balance_QPAY129(ApiClient apiClient, string aname, string name)
    {
        StringBuilder sb = new();

        ReportResponseBaseOfDistributionsAndForfeitureResponse? r2 = await apiClient
            .ReportsYearEndCleanupDistributionsAndForfeitureEndpointAsync(null, null, _profitYear, null, null, null, null, null, CancellationToken.None);
        sb.Append($"Records Loaded {r2.Response.Results.Count}\n");

        return Ok(aname, name, sb);
    }

    private static async Task<Outcome> A5_Extract_Excutive_Hours_and_Dollars(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfExecutiveHoursAndDollarsResponse? r2 = await apiClient
            .ReportsYearEndExecutiveHoursAndDollarsExecutiveHoursAndDollarsEndpointAsync(null, null, "", true, false, _profitYear, null, null, 0, int.MaxValue, null);
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
        ReportResponseBaseOfWagesCurrentYearResponse? r = await apiClient.ReportsYearEndWagesCurrentYearWagesEndpointAsync(_profitYear, null, null, 0, int.MaxValue, null);
        return Ok(aname, name, $"Record Count: {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A12_PROF_LOAD_YREND_DEMO_PROFITSHARE(ApiClient apiClient, string aname, string name)
    {
        // Swagger seems to not document POST calls correctly.  The entities dont have the required fields.
        // So the generated code also doesnt fit the bill.
        // The work around is to use the "curL" suggested by swagger, which does have the correct arguments in the post. 
        //     example from swagger
        //     curl -X 'POST' \
        //     'https://ps.qa.demoulas.net:8443/api/itdevops/freeze' \
        //     -H 'accept: application/json' \
        //     -H 'Authorization: ...\
        //     -H 'Content-Type: application/json' \
        //     -d '{
        //     "asOfDateTime": "2025-05-06T00:00:00-04:00",
        //     "profitYear": 2025
        // }'
        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "IT-DevOps");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/itdevops/freeze")
        {
            Content = new StringContent("{ \"ProfitYear\" : " + _profitYear + ", \"asOfDateTime\": \"2025-01-09T00:00:00-04:00\"}"
                , Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);

        return Ok(aname, name, "");
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
        StringBuilder sb = new();
        foreach (KeyValuePair<string, Pay426NCriteria> kvp in Pay426NCriteria._reportCriteria.OrderBy(kvp => kvp.Key))
        {
            string key = kvp.Key;
            Console.WriteLine("key " + key);
            if (key == "PAY426N-10")
            {
                Console.WriteLine("WARNING SKIPPING PAY426N-10 because it is broken.");
                continue;
            }

            Pay426NCriteria criteria = kvp.Value;
            string postBody = JsonSerializer.Serialize(criteria);

            HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
            TestToken.CreateAndAssignTokenForClient(httpClient, "Executive-Administrator");
            HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/yearend-profit-sharing-report")
            {
                Content = new StringContent(postBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            using HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            ///sb.Append(kvp.Key[^2..] + "=" + response.Response.Results.Count);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Outcome(aname, name, "", OutcomeStatus.Error, "Problem with endpoint\n", null, true);
            }
        }

        return Ok(aname, name, sb.ToString());
    }

    private static async Task<Outcome> A18_Profit_Share_Report_final_run(ApiClient apiClient, string aname, string name)
    {
        // Translated from the swagger curl example

        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/final")
        {
            Content = new StringContent("{ \"ProfitYear\" : " + _profitYear + "}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);

        return Ok(aname, name, "");
    }

    private static async Task<Outcome> A19_Get_Eligible_Employees(ApiClient apiClient, string aname, string name)
    {
        GetEligibleEmployeesResponse? r = await apiClient.ReportsYearEndEligibilityGetEligibleEmployeesEndpointAsync(_profitYear, null, null, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A20_Profit_Forfeit_PAY443(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfForfeituresAndPointsForYearResponse? r =
            await apiClient.ReportsYearEndFrozenForfeituresAndPointsForYearEndpointAsync(true, _profitYear, null, null, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A21_Profit_Share_Update_PAY444(ApiClient apiClient, string aname, string name)
    {
        ProfitShareUpdateResponse? r = await apiClient.ReportsYearEndProfitShareUpdateProfitShareUpdateEndpointAsync(15, 4, 5, 0, 76_500, 0, 0, 0, 0, 0, 0, _profitYear, null, null,
            0, int.MaxValue,
            null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A22_Profit_Share_Edit_PAY477(ApiClient apiClient, string aname, string name)
    {
        ProfitShareEditResponse? r = await apiClient.ReportsYearEndProfitShareEditEndpointAsync(15, 4, 5, 0,
            76_500, 0, 0, 0, 0, 0, 0, _profitYear, null, null, 0, int.MaxValue, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    public sealed class ProfitShareUpdateRequestLocal : ProfitShareUpdateRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

        public string? SortBy { get; init; }
        public bool? IsSortDescending { get; init; }
        public short ProfitYear { get; set; }
        public decimal ContributionPercent { get; set; }
        public decimal IncomingForfeitPercent { get; set; }
        public decimal EarningsPercent { get; set; }
        public decimal SecondaryEarningsPercent { get; set; }
        public long MaxAllowedContributions { get; set; }
        public long BadgeToAdjust { get; set; }
        public long BadgeToAdjust2 { get; set; }
        public decimal AdjustContributionAmount { get; set; }
        public decimal AdjustEarningsAmount { get; set; }
        public decimal AdjustIncomingForfeitAmount { get; set; }
        public decimal AdjustEarningsSecondaryAmount { get; set; }
    }


    private static async Task<Outcome> A23_Profit_Master_Update(ApiClient apiClient, string aname, string name)
    {
        // Awards profit sharing.    

        ProfitShareUpdateRequestLocal req = new();
        req.ProfitYear = _profitYear;
        req.ContributionPercent = 15m;
        req.IncomingForfeitPercent = 4m;
        req.EarningsPercent = 5m;
        req.SecondaryEarningsPercent = 0m;
        req.MaxAllowedContributions = 76_500;
        req.Take = int.MaxValue;
        string postBody = JsonSerializer.Serialize(req);

        // ProfitMasterUpdateResponse? r = await apiClient.ReportsYearEndProfitMasterProfitMasterUpdateEndpointAsync(null, req);

        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "System-Administrator");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/profit-master-update")
        {
            Content = new StringContent(postBody, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        int beneficiaries = root.GetProperty("beneficiariesEffected").GetInt32();
        int employees = root.GetProperty("employeesEffected").GetInt32();
        int etvas = root.GetProperty("etvasEffected").GetInt32();

        return Ok(aname, name, $"beneficiariesEffected: {beneficiaries}, employeesEffected: {employees}, etvasEffected: {etvas}");
    }

    private static async Task<Outcome> A24_PROF_PAYMASTER_UPD(ApiClient apiClient, string aname, string name)
    {
        // curl -X 'POST' \
        // 'https://ps.qa.demoulas.net:8443/api/yearend/update-enrollment' \
        // -H 'accept: */*' \
        // -H 'Impersonation: Finance-Manager' \
        // -H 'Authorization: Bearer eyJraWQiOiJkZUZKc3o3OTVsNU1wQ2RUdlVVY3JaOEFUbFdXM0t2NFFjZ0dLa0NPZU5RIiwiYWxnIjoiUlMyNTYifQ.eyJ2ZXIiOjEsImp0aSI6IkFULk5MYkwyZEtUQld4OEZrV1FFckFrZ3hlSVFBLVJqUWdmQjluQzN2T1I1U2siLCJpc3MiOiJodHRwczovL21hcmtldGJhc2tldC5va3RhLmNvbS9vYXV0aDIvYXVzMTEzZWhjNWtrcVRlWEIxdDgiLCJhdWQiOiJhcGk6Ly9zbWFydC1wcyIsImlhdCI6MTc1MDc4NzcxNSwiZXhwIjoxNzUwNzkxMzE1LCJjaWQiOiIwb2ExMTNlYWpqNEpUMWlrSjF0OCIsInVpZCI6IjAwdXd3dG1vcjhTdDNRbFlxMXQ3Iiwic2NwIjpbIm9wZW5pZCJdLCJhdXRoX3RpbWUiOjE3NTA3NjcxODUsInN1YiI6ImJoZXJybWFubkBNYWlub2ZmaWNlLkRlbW91bGFzLkNvcnAiLCJncm91cHMiOlsiU01BUlQtUFMtUUEtSW1wZXJzb25hdGlvbiJdfQ.m8ITgSbziHEvh6NBSnVo0pnrUIjsk4ZDWXoiP86Jdkgcz4BAAuYBI3euaXoRocWGdEfUEVmBmqjldkeAzXeoIKsp28L04n06cqzXsP6_a_y_8X9TptRxDz3dBROu5r_pGbsKXmGP8S4UaJYwiWTWc-8FDf0aM3GML0vMEQzoZE5nS8_IqEUpiyEu1anmqMMJ1Oc6t0kSbxBVSK7qkoMKsoEtB64OX2_qDrKXAzNlQg-6_w_m9U1Lgvq-iriXcU1KxMfL-HSjIa0e4LSRJcI7WrCRatA-bdg3_nRik6KW3dZNMj1vnLRqdQkJq6-O5wrIDKJ3Ek2EqJPMYMllv7oycQ' \
        // -H 'Content-Type: application/json' \
        // -d '{  "profitYear": 2024 }'

        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/update-enrollment")
        {
            Content = new StringContent("{ \"profitYear\": 2024}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return Ok(aname, name, "Updated enrollment.");
    }

    private static async Task<Outcome> A24B_PROF_PAYMASTER_UPD(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
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
        ReportResponseBaseOfMemberYearSummaryDto? r = await apiClient.ReportsYearEndBreakdownEndpointAsync(false, 1, null, null, 2025, "", null, null, null, null);
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
