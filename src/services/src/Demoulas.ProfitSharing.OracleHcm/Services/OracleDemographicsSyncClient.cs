using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Configuration;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

internal sealed class OracleDemographicsSyncClient
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public OracleDemographicsSyncClient(HttpClient httpClient, OracleHcmConfig oracleHcmConfig)
    {
        _httpClient = httpClient;
        _oracleHcmConfig = oracleHcmConfig;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    /// <summary>
    /// Will retrieve all employees from OracleHCM
    /// https://docs.oracle.com/en/cloud/saas/human-resources/24c/farws/op-workers-get.html
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<OracleEmployee?> GetAllEmployees([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string url = await BuildUrl(cancellationToken: cancellationToken);

        while (true)
        {
            using HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken);
            OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(_jsonSerializerOptions, cancellationToken);

            if (demographics?.Employees == null)
            {
                break;
            }

            foreach (OracleEmployee emp in demographics.Employees)
            {
                yield return emp;
            }

            if (!demographics.HasMore)
            {
                break;
            }

            // Construct the next URL for pagination
            string nextUrl = await BuildUrl(demographics.Count + demographics.Offset, cancellationToken);
            if (string.IsNullOrEmpty(nextUrl))
            {
                break;
            }

            url = nextUrl;
        }
    }

    private async Task<string> BuildUrl(int offset = 0, CancellationToken cancellationToken = default)
    {
        // Oracle will limit us to 500, but we run the risk of timeout well below that, so we need to be conservative.
        ushort limit = ushort.Min(50, _oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string>
        {
            { "limit", $"{limit}" },
            { "offset", $"{offset}" },
            { "totalResults", "false" },
            { "onlyData", "true" },
            { "fields", HttpRequestFields.ToFormattedString() }
        };
        UriBuilder initialUriBuilder = new UriBuilder(string.Concat(_oracleHcmConfig.BaseAddress, _oracleHcmConfig.DemographicUrl));
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken);
        initialUriBuilder.Query = initialQueryString;
        return initialUriBuilder.Uri.ToString();
    }

   

    private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        _ = response.EnsureSuccessStatusCode();
        return response;
    }
}
