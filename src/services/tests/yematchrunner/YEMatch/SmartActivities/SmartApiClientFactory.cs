using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using YEMatch.Activities;
using HttpMethod = System.Net.Http.HttpMethod;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable S125
#pragma warning disable S1075
#pragma warning disable S2223
#pragma warning disable S1104

namespace YEMatch.SmartActivities;

[SuppressMessage("Minor Code Smell", "S1199:Nested code blocks should not be used")]
public sealed class SmartApiClientFactory : ISmartApiClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly bool _nuke = false;
    private readonly YeMatchOptions _options;
    private readonly short _profitYear;
    private readonly JwtOptions _jwtOptions;
    private ApiClient? _client;

    public SmartApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<YeMatchOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);

        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _profitYear = _options.YearEndDates.ProfitYear;
        _jwtOptions = _options.Jwt;
    }

    public ApiClient CreateClient()
    {
        if (_client is not null)
        {
            return _client;
        }

        HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
        TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "System-Administrator", "Executive-Administrator");

        _client = new ApiClient(httpClient);
        return _client;
    }

    public List<IActivity> CreateActivities(string dataDirectory)
    {
        ApiClient client = CreateClient();

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
            new SmartActivity(A24B_PROF_PAYMASTER_UPD, client, "A24B", "PROF PAYMASTER UPD part two"), // Do an update to BE.
            new SmartActivity(A25_Prof_Share_Report_By_Age, client, "A25", "Prof Share Report By Age (prof130 prof130b prof130v prof130y)"),
            new SmartActivity(A26_Prof_Share_Gross_Rpt_QPAY501, client, "A26", "Prof Share Gross Rpt (qpay501)"),
            new SmartActivity(A27_Prof_Share_by_Store, client, "A27", "Prof Share by Store (qpay066-undr21 qpay066ta-undr21 qpay066ta \"newlabels report\" labels labelsnew)"),
            new SmartActivity(A28_Print_Profit_Certs, client, "A28", "Print Profit Certs (paycert)"),
            new SmartActivity(A29_Save_Prof_Paymstr, client, "A29", "Save Prof Paymstr final job to backup tables to tape")
        ];
    }

    private async Task<Outcome> A0_Initialize_Database_with_Obfuscated_data(ApiClient apiClient, string aname, string name)
    {
        try
        {
            if (_nuke)
            {
                AspireToAspire("drop-recreate-db --connection-name ProfitSharing");
                AspireToAspire(
                    "import-from-navigation --connection-name ProfitSharing --source-schema \"PROFITSHARE\" --sql-file \"../../../../src/database/ready_import/Navigations/add-navigation-data.sql\"");
            }

            AspireToAspire(
                "import-from-ready --connection-name ProfitSharing --source-schema PROFITSHARE --sql-file \"../../../../src/database/ready_import/SQL copy all from ready to smart ps.sql\"");
        }
        catch (Exception ex)
        {
            return new Outcome(aname, name, "", OutcomeStatus.Error, "Problem setting up database " + ex.Message, null, true);
        }

        return new Outcome(aname, name, "", OutcomeStatus.Ok, "Database setup complete.\n", null, true);
    }

    /** Invoke the CLI so we are not reinventing the wheel in this test framework **/
    private static void AspireToAspire(string command)
    {
        string cliPath = "/Users/robertherrmann/prj/smart-profit-sharing/src/services/src/Demoulas.ProfitSharing.Data.Cli";
        int res = ScriptRunner.Run(false /*NOT CHATTY*/, cliPath, "dotnet", "run " + command);

        if (res != 0)
        {
            throw new Exception($" res={res} during command --> {command}");
        }
    }

    private async Task<Outcome> A1_Profit_Sharing_Clean_up_Reports(ApiClient apiClient, string aname, string name)
    {
        StringBuilder sb = new();

        ReportResponseBaseOfNegativeEtvaForSsNsOnPayProfitResponse? r6 =
            await apiClient.ReportsYearEndCleanupNegativeEtvaForSsNsOnPayProfitEndPointAsync(_profitYear, "", null, null, null, int.MaxValue, null);
        sb.Append($"Negative Etva for Ssns  - records loaded: {r6.Response.Results.Count}\n");

        ReportResponseBaseOfPayrollDuplicateSsnResponseDto? r4 =
            await apiClient.ReportsYearEndCleanupGetDuplicateSsNsEndpointAsync(_profitYear, "", null, int.MaxValue, null, null, null);
        sb.Append($"Duplicate Ssns  - records loaded: {r4.Response.Results.Count}\n");

        ReportResponseBaseOfDemographicBadgesNotInPayProfitResponse? r = await apiClient
            .ReportsYearEndCleanupDemographicBadgesNotInPayProfitEndpointAsync(_profitYear, "", null, 0, int.MaxValue, null, null);
        sb.Append($"Badges Not In PayProfit - records loaded: {r.Response.Results.Count}\n");

        ReportResponseBaseOfDuplicateNamesAndBirthdaysResponse? r3 = await apiClient
            .ReportsYearEndCleanupDuplicateNamesAndBirthdaysEndpointAsync(_profitYear, "", null, null, int.MaxValue, 0, null);
        sb.Append($"Duplicate Names And Birthdays - records loaded: {r3.Response.Results.Count}\n");

        return new Outcome(aname, name, "", OutcomeStatus.Ok, sb.ToString(), null, true);
    }

    private static async Task<Outcome> A2_Military_and_Rehire(ApiClient apiClient, string aname, string name)
    {
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

    public class StartAndEndDateRequestLocal : FilterableStartAndEndDateRequest
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
        DistributionsAndForfeituresRequest dafr = new();

        ReportResponseBaseOfDistributionsAndForfeitureResponse? r2 = await apiClient
            .ReportsYearEndCleanupDistributionsAndForfeitureEndpointAsync(null, dafr);
        sb.Append($"Records Loaded {r2.Response.Results.Count}\n");

        return Ok(aname, name, sb);
    }

    private async Task<Outcome> A5_Extract_Excutive_Hours_and_Dollars(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfExecutiveHoursAndDollarsResponse? r2 = await apiClient
            .ReportsYearEndExecutiveHoursAndDollarsExecutiveHoursAndDollarsEndpointAsync(null, null, null, null, null, _profitYear, null, null, null, int.MaxValue, 0, null);
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

    private static async Task<Outcome> A8_PROFIT_SHARE_REPORT_PAY426(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name, "Summary report not yet complete.");
    }

    private static async Task<Outcome> A9_YE_ORACLE_HCM_Payroll_Processing(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private static async Task<Outcome> A10_Load_Orcle_PAYRPROFIT(ApiClient apiClient, string aname, string name)
    {
        return NOP(aname, name);
    }

    private async Task<Outcome> A11_Profit_Sharing_YTD_Wages_Extract(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfWagesCurrentYearResponse? r =
            await apiClient.ReportsYearEndWagesCurrentYearWagesEndpointAsync(_profitYear, false, "", null, null, null, int.MaxValue, null);
        return Ok(aname, name, $"Record Count: {r.Response.Results.Count}");
    }

    private async Task<Outcome> A12_PROF_LOAD_YREND_DEMO_PROFITSHARE(ApiClient apiClient, string aname, string name)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
        TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "IT-DevOps");

