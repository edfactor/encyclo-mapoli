using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.Util.Extensions;
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
        // Track offset explicitly so we can advance to the next batch when a transient
        // error occurs while reading the response (so we don't stop the whole enumeration).
        int offset = 0;
        int batchSize = Math.Min(75, (int)_oracleHcmConfig.Limit);
        const int maxConsecutiveFailures = 5;
        int consecutiveFailures = 0;

        string url = await BuildUrl(offset, cancellationToken: cancellationToken).ConfigureAwait(false);
        while (true)
        {
            using HttpResponseMessage response = await GetOracleHcmValue(url, cancellationToken).ConfigureAwait(false);
            OracleDemographics? demographics = null;

            try
            {
                demographics = await response.Content.ReadFromJsonAsync<OracleDemographics>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                // reset consecutive failures on success
                consecutiveFailures = 0;
            }
            catch (Exception e)
            {
                // Log the failure but continue to the next batch instead of terminating the whole enumeration.
                consecutiveFailures++;
                _logger.LogError(e, "Failed to retrieve employee demographics batch at offset {Offset} (failure {FailureCount}/{MaxFailures}): {Error}", offset,
                    consecutiveFailures, maxConsecutiveFailures, e.Message);

                if (consecutiveFailures >= maxConsecutiveFailures)
                {
                    _logger.LogCritical("Aborting iteration after {MaxFailures} consecutive failures starting at offset {Offset}", maxConsecutiveFailures, offset);
                    break;
                }

                // Advance by one batch and continue. This keeps the IAsyncEnumerable alive even
                // when a single batch fails to deserialize.
                offset += batchSize;
                string nextUrlFallback = await BuildUrl(offset, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrEmpty(nextUrlFallback))
                {
                    break;
                }

                url = nextUrlFallback;
                continue;
            }

            if (demographics?.Employees == null)
            {
                break;
            }

            foreach (var emps in demographics.Employees.Chunk(batchSize))
            {
                yield return emps;
            }

            if (!demographics.HasMore)
            {
                break;
            }

            // Per the Oracle Consultants, we need to slow this operation down, or continue to risk 401 errors as a way of handling too many requests.
            await Task.Delay(new TimeSpan(0, 0, 10), cancellationToken).ConfigureAwait(false);

            // Use the server-provided pagination information when available.
            offset = demographics.Count + demographics.Offset;
            string nextUrl = await BuildUrl(offset, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        ushort limit = ushort.Min(100, _oracleHcmConfig.Limit);
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

        // Copy default headers from HttpClient to the request message
        // (SendAsync does NOT automatically add Authorization and other default headers)
        foreach (var header in _httpClient.DefaultRequestHeaders)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode && Debugger.IsAttached)
        {
            string errorResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogWarning("Oracle HCM API request failed: {ErrorResponse} / {ReasonPhrase}", errorResponse, response.ReasonPhrase);

            // Generate and display cURL command for manual testing
            string curlCommand = request.GenerateCurlCommand(url);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("=== API REQUEST FAILED ===");
            Console.WriteLine(errorResponse);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== cURL Command for Postman/Manual Testing ===");
            Console.WriteLine(curlCommand);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            _logger.LogInformation("cURL command for manual testing: {CurlCommand}", curlCommand);
        }

        _ = response.EnsureSuccessStatusCode();
        return response;
    }
}
