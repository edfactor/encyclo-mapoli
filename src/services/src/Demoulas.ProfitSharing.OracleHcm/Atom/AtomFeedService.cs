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

    public async IAsyncEnumerable<Context> GetFeedDataAsync(string feedType, DateTime minDate, DateTime maxDate,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var url = $"/hcmRestApi/atomservlet/employee/{feedType}?published-min={minDate:yyyy-MM-ddTHH:mm:ssZ}&published-max={maxDate:yyyy-MM-ddTHH:mm:ssZ}";


        AtomFeedResponse? feedRoot = null;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            feedRoot = await response.Content.ReadFromJsonAsync<AtomFeedResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Atom feed: {Url}", url);
        }

        if (feedRoot?.Feed.Entries != null)
        {
            foreach (var record in feedRoot.Feed.Entries.Select(e => e.Content).SelectMany(c => c.Context)!)
            {
                yield return record;
            }
        }
    }
}
