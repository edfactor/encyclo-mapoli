using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demoulas.ProfitSharing.OracleHcm.HealthCheck;

public class OracleHcmHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly OracleHcmConfig _oracleHcmConfig;

    public OracleHcmHealthCheck(HttpClient httpClient, OracleHcmConfig oracleHcmConfig)
    {
        _httpClient = httpClient;
        _oracleHcmConfig = oracleHcmConfig;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        string url = string.Empty; // Declare the URL outside the try block
        try
        {
            url = await BuildUrl(cancellationToken).ConfigureAwait(false);
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Oracle HCM is available.");
            }

            return HealthCheckResult.Degraded($"Oracle HCM is not available : Response Status Code {response.StatusCode} for Url '{url}'");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded($"Oracle HCM check failed: {ex.Message}", ex, new Dictionary<string, object> { { "url", url } });
        }
    }


    private async Task<string> BuildUrl(CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> initialQuery = new Dictionary<string, string>
        {
            { "limit", "1" }, { "totalResults", "false" }, { "onlyData", "true" }, { "fields", "PersonNumber" }
        };
        UriBuilder initialUriBuilder = new UriBuilder(string.Concat(_oracleHcmConfig.BaseAddress, _oracleHcmConfig.DemographicUrl));
        string initialQueryString = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        initialUriBuilder.Query = initialQueryString;
        return initialUriBuilder.Uri.ToString();
    }
}
