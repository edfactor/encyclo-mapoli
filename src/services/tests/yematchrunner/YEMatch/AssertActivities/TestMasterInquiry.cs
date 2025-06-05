using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace YEMatch;

public class TestMasterInquiry : BaseSqlActivity
{
    private readonly HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };

    public override async Task<Outcome> Execute()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "OUTFL");
        string content = await File.ReadAllTextAsync(path);

        List<OutFL> outties = OutFL.ParseStringIntoRecords(content);

        Console.WriteLine($"Count of reference MasterInquiry Screens {outties.Count}");

        int missing = 0;
        int bad = 0;
        int good = 0;
        int quantity = 1000;
        foreach (OutFL ready in outties.Slice(0, quantity))
        {

            MemberProfitPlanDetails? employeeDetails = await GetEmployeeDetails(ready.OUT_SSN);
            if (employeeDetails == null)
            {
                OutFL.HeaderIfNeeded();
                missing++;
                ready.ConsoleStock("READY");
                OutFL.ConsoleMissing("SMART");
            }
            else
            {
                string outSsn = employeeDetails.Ssn;
                decimal outHrs = employeeDetails.YearToDateProfitSharingHours;
                int outYears = employeeDetails.YearsInPlan;
                string outEnrolled = EnrollmentToReady(employeeDetails.EnrollmentId);
                decimal outBeginBal = employeeDetails.BeginPSAmount;
                decimal outBeginVest = employeeDetails.BeginVestedAmount;
                decimal outCurrentBal = employeeDetails.CurrentPSAmount;
                decimal outVestingPct = employeeDetails.PercentageVested;
                decimal outVestingAmt = employeeDetails.CurrentVestedAmount;
                string outErrMesg = string.Join(",", employeeDetails.Missives);

                OutFL smart = new()
                {
                    OUT_SSN = outSsn,
                    OUT_HRS = outHrs,
                    OUT_YEARS = outYears,
                    OUT_ENROLLED = outEnrolled,
                    OUT_BEGIN_BAL = outBeginBal,
                    OUT_BEGIN_VEST = outBeginVest,
                    OUT_CURRENT_BAL = outCurrentBal,
                    OUT_VESTING_PCT = outVestingPct,
                    OUT_VESTING_AMT = outVestingAmt,
                    OUT_ERR_MESG = outErrMesg
                };

                if (!OutFL.IsSame(ready, smart))
                {
                    bad++;
                    OutFL.PrintComparisonTable(ready, smart);
                }
                else
                {
                    good++;
                }
            }
        }

        Console.WriteLine($"Missing: {missing} Bad: {bad} Good: {good} of tested {quantity}");

        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }

    private static string EnrollmentToReady(byte? employeeDetailsEnrollmentId)
    {
        if (employeeDetailsEnrollmentId == null)
        {
            return " ";
        }

        return "*" + employeeDetailsEnrollmentId;
    }


    private async Task<MemberProfitPlanDetails?> GetEmployeeDetails(string ssn)
    {
        ApiClient apiClient = SmartActivityFactory.Client!;

        TestToken.CreateAndAssignTokenForClient(httpClient, "IT-Operations");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/master/master-inquiry/search")
        {
            Content = new StringContent($$"""{"Ssn": {{ssn}},"memberType":1,"profitYear":2024}""",
                Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        // Console.WriteLine(("Requesting.... "));
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        PaginatedResponseDtoOfMemberDetails jresponse1 = JsonConvert.DeserializeObject<PaginatedResponseDtoOfMemberDetails>(responseBody)!;
        if (jresponse1.Total == 0)
        {
            return null;
        }

        MemberDetails memberDetails = jresponse1.Results.First();

/*
        curl -X 'POST' \
        'https://localhost:7141/api/master/master-inquiry/member' \
        -H 'accept: application/json' \
        -H 'Impersonation: Finance-Manager' \
        -H 'Authorization: Bearer eyJraWQiOiJkZUZKc3o3OTVsNU1wQ2RUdlVVY3JaOEFUbFdXM0t2NFFjZ0dLa0NPZU5RIiwiYWxnIjoiUlMyNTYifQ.eyJ2ZXIiOjEsImp0aSI6IkFULlN0ay1xcFZDTmswR2VPYk1sX19BbndKVkF6OTZPQ191bTBSb251QXpHdEkiLCJpc3MiOiJodHRwczovL21hcmtldGJhc2tldC5va3RhLmNvbS9vYXV0aDIvYXVzMTEzZWhjNWtrcVRlWEIxdDgiLCJhdWQiOiJhcGk6Ly9zbWFydC1wcyIsImlhdCI6MTc0ODkxMTU4MSwiZXhwIjoxNzQ4OTE1MTgxLCJjaWQiOiIwb2ExMTNlYWpqNEpUMWlrSjF0OCIsInVpZCI6IjAwdXd3dG1vcjhTdDNRbFlxMXQ3Iiwic2NwIjpbIm9wZW5pZCJdLCJhdXRoX3RpbWUiOjE3NDg5MTE1ODAsInN1YiI6ImJoZXJybWFubkBNYWlub2ZmaWNlLkRlbW91bGFzLkNvcnAiLCJncm91cHMiOlsiU01BUlQtUFMtUUEtSW1wZXJzb25hdGlvbiJdfQ.j9X3DTJLhHwFgovhEzvi9LuYjGjKvUTmJedFLo86AHMbFCw9Al8weBZsoIP3aZ01vA3SHmgjsOLW309CKXg4igx0C_8wgKox1OmIMDgi_50ln61q2Cb4uiEiFq7AT79QB4Uj4_y1YK0CGtuxRb0Ogwsupo3lYelYFZDm0MZpiUirCODnxdj0gGYhRcptL6eKbVf-SAp019HhSDFfRgvXqo5c6RfT_i7J0FROpcZjlxAmGGha--_v1fOUxLtttxeF-4JC58CKskQz0KJByKgA3Y6sX9gIEzlVy8Hh6Lsw6gloA-tWO_5SQpI6Ecd-fkTU4qNeyclpaUs-HS7FoR_GXg' \
        -H 'Content-Type: application/json' \
        -d '{
        "memberType": 1,
        "id": 357807,
        "profitYear": 2023,
        "sortBy": null,
        "isSortDescending": null,
        "skip": 0,
        "take": 255
    }'
    */
        int Id = memberDetails.Id;
        HttpRequestMessage request2 = new(HttpMethod.Post, apiClient.BaseUrl + "api/master/master-inquiry/member")
        {
            Content = new StringContent(
                $$"""
                  {
                    "memberType": 1,
                    "id": {{Id}},
                    "profitYear": 2025
                  }
                  """,
                Encoding.UTF8, "application/json")
        };
        request2.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        //Console.WriteLine(("Requesting.... "));
        using HttpResponseMessage response2 = await httpClient.SendAsync(request2);
        response2.EnsureSuccessStatusCode();

        string responseBody2 = await response2.Content.ReadAsStringAsync();
        MemberProfitPlanDetails jresponse = JsonConvert.DeserializeObject<MemberProfitPlanDetails>(responseBody2)!;


        return jresponse;
    }
}
