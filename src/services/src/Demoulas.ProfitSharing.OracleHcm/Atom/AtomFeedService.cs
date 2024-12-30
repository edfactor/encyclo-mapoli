using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class AtomFeedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AtomFeedService> _logger;

    public AtomFeedService(HttpClient httpClient, ILogger<AtomFeedService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async IAsyncEnumerable<AtomFeedRecord> GetFeedDataAsync(string feedType, DateTime minDate, DateTime maxDate,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var url = $"/hcmRestApi/atomservlet/employee/{feedType}?published-min={minDate:yyyy-MM-ddTHH:mm:ssZ}&published-max={maxDate:yyyy-MM-ddTHH:mm:ssZ}";

        while (!string.IsNullOrWhiteSpace(url))
        {
            AtomFeedResponse? feedResponse;
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                var str = await response.Content.ReadAsStringAsync(cancellationToken);
                feedResponse = System.Text.Json.JsonSerializer.Deserialize<AtomFeedResponse>(str);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Atom feed: {Url}", url);
                break;
            }

            if (feedResponse?.Records != null)
            {
                foreach (var record in feedResponse.Records)
                {
                    yield return record;
                }
            }

            url = feedResponse?.NextPageUrl;
        }
    }
}