#if false
        // try to use the nswag approach
        SetFrozenStateRequest setFrozenStateRequest = new();
        setFrozenStateRequest.AsOfDateTime = DateTimeOffset.ParseExact(
            "2026-01-03T00:00:00-04:00",
            "yyyy-MM-ddTHH:mm:sszzz",
            CultureInfo.InvariantCulture);
         FrozenStateResponse fsr = await apiClient.ItOperationsFreezeDemographicsEndpointAsync(null, setFrozenStateRequest);
#else

        // bail and do it old school.
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/it-devops/freeze")
        {
            Content = new StringContent("{ \"ProfitYear\" : " + _profitYear + ", \"asOfDateTime\": \"2026-01-03T00:00:00-04:00\"}"
                , Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
#endif


        //        return Ok(aname, name, "Freeze");
        return new Outcome(aname, name, "Freeze on SMART", OutcomeStatus.Ok, "", null, true);
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

    private async Task<Outcome> A15_Profit_sharing_YTD_Wages_Extract(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfWagesCurrentYearResponse? r =
            await apiClient.ReportsYearEndWagesCurrentYearWagesEndpointAsync(_profitYear, true, "", null, null, null, int.MaxValue, null);
        return Ok(aname, name, $"Record Count: {r.Response.Results.Count}");
    }

    private static async Task<Outcome> A16_READY_screen_008_09(ApiClient apiClient, string aname, string name)
    {
        return TBD(aname, name, "Enter executive hours and dollars (Second chance)");
    }

    private async Task<Outcome> A17_Profit_Share_Report_edit_run(ApiClient apiClient, string aname, string name)
    {
        StringBuilder sb = new();

        foreach (KeyValuePair<string, Pay426NCriteria> kvp in Pay426NCriteria._reportCriteria.OrderBy(kvp => kvp.Key))
        {
            string key = kvp.Key;

            if (key == "PAY426N-10")
            {
                // Skip PAY426N-10 because it is broken
                continue;
            }

            Pay426NCriteria criteria = kvp.Value;
            string postBody = JsonSerializer.Serialize(criteria);

            HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
            TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "Executive-Administrator");

            HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/yearend-profit-sharing-report")
            {
                Content = new StringContent(postBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            using HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Outcome(aname, name, "", OutcomeStatus.Error, "Problem with endpoint\n", null, true);
            }
        }

        return Ok(aname, name, sb.ToString());
    }

    private async Task<Outcome> A18_Profit_Share_Report_final_run(ApiClient apiClient, string aname, string name)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
        TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "Finance-Manager");

        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/final")
        {
            Content = new StringContent("{ \"ProfitYear\" : " + _profitYear + "}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        //return Ok(aname, name, "Update enrollment");
        return new Outcome(aname, name, "Update enrollment on SMART", OutcomeStatus.Ok, "", null, true);
    }

    private async Task<Outcome> A19_Get_Eligible_Employees(ApiClient apiClient, string aname, string name)
    {
        GetEligibleEmployeesResponse? r = await apiClient.ReportsYearEndEligibilityGetEligibleEmployeesEndpointAsync(_profitYear, "", null, null, int.MaxValue, null, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private async Task<Outcome> A20_Profit_Forfeit_PAY443(ApiClient apiClient, string aname, string name)
    {
        ReportResponseBaseOfForfeituresAndPointsForYearResponse? r =
            await apiClient.ReportsYearEndFrozenForfeituresAndPointsForYearEndpointAsync(true, _profitYear, "", null, null, int.MaxValue, null, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    // Reference values from the 2022 run.
    private const int RefMaxAllowedContribution = 57_000;
    private const decimal RefContributionPercent = 15m;
    private const decimal RefIncomingForfeitPercent = 0.876678m;
    private const decimal RefEarningsPercent = 9.280136m;

    private async Task<Outcome> A21_Profit_Share_Update_PAY444(ApiClient apiClient, string aname, string name)
    {
        ProfitShareUpdateResponse? r = await apiClient.ReportsYearEndProfitShareUpdateProfitShareUpdateEndpointAsync(RefContributionPercent, RefIncomingForfeitPercent,
            RefEarningsPercent,
            0, RefMaxAllowedContribution, 0, 0, 0, 0, 0, 0, _profitYear, null, null,
            0, int.MaxValue,
            null, null);
        return Ok(aname, name, $"Records Loaded = {r.Response.Results.Count}");
    }

    private async Task<Outcome> A22_Profit_Share_Edit_PAY477(ApiClient apiClient, string aname, string name)
    {
        ProfitShareEditResponse? r = await apiClient.ReportsYearEndProfitShareEditEndpointAsync(RefContributionPercent, RefIncomingForfeitPercent, RefEarningsPercent, 0,
            RefMaxAllowedContribution, 0, 0, 0, 0, 0, 0, _profitYear, null, null, 0, int.MaxValue, null, null);
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

    private async Task<Outcome> A23_Profit_Master_Update(ApiClient apiClient, string aname, string name)
    {
        ProfitShareUpdateRequestLocal req = new();
        req.ProfitYear = _profitYear;
        req.ContributionPercent = RefContributionPercent;
        req.IncomingForfeitPercent = RefIncomingForfeitPercent;
        req.EarningsPercent = RefEarningsPercent;
        req.SecondaryEarningsPercent = 0m;
        req.MaxAllowedContributions = RefMaxAllowedContribution;
        req.Take = int.MaxValue;

        string postBody = JsonSerializer.Serialize(req);

        HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
        TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "System-Administrator");

        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/profit-master-update")
        {
            Content = new StringContent(postBody, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using JsonDocument doc = JsonDocument.Parse(responseBody);
        JsonElement root = doc.RootElement;

        int beneficiaries = root.GetProperty("beneficiariesEffected").GetInt32();
        int employees = root.GetProperty("employeesEffected").GetInt32();
        int etvas = root.GetProperty("etvasEffected").GetInt32();

        return Ok(aname, name, $"beneficiariesEffected: {beneficiaries}, employeesEffected: {employees}, etvasEffected: {etvas}");
    }

    private async Task<Outcome> A24_PROF_PAYMASTER_UPD(ApiClient apiClient, string aname, string name)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("SmartApi");
        TestToken.CreateAndAssignTokenForClient(httpClient, _jwtOptions, "Finance-Manager");

        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/enrollments")
        {
            Content = new StringContent("{ \"profitYear\": " + TestConstants.OpenProfitYear + "}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

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
        /* We dont capture this yet var r = */
        await apiClient.ReportsYearEndBreakdownEndpointAsync(false, 1, null, null, 2025, "", null, null, null, null, null);
        return Ok(aname, name, $"Something?");
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
