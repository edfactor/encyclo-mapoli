using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime;
using System.Text.Json;
using System.Threading.Channels;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Clients;

internal class PayrollSyncClient
{
    // Define constants for balance type IDs


    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly EmployeeFullSyncClient _oracleEmployeeDataSyncClient;
    private readonly IProcessWatchdog _watchdog;
    private readonly ILogger<PayrollSyncClient> _logger;
    private readonly Channel<MessageRequest<PayrollItem[]>> _payrollSyncBus;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public PayrollSyncClient(HttpClient httpClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IEmployeeSyncService employeeSyncService,
        OracleHcmConfig oracleHcmConfig,
        EmployeeFullSyncClient oracleEmployeeDataSyncClient,
        IProcessWatchdog watchdog,
        ILogger<PayrollSyncClient> logger,
        Channel<MessageRequest<PayrollItem[]>> payrollSyncBus)
    {
        _httpClient = httpClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _employeeSyncService = employeeSyncService;
        _oracleHcmConfig = oracleHcmConfig;
        _oracleEmployeeDataSyncClient = oracleEmployeeDataSyncClient;
        _watchdog = watchdog;
        _logger = logger;
        _payrollSyncBus = payrollSyncBus;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    /// <summary>
    /// Retrieves payroll balances asynchronously by processing payroll data for specified individuals.
    /// </summary>
    /// <param name="requestedBy">
    /// The identifier of the entity or user requesting the operation. Defaults to "System".
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method initiates a job to synchronize payroll data, processes payroll balances for individuals
    /// with valid Oracle HCM IDs, and updates the job status upon completion or failure.
    /// </remarks>
    /// <exception cref="Exception">
    /// Logs critical errors if an exception occurs during the payroll synchronization process.
    /// </exception>
    public async Task RetrievePayrollBalancesAsync(string requestedBy = Constants.SystemAccountName, CancellationToken cancellationToken = default)
    {
        using Activity? activity = OracleHcmActivitySource.Instance.StartActivity(nameof(RetrievePayrollBalancesAsync), ActivityKind.Internal);

        Job job = new Job
        {
            JobTypeId = JobType.Constants.PayrollSyncFull,
            StartMethodId = StartMethod.Constants.System,
            RequestedBy = requestedBy,
            JobStatusId = JobStatus.Constants.Running,
            Started = DateTime.Now
        };

        await _profitSharingDataContextFactory.UseWritableContext(db =>
        {
            db.Jobs.Add(job);
            return db.SaveChangesAsync(cancellationToken);
        }, cancellationToken).ConfigureAwait(false);

        bool success = true;
        try
        {
            // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
            await GetPayrollProcessResultsAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogCritical(ex, ex.Message);
        }
        finally
        {
            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);

#pragma warning disable S1215
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
#pragma warning restore S1215
        }
    }

