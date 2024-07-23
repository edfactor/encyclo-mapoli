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

    public OracleDemographicsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<OracleEmployee?> GetAllEmployees(
        OracleHcmConfig oracleHcmConfig,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string initialUrl = await BuildUrl(oracleHcmConfig, cancellationToken: cancellationToken);
        ConcurrentQueue<OracleEmployee> queue = new ConcurrentQueue<OracleEmployee>();

        Task fetchTask = FetchOracleDemographic(oracleHcmConfig, initialUrl, queue, cancellationToken);

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

    private async Task<string> BuildUrl(OracleHcmConfig oracleHcmConfig, int offset = 0, CancellationToken cancellationToken = default)
    {
        // Oracle will limit us to 500.
        ushort limit = ushort.Min(500, oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string> { { "limit", $"{limit}" }, { "offset", $"{offset}" }, { "totalResults", "false" } };
        UriBuilder initialUriBuilder = new UriBuilder(oracleHcmConfig.Url);
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken);
        initialUriBuilder.Query = initialQueryString;
        return initialUriBuilder.Uri.ToString();
    }

    private Task FetchOracleDemographic(OracleHcmConfig oracleHcmConfig, string initialUrl, ConcurrentQueue<OracleEmployee> queue,
        CancellationToken cancellationToken)
    {
        // Task to fetch data and enqueue it
        return Task.Run(async () =>
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"{oracleHcmConfig.Username}:{oracleHcmConfig.Password}");
            string encodedAuth = Convert.ToBase64String(bytes);
            string url = initialUrl;

            while (true)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("REST-Framework-Version", oracleHcmConfig.RestFrameworkVersion);
                request.Headers.Add("Authorization", $"Basic {encodedAuth}");

                HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                OracleDemographics? demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(cancellationToken);

                if (demographics?.Items == null)
                {
                    break;
                }

                foreach (OracleEmployee emp in demographics.Items)
                {
                    queue.Enqueue(emp);
                }

                if (!demographics.HasMore)
                {
                    break;
                }

                // Construct the next URL for pagination
                string nextUrl = await BuildUrl(oracleHcmConfig, demographics.Offset + 1, cancellationToken);
                if (string.IsNullOrEmpty(nextUrl))
                {
                    break;
                }

                url = nextUrl;
            }
        }, cancellationToken);
    }
}
