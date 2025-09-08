using System.Net.Http.Headers;
using System.Text;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

/* Uses SMART to rebuild the 2023 ZeroContribution flag.  This flag is used in the vesting calculation, so we need to compute it for SMART to use */

public class S18_Rebuild2023ZeroCont : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        ApiClient apiClient = SmartActivityFactory.Client!;

        HttpClient httpClient = new() { Timeout = TimeSpan.FromHours(2) };
        TestToken.CreateAndAssignTokenForClient(httpClient, "Finance-Manager");
        HttpRequestMessage request = new(HttpMethod.Post, apiClient.BaseUrl + "api/yearend/final")
        {
            Content = new StringContent("{ \"ProfitYear\" :  2023, \"Rebuild\": true }", Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);

        return new Outcome("ZeroContribRebuild", "ZeroContribRebuild", "Updated ZeroContrib for 2023", OutcomeStatus.Ok, "", null, true, "", "");
    }
}
