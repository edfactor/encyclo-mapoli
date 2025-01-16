using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Clients;

internal sealed class OracleEmployeeDataSyncClient
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<OracleEmployeeDataSyncClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public OracleEmployeeDataSyncClient(HttpClient httpClient, OracleHcmConfig oracleHcmConfig, ILogger<OracleEmployeeDataSyncClient> logger)
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
            string nextUrl = await BuildUrl(demographics.Count + demographics.Offset, cancellationToken: cancellationToken);
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
    internal async IAsyncEnumerable<OracleEmployee?> GetEmployee(long oracleHcmId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string url = await BuildUrl(oracleHcmId: oracleHcmId, cancellationToken: cancellationToken);

        using HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken);
        OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(_jsonSerializerOptions, cancellationToken);

        if (demographics?.Employees == null)
        {
            yield return null;
        }


        foreach (OracleEmployee emp in demographics!.Employees)
        {
            yield return emp;
        }
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
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken);
        initialUriBuilder.Query = initialQueryString;
        var returnUrl = initialUriBuilder.Uri.ToString();

        if (Debugger.IsAttached)
        {
            _logger.LogInformation(returnUrl);
        }

        return returnUrl;
    }


    private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
    {
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode && Debugger.IsAttached)
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
        catch (SocketException e)
        {
            _logger.LogError(e, e.Message);
        }

        return new HttpResponseMessage();
    }
}
