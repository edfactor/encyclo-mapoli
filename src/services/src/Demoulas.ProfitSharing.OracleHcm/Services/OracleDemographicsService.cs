using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

internal sealed class OracleDemographicsService
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public OracleDemographicsService(HttpClient httpClient, OracleHcmConfig oracleHcmConfig)
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
        string initialUrl = await BuildUrl(_oracleHcmConfig.Url, cancellationToken: cancellationToken);

        var returnValues = FetchOracleDemographic(initialUrl, cancellationToken);
        await foreach (var emp in returnValues)
        {
            yield return emp;
        }
    }

    private async Task<string> BuildUrl(string url, int offset = 0, CancellationToken cancellationToken = default)
    {
        // Oracle will limit us to 500, but we run the risk of timeout well below that, so we need to be conservative.
        ushort limit = ushort.Min(100, _oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string>
        {
            { "limit", $"{limit}" },
            { "offset", $"{offset}" },
            { "totalResults", "false" },
            { "onlyData", "true" },
            { "fields", HttpRequestFields.ToFormattedString() }
        };
        UriBuilder initialUriBuilder = new UriBuilder(url);
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken);
        initialUriBuilder.Query = initialQueryString;
        return initialUriBuilder.Uri.ToString();
    }

    private async IAsyncEnumerable<OracleEmployee?> FetchOracleDemographic(string initialUrl, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        OracleHcmActivitySource.Instance.StartActivity(nameof(FetchOracleDemographic), ActivityKind.Internal);
        // Task to fetch data and enqueue it
        string url = initialUrl;

        while (true)
        {
            HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken);
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
            string nextUrl = await BuildUrl(_oracleHcmConfig.Url, demographics.Count + demographics.Offset, cancellationToken);
            if (string.IsNullOrEmpty(nextUrl))
            {
                break;
            }

            url = nextUrl;
        }
    }

    private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
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
