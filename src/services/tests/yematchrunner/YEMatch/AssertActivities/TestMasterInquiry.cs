using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using YEMatch.AssertActivities.MasterInquiry;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace YEMatch.AssertActivities;

public sealed class TeeWriter(TextWriter one, TextWriter two) : TextWriter
{
    public override Encoding Encoding => one.Encoding;

    public override void Write(char value)
    {
        one.Write(value);
        two.Write(value);
    }

    public override void WriteLine(string? value)
    {
        one.WriteLine(value);
        two.WriteLine(value);
    }

    public override void Flush()
    {
        one.Flush();
        two.Flush();
    }
}

/* Activity which compares SMART master inquiry endpoint to READY master inquery screens (Saved in the OUTFL file.) */
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public class TestMasterInquiry : BaseSqlActivity
{
    private readonly HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
    public required ApiClient ApiClient { get; init; }

    public override async Task<Outcome> Execute()
    {
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");

        string path; //  = Path.Combine(AppContext.BaseDirectory, "OUTFL");
        path = "/Users/robertherrmann/prj/yerunner/src/services/tests/yematchrunner/YEMatch/Resources/MTPR.OUTFL";
        string content = await File.ReadAllTextAsync(path);
        List<OutFL> outties = OutFLParser.ParseStringIntoRecords(content);

        await using StreamWriter writer = new("compare.md") { AutoFlush = true };
        Console.SetOut(new TeeWriter(Console.Out, writer));

        int profitYear = 2025;
        int quantity = 50; // int.MaxValue;

        Console.WriteLine($"### Comparison of READY (local) MasterInquiry vs SMART (profitYear={profitYear})");
        Console.WriteLine("");
        Console.WriteLine($"Showing the first {quantity} differences of {outties.Count} Master Inquiry screen dumps.");
        Console.WriteLine("");

        int missing = 0;
        int bad = 0;
        int good = 0;
        Dictionary<string, int> diffsByFieldName = [];
        foreach (OutFL ready in outties)
        {
            MemberProfitPlanDetails? employeeDetails = await GetEmployeeDetails(profitYear, ready.OUT_SSN);
            if (employeeDetails == null)
            {
                missing++;
                bad++;
                if (bad < quantity)
                {
                    OutFLPrinter.ConsoleStock("READY", ready);
                    OutFLPrinter.ConsoleMissing("SMART");
                }
                else
                {
                    break;
                }

                continue;
            }

            OutFL smart = LoadFromSmart(employeeDetails);
            if (OutFLComparer.IsSame(ready, smart))
            {
                good++;
                continue;
            }

            bad++;
            if (bad < quantity)
            {
                OutFLPrinter.PrintComparisonTable(diffsByFieldName, ready, smart);
            }
            else
            {
                break;
            }
        }

        DumpColumnStats(missing, bad, good, diffsByFieldName);
        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }

    private OutFL LoadFromSmart(MemberProfitPlanDetails employeeDetails)
    {
        string outSsn = employeeDetails.Ssn;
        decimal outHrs = employeeDetails.YearToDateProfitSharingHours;
        int outYears = employeeDetails.YearsInPlan;
        string outEnrolled = EnrollmentToReady(employeeDetails.EnrollmentId);
        decimal outBeginBal = (decimal)employeeDetails.BeginPSAmount!;
        decimal outBeginVest = (decimal)employeeDetails.BeginVestedAmount!;
        decimal outCurrentBal = (decimal)employeeDetails.CurrentPSAmount!;
        decimal outVestingPct = employeeDetails.PercentageVested;
        decimal outVestingAmt = (decimal)employeeDetails.CurrentVestedAmount!;
//        bool outContLastYear = employeeDetails.ContributionsLastYear;
        decimal outEtva = employeeDetails.CurrentEtva;
        string outErrMesg = string.Join(",", employeeDetails.Missives);

        return new OutFL
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
            // OUT_CONT_LAST_YEAR = outContLastYear,
            OUT_CONT_LAST_YEAR = false,
            OUT_ETVA = outEtva,
            OUT_ERR_MESG = outErrMesg
        };
    }

    private static void DumpColumnStats(int missing, int bad, int good, Dictionary<string, int> diffsByFieldName)
    {
        Console.WriteLine("");
        Console.WriteLine($"Missing: {missing} Bad: {bad} Good: {good}");
        Console.WriteLine("");
        Console.WriteLine("| Field Name | Differences |");
        Console.WriteLine("|------------|------------ |");

        foreach ((string field, int count) in diffsByFieldName)
        {
            Console.WriteLine($"| {field} | {count} |");
        }

        Console.WriteLine("");
    }

    private static string EnrollmentToReady(byte? employeeDetailsEnrollmentId)
    {
        if (employeeDetailsEnrollmentId == null)
        {
            return " ";
        }

        return "*" + employeeDetailsEnrollmentId;
    }


    private async Task<MemberProfitPlanDetails?> GetEmployeeDetails(int profitYear, string ssn)
    {
        ApiClient apiClient = ApiClient;


        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/master/master-inquiry/search")
        {
            Content = new StringContent($$"""{"Ssn": {{ssn}},"memberType":1,"profitYear":2024}""",
                Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        // Console.WriteLine(("Requesting.... "))
        string responseBody = "";
        try
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed for SSN: {ssn}");
            Console.WriteLine(ex);
            throw;
        }


        PaginatedResponseDtoOfMemberDetails jresponse1 = JsonConvert.DeserializeObject<PaginatedResponseDtoOfMemberDetails>(responseBody)!;
        if (jresponse1.Total == 0)
        {
            return null;
        }

        MemberDetails memberDetails = jresponse1.Results.First();
        Console.WriteLine(memberDetails);

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
        int demographicsId = 77; // memberDetails.id
        if (demographicsId == 77)
        {
            throw new Exception("Not implemented");
        }

        HttpRequestMessage request2 = new(HttpMethod.Post, apiClient.BaseUrl + "api/master/master-inquiry/member")
        {
            Content = new StringContent(
                $$"""
                  {
                    "memberType": 1,
                    "id": {{demographicsId}},
                    "profitYear": {{profitYear}}
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
