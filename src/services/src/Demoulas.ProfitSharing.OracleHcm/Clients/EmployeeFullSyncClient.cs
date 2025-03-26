using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Clients;

internal sealed class EmployeeFullSyncClient
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<EmployeeFullSyncClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public EmployeeFullSyncClient(HttpClient httpClient, OracleHcmConfig oracleHcmConfig, 
        ILogger<EmployeeFullSyncClient> logger)
    {
        _httpClient = httpClient;
        _oracleHcmConfig = oracleHcmConfig;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    /// <summary>
    /// Will retrieve all employees from OracleHCM
    /// https://docs.oracle.com/en/cloud/saas/human-resources/24c/farws/op-workers-get.html
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<OracleEmployee[]> GetAllEmployees([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string url = await BuildUrl(cancellationToken: cancellationToken).ConfigureAwait(false);
        while (true)
        {
            using HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken).ConfigureAwait(false);
            OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);

            if (demographics?.Employees == null)
            {
                break;
            }

            foreach (var emps in demographics.Employees.Chunk(50))
            {
              yield return emps;
            }

            if (!demographics.HasMore)
            {
                break;
            }

            // Construct the next URL for pagination
            string nextUrl = await BuildUrl(demographics.Count + demographics.Offset, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(nextUrl))
            {
                break;
            }

            url = nextUrl;
        }
    }

    /// <summary>
    /// Will retrieve all employees from OracleHCM
    /// https://docs.oracle.com/en/cloud/saas/human-resources/24c/farws/op-workers-get.html
    /// </summary>
    /// <param name="oracleHcmId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<OracleEmployee[]> GetEmployee(long oracleHcmId, CancellationToken cancellationToken = default)
    {
        string url = await BuildUrl(oracleHcmId: oracleHcmId, cancellationToken: cancellationToken).ConfigureAwait(false);

        using HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken).ConfigureAwait(false);
        OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);

        return demographics?.Employees.ToArray() ?? [];
    }

    private async Task<string> BuildUrl(int offset = 0, long? oracleHcmId = null, CancellationToken cancellationToken = default)
    {
        // Oracle will limit us to 500, but we run the risk of timeout well below that, so we need to be conservative.
        ushort limit = ushort.Min(50, _oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string>()
        {
            { "limit", $"{limit}" },
            { "offset", $"{offset}" },
            { "totalResults", "false" },
            { "onlyData", "true" },
            { "fields", HttpRequestFields.ToFormattedString() }
        };

        string url = string.Concat(_oracleHcmConfig.BaseAddress, _oracleHcmConfig.DemographicUrl);
        if (oracleHcmId.HasValue)
        {
            // The query "q" params are CasE SensItIvE
            initialQuery.Add("q", $"PersonId={oracleHcmId}");
        }

        UriBuilder initialUriBuilder = new UriBuilder(url);
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        initialUriBuilder.Query = initialQueryString;
        string returnUrl = initialUriBuilder.Uri.ToString();

        if (Debugger.IsAttached)
        {
            _logger.LogInformation(returnUrl);
        }

        return returnUrl;
    }


    private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode && Debugger.IsAttached)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        _ = response.EnsureSuccessStatusCode();
        return response;
    }
}
