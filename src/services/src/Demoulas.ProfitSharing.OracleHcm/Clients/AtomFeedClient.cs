using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Clients;

internal class AtomFeedClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AtomFeedClient> _logger;

    public AtomFeedClient(HttpClient httpClient, ILogger<AtomFeedClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves data from an Atom feed asynchronously based on the specified feed type and date range.
    /// </summary>
    /// <typeparam name="TContextType">
    /// The type of the context representing the feed data. Must inherit from <see cref="DeltaContextBase"/>.
    /// </typeparam>
    /// <param name="feedType">
    /// The type of the feed to retrieve, such as "newhire", "empassignment", etc.
    /// </param>
    /// <param name="minDate">
    /// The minimum date for filtering feed entries. Only entries published after this date will be included.
    /// </param>
    /// <param name="maxDate">
    /// The maximum date for filtering feed entries. Only entries published before this date will be included.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// An asynchronous stream of feed data of type <typeparamref name="TContextType"/>.
    /// </returns>
    /// <remarks>
    /// This method constructs a URL to query the Atom feed API, retrieves the feed data, and yields the parsed entries.
    /// If an error occurs during the fetch operation, it logs the error and skips processing.
    /// </remarks>
    internal async IAsyncEnumerable<TContextType> GetFeedDataAsync<TContextType>(
        string feedType,
        DateTimeOffset minDate,
        DateTimeOffset maxDate,
        [EnumeratorCancellation] CancellationToken cancellationToken
    ) where TContextType : DeltaContextBase
    {
        int page = 1;
        bool hasMoreData;
        maxDate = maxDate.AddDays(1).Date;

        do
        {
            string url = $"/hcmRestApi/atomservlet/employee/{feedType}?page-size=25&page={page}&published-min={minDate:yyyy-MM-ddTHH:mm:ssZ}&published-max={maxDate:yyyy-MM-ddTHH:mm:ssZ}";
            AtomFeedResponse<TContextType>? feedRoot = null;

            try
            {

                await Task.Delay(new TimeSpan(0, 0, 10), cancellationToken).ConfigureAwait(false);

                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
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

                response.EnsureSuccessStatusCode();

                feedRoot = await response.Content.ReadFromJsonAsync<AtomFeedResponse<TContextType>>(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Atom feed: {Url}", Uri.EscapeDataString(url));
                yield break;
            }

            if (feedRoot?.Feed.Entries != null && feedRoot.Feed.Entries.Any())
            {
                foreach (TContextType record in feedRoot.Feed.Entries
                             .Select(e => e.Content)
                             .SelectMany(c => c.Context)!)
                {
                    record.FeedType = feedType;
                    yield return record;
                }

                hasMoreData = true;
                page++;
            }
            else
            {
                hasMoreData = false;
            }
        } while (hasMoreData);
    }
}
