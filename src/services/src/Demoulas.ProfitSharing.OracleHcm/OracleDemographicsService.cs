using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

namespace Demoulas.ProfitSharing.OracleHcm;

public sealed class OracleDemographicsService
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly string _encodedAuth;

    public OracleDemographicsService(HttpClient httpClient, OracleHcmConfig oracleHcmConfig)
    {
        _httpClient = httpClient;
        _oracleHcmConfig = oracleHcmConfig;
        byte[] bytes = Encoding.UTF8.GetBytes($"{_oracleHcmConfig.Username}:{_oracleHcmConfig.Password}");
        _encodedAuth = Convert.ToBase64String(bytes);
    }

    public async IAsyncEnumerable<OracleEmployee?> GetAllEmployees([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string initialUrl = await BuildUrl(_oracleHcmConfig.Url, cancellationToken: cancellationToken);
        ConcurrentQueue<OracleEmployee> queue = new ConcurrentQueue<OracleEmployee>();

        Task fetchTask = FetchOracleDemographic(initialUrl, queue, cancellationToken);

        // Yield results from the queue
        while (!fetchTask.IsCompleted || !queue.IsEmpty)
        {
            while (queue.TryDequeue(out OracleEmployee? emp))
            {
                yield return emp;
            }
        }

        await fetchTask; // Ensure fetch task completes
    }

    public async Task<string> GetEmployeeAddress(long workersUniqID, CancellationToken cancellationToken = default)
    {
        string url = $"{_oracleHcmConfig.Url}/{workersUniqID}/child/addresses";
        var addressUrl = await BuildUrl(url, cancellationToken: cancellationToken);
        HttpResponseMessage response = await GetOracleHcmValue(addressUrl, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private async Task<string> BuildUrl(string url, int offset = 0, CancellationToken cancellationToken = default)
    {
        // Oracle will limit us to 500.
        ushort limit = ushort.Min(500, _oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string> { { "limit", $"{limit}" }, { "offset", $"{offset}" }, { "totalResults", "false" } };
        UriBuilder initialUriBuilder = new UriBuilder(url);
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken);
        initialUriBuilder.Query = initialQueryString;
        return initialUriBuilder.Uri.ToString();
    }

    private Task FetchOracleDemographic(string initialUrl, ConcurrentQueue<OracleEmployee> queue,
        CancellationToken cancellationToken)
    {
        // Task to fetch data and enqueue it
        return Task.Run(async () =>
        {
            string url = initialUrl;

            while (true)
            {
                HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken);
                OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(cancellationToken);

                if (demographics?.Items == null)
                {
                    break;
                }

                foreach (OracleEmployee emp in demographics.Items)
                {
                    var address = await GetEmployeeAddress(emp.PersonId, cancellationToken);
                    Console.WriteLine(address);

                    queue.Enqueue(emp);
                }

                if (!demographics.HasMore)
                {
                    break;
                }

                // Construct the next URL for pagination
                string nextUrl = await BuildUrl(_oracleHcmConfig.Url, demographics.Offset + 1, cancellationToken);
                if (string.IsNullOrEmpty(nextUrl))
                {
                    break;
                }

                url = nextUrl;
            }
        }, cancellationToken);
    }

    private async Task<HttpResponseMessage> GetOracleHcmValue(string url, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("REST-Framework-Version", _oracleHcmConfig.RestFrameworkVersion);
        request.Headers.Add("Authorization", $"Basic {_encodedAuth}");

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
