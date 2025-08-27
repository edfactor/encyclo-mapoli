using System.Net.Http.Headers;
using System.Text;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

/* Uses SMART to rebuild the 2023 ZeroContribution flag.  This flag is used in the vesting calculation, so we need to compute it for SMART to use */
public class S24_Rebuild2023Enrollment : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {

       // private static async Task<Outcome> A24_PROF_PAYMASTER_UPD(ApiClient apiClient, string aname, string name)
    
        // curl -X 'POST' \
        // 'https://ps.qa.demoulas.net:8443/api/yearend/update-enrollment' \
        // -H 'accept: */*' \
        // -H 'Impersonation: Finance-Manager' \
        // -H 'Authorization: Bearer eyJraWQiOiJkZUZKc3o3OTVsNU1wQ2RUdlVVY3JaOEFUbFdXM0t2NFFjZ0dLa0NPZU5RIiwiYWxnIjoiUlMyNTYifQ.eyJ2ZXIiOjEsImp0aSI6IkFULk5MYkwyZEtUQld4OEZrV1FFckFrZ3hlSVFBLVJqUWdmQjluQzN2T1I1U2siLCJpc3MiOiJodHRwczovL21hcmtldGJhc2tldC5va3RhLmNvbS9vYXV0aDIvYXVzMTEzZWhjNWtrcVRlWEIxdDgiLCJhdWQiOiJhcGk6Ly9zbWFydC1wcyIsImlhdCI6MTc1MDc4NzcxNSwiZXhwIjoxNzUwNzkxMzE1LCJjaWQiOiIwb2ExMTNlYWpqNEpUMWlrSjF0OCIsInVpZCI6IjAwdXd3dG1vcjhTdDNRbFlxMXQ3Iiwic2NwIjpbIm9wZW5pZCJdLCJhdXRoX3RpbWUiOjE3NTA3NjcxODUsInN1YiI6ImJoZXJybWFubkBNYWlub2ZmaWNlLkRlbW91bGFzLkNvcnAiLCJncm91cHMiOlsiU01BUlQtUFMtUUEtSW1wZXJzb25hdGlvbiJdfQ.m8ITgSbziHEvh6NBSnVo0pnrUIjsk4ZDWXoiP86Jdkgcz4BAAuYBI3euaXoRocWGdEfUEVmBmqjldkeAzXeoIKsp28L04n06cqzXsP6_a_y_8X9TptRxDz3dBROu5r_pGbsKXmGP8S4UaJYwiWTWc-8FDf0aM3GML0vMEQzoZE5nS8_IqEUpiyEu1anmqMMJ1Oc6t0kSbxBVSK7qkoMKsoEtB64OX2_qDrKXAzNlQg-6_w_m9U1Lgvq-iriXcU1KxMfL-HSjIa0e4LSRJcI7WrCRatA-bdg3_nRik6KW3dZNMj1vnLRqdQkJq6-O5wrIDKJ3Ek2EqJPMYMllv7oycQ' \
        // -H 'Content-Type: application/json' \
        // -d '{  "profitYear": 2024 }'

        ApiClient apiClient = SmartActivityFactory.Client!;

        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "IT-Operations");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/update-enrollment")
        {
            Content = new StringContent("{ \"profitYear\": 2023}", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return new Outcome("EnrollmentRebuild", "EnrollmentRebuild", "Updated enrollment for 2023", OutcomeStatus.Ok, "", null,  true, "", "");
    }

}