    /// <summary>
    /// Retrieves payroll process results from the Oracle HCM system and processes them using the provided callback function.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method retrieves payroll process results in a paginated manner, processes them using the provided callback,
    /// and continues fetching until all results are processed or an error occurs.
    /// </remarks>
    private async Task GetPayrollProcessResultsAsync(CancellationToken cancellationToken)
    {

        string url = await BuildUrl(cancellationToken: cancellationToken).ConfigureAwait(false);
        while (true)
        {
            using HttpResponseMessage response = await GetOraclePayrollValue(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                break;
            }

            PayrollRoot? results = await response.Content.ReadFromJsonAsync<PayrollRoot>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
            if ((results?.Count ?? 0) == 0)
            {
                return;
            }

            // Queue Here
            const string requestedBy = "System";
            int payrollChunkCounter = 0;
            foreach (PayrollItem[] items in results!.Items.Chunk(15))
            {
                MessageRequest<PayrollItem[]> message = new() { ApplicationName = nameof(PayrollSyncClient), Body = items, UserId = requestedBy };

                await _payrollSyncBus.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);

                // Record heartbeat every 20 chunks to prevent watchdog timeout during long-running payroll sync
                if (++payrollChunkCounter % 20 == 0)
                {
                    _watchdog.RecordHeartbeat();
                    _logger.LogDebug("PayrollSync: Queued {ChunkCount} payroll chunks ({ItemCount} items)",
                        payrollChunkCounter, payrollChunkCounter * 15);
                }
            }

            // Record heartbeat before attempting missing employee sync (can take time)
            _watchdog.RecordHeartbeat();
            await TrySyncMissingEmployees(results!.Items, cancellationToken).ConfigureAwait(false);

            if (!results.HasMore)
            {
                break;
            }

            // Construct the next URL for pagination
            string nextUrl = await BuildUrl(results.Count + results.Offset, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(nextUrl))
            {
                break;
            }

            url = nextUrl;
        }
    }

    private async Task TrySyncMissingEmployees(IReadOnlyList<PayrollItem> items, CancellationToken cancellationToken)
    {
        HashSet<long> existsCollection = items.Select(d => d.PersonId).ToHashSet();
        List<long> missingPersonIds = await _profitSharingDataContextFactory.UseReadOnlyContext(async c =>
        {
            // Query only the relevant PersonIds from the database
            HashSet<long> existingPersonIds = await c.Demographics
                .Where(d => existsCollection.Contains(d.OracleHcmId))
                .Select(d => d.OracleHcmId)
                .ToHashSetAsync(cancellationToken).ConfigureAwait(false);

            // Find PersonIds that are in existsCollection but not in the database
            return existsCollection.Except(existingPersonIds).ToList();
        }, cancellationToken).ConfigureAwait(false);

        foreach (long id in missingPersonIds)
        {
            OracleEmployee[] oracleHcmEmployees = await _oracleEmployeeDataSyncClient.GetEmployee(id, cancellationToken).ConfigureAwait(false);
            await _employeeSyncService.QueueEmployee(Constants.SystemAccountName, oracleHcmEmployees, cancellationToken).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Constructs a URL for retrieving payroll data from the Oracle HCM system.
    /// </summary>
    /// <param name="offset">
    /// The offset value used for pagination. Defaults to 0.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with a result of the constructed URL as a <see cref="string"/>.
    /// </returns>
    /// <remarks>
    /// The URL is built using the base address and payroll URL from the Oracle HCM configuration,
    /// along with query parameters such as limit, offset, and specific fields to retrieve.
    /// </remarks>
    private async Task<string> BuildUrl(int offset = 0, CancellationToken cancellationToken = default)
    {
        const int payrollActionId = 2003;
        ushort limit = ushort.Min(byte.MaxValue, _oracleHcmConfig.Limit);
        Dictionary<string, string> initialQuery = new Dictionary<string, string>()
        {
            { "limit", $"{limit}" },
            { "offset", $"{offset}" },
            { "totalResults", "false" },
            { "onlyData", "true" },
            { "q", $"PayrollActionId={payrollActionId}" },
            { "fields", "PayrollActionId,ObjectActionId,SubmissionDate,PersonId" }
        };

        string url = string.Concat(_oracleHcmConfig.BaseAddress, _oracleHcmConfig.PayrollUrl);
        UriBuilder initialUriBuilder = new UriBuilder(url) { Query = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken).ConfigureAwait(false) };
        return initialUriBuilder.Uri.ToString();
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified Oracle Payroll API URL and retrieves the response.
    /// </summary>
    /// <param name="url">The URL of the Oracle Payroll API endpoint to send the request to.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="HttpResponseMessage"/> returned by the Oracle Payroll API.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP response indicates an unsuccessful status code.
    /// </exception>
    /// <remarks>
    /// This method ensures that the HTTP response has a successful status code before returning it.
    /// If the response is unsuccessful, the content of the response is logged to the console.
    /// </remarks>
    private async Task<HttpResponseMessage> GetOraclePayrollValue(string url, CancellationToken cancellationToken)
    {
        await Task.Delay(new TimeSpan(0, 0, 15), cancellationToken).ConfigureAwait(false);

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

        _ = response.EnsureSuccessStatusCode();
        return response;
    }
}
