using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
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

    public async IAsyncEnumerable<TContextType> GetFeedDataAsync<TContextType>(string feedType, DateTime minDate, DateTime maxDate,
        [EnumeratorCancellation] CancellationToken cancellationToken) where TContextType : IDeltaContext
    {
        var url = $"/hcmRestApi/atomservlet/employee/{feedType}?page-size=25&page=1&published-min={minDate:yyyy-MM-ddTHH:mm:ssZ}&published-max={maxDate:yyyy-MM-ddTHH:mm:ssZ}";


        AtomFeedResponse<TContextType>? feedRoot = null;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            feedRoot = await response.Content.ReadFromJsonAsync<AtomFeedResponse<TContextType>>(cancellationToken);
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
