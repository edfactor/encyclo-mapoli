using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Clients;

internal class PayrollSyncClient
{
    // Define constants for balance type IDs
    private static class BalanceTypeIds
    {
        public const long MbProfitSharingDollars = 300000789345470; // MB Profit Sharing Dollars
        public const long MbProfitSharingHours = 300000789345477; // MB Profit Sharing Hours
        public const long MbProfitSharingWeeks = 300000785152356; // MB Profit Sharing Weeks
    }

    private readonly List<long> _balanceTypeIds =
        [BalanceTypeIds.MbProfitSharingDollars, BalanceTypeIds.MbProfitSharingHours, BalanceTypeIds.MbProfitSharingWeeks];

    // Constants for other parameters
    private const string PLDGId = "300000005030436";
    private const string PLC = "US";

    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly OracleEmployeeDataSyncClient _oracleEmployeeDataSyncClient;
    private readonly ILogger<PayrollSyncClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PayrollSyncClient(HttpClient httpClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IEmployeeSyncService employeeSyncService,
        OracleHcmConfig oracleHcmConfig,
        OracleEmployeeDataSyncClient oracleEmployeeDataSyncClient,
        ILogger<PayrollSyncClient> logger)
    {
        _httpClient = httpClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _employeeSyncService = employeeSyncService;
        _oracleHcmConfig = oracleHcmConfig;
        _oracleEmployeeDataSyncClient = oracleEmployeeDataSyncClient;
        _logger = logger;
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
    public async Task RetrievePayrollBalancesAsync(string requestedBy = "System", CancellationToken cancellationToken = default)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(RetrievePayrollBalancesAsync), ActivityKind.Internal);

        var job = new Job
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
        }, cancellationToken);

        bool success = true;
        try
        {
            // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
            await GetPayrollProcessResultsAsync(GetBalanceTypesForProcessResultsAsync, cancellationToken);
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
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves payroll process results from the Oracle HCM system and processes them using the provided callback function.
    /// </summary>
    /// <param name="getBalanceTypesForProcessResults">
    /// A callback function to process the retrieved payroll items. It takes a list of <see cref="PayrollItem"/> and a <see cref="CancellationToken"/> as parameters.
    /// </param>
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
    private async Task GetPayrollProcessResultsAsync(
        Func<IReadOnlyList<PayrollItem>, CancellationToken, ValueTask> getBalanceTypesForProcessResults,
        CancellationToken cancellationToken)
    {

        string url = await BuildUrl(cancellationToken: cancellationToken);
        while (true)
        {
            using var response = await GetOraclePayrollValue(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                break;
            }

            var results = await response.Content.ReadFromJsonAsync<PayrollRoot>(_jsonSerializerOptions, cancellationToken);
            if ((results?.Count ?? 0) == 0)
            {
                return;
            }

            await TrySyncMissingEmployees(results!.Items, cancellationToken);
            await getBalanceTypesForProcessResults(results!.Items, cancellationToken);


            if (!results.HasMore)
            {
                break;
            }

            // Construct the next URL for pagination
            string nextUrl = await BuildUrl(results.Count + results.Offset, cancellationToken: cancellationToken);
            if (string.IsNullOrEmpty(nextUrl))
            {
                break;
            }

            url = nextUrl;
        }
    }

    private async Task TrySyncMissingEmployees(IReadOnlyList<PayrollItem> items, CancellationToken cancellationToken)
    {
        var existsCollection = items.Select(d => d.PersonId).ToHashSet();
        var missingPersonIds = await _profitSharingDataContextFactory.UseReadOnlyContext(async c =>
        {
            // Query only the relevant PersonIds from the database
            var existingPersonIds = await c.Demographics
                .Where(d => existsCollection.Contains(d.OracleHcmId))
                .Select(d => d.OracleHcmId)
                .ToHashSetAsync(cancellationToken);

            // Find PersonIds that are in existsCollection but not in the database
            return existsCollection.Except(existingPersonIds).ToList();
        });

        foreach (long id in missingPersonIds)
        {
            var oracleHcmEmployees = _oracleEmployeeDataSyncClient.GetEmployee(id, cancellationToken);
            await _employeeSyncService.QueueEmployee("System", oracleHcmEmployees, cancellationToken);
        }
    }

    /// <summary>
    /// Processes the balance types for the given payroll process results asynchronously.
    /// </summary>
    /// <param name="items">
    /// A read-only list of <see cref="PayrollItem"/> objects representing the payroll process results to process.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method retrieves balance type totals for each payroll item by making HTTP requests to the Oracle HCM service.
    /// It calculates and updates the profit-sharing records based on the retrieved data.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown when an error occurs while making HTTP requests to fetch balance types.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled via the provided <paramref name="cancellationToken"/>.
    /// </exception>
    private async ValueTask GetBalanceTypesForProcessResultsAsync(IReadOnlyList<PayrollItem> items,
        CancellationToken cancellationToken)
    {
        // DimensionName should be set to Relationship No Calculation Breakdown Inception to Date. That will give you the correct value for current dollars, weeks, hours.
        const string dimensionName = "Relationship No Calculation Breakdown Inception to Date";

        // Initialize totals dictionary
        int year = DateTime.Today.Year;

        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 4), // 4 threads seems to be the max/sweet spot for OracleHCM
            CancellationToken = cancellationToken
        };
        await Parallel.ForEachAsync(items, parallelOptions, async (item, token) =>
        {
            var balanceTypeTotals = new Dictionary<long, decimal>
            {
                { BalanceTypeIds.MbProfitSharingDollars, 0 }, { BalanceTypeIds.MbProfitSharingHours, 0 }, { BalanceTypeIds.MbProfitSharingWeeks, 0 }
            };
            foreach (var balanceTypeId in _balanceTypeIds)
            {
                string url =
                    $"{_oracleHcmConfig.PayrollUrl}/{item.ObjectActionId}/child/BalanceView/?onlyData=true&fields=BalanceTypeId,TotalValue1,TotalValue2,DefbalId1,DimensionName&finder=findByBalVar;pBalGroupUsageId1=null,pBalGroupUsageId2=-1,pLDGId={PLDGId},pLC={PLC},pBalTypeId={balanceTypeId}";

                try
                {
                    using var response = await _httpClient.GetAsync(url, token);

                    if (response.IsSuccessStatusCode)
                    {
                        var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(_jsonSerializerOptions, token);

                        decimal total = balanceResults!.Items.Where(i => string.CompareOrdinal(i.DimensionName, dimensionName) == 0)
                            .Sum(b => b.TotalValue1);

                        // Accumulate totals per balance type ID
                        balanceTypeTotals[balanceTypeId] += total;

                    }
                    else
                    {
                        _logger.LogError("Failed to get balance types for PersonId {PersonId}/ObjectActionId {ObjectActionId}: {ResponseReasonPhrase}",
                            item.PersonId, item.ObjectActionId,
                            response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching balance types for PersonId {PersonId}/ObjectActionId {ObjectActionId}: {Message}", item.PersonId,
                        item.ObjectActionId, ex.Message);
                }
            }

            await CalculateAndUpdatePayProfitRecord(item.PersonId, year, balanceTypeTotals, cancellationToken);
        });
    }

    /// <summary>
    /// Calculates and updates the pay profit record for a specified individual based on the provided balance type totals.
    /// </summary>
    /// <param name="oracleHcmId">
    /// The unique identifier of the individual in the Oracle HCM system.
    /// </param>
    /// <param name="year">
    /// The year for which the pay profit record is being calculated and updated.
    /// </param>
    /// <param name="balanceTypeTotals">
    /// A dictionary containing balance type IDs and their corresponding total values.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    private Task CalculateAndUpdatePayProfitRecord(long oracleHcmId, int year, IDictionary<long, decimal> balanceTypeTotals,
        CancellationToken cancellationToken)
    {
        return _profitSharingDataContextFactory.UseWritableContext(async context =>
        {
            var demographic = await context.Demographics
                .Include(d => d.PayProfits.Where(p => p.ProfitYear == year))
                .Include(demographic => demographic.ContactInfo)
                .FirstOrDefaultAsync(d => d.OracleHcmId == oracleHcmId, cancellationToken);

            if (demographic == null)
            {
                return;
            }

            var payProfit = demographic.PayProfits.FirstOrDefault(p => p.ProfitYear == year);

            if (payProfit == null)
            {
                // Create new PayProfit if it doesn't exist
                payProfit = new PayProfit { ProfitYear = (short)year, DemographicId = demographic.Id, EarningsEtvaValue = 0, LastUpdate = DateTime.Now };
                demographic.PayProfits.Add(payProfit);
            }

            // Update PayProfit with accumulated totals
            foreach (var kvp in balanceTypeTotals)
            {
                long balanceTypeId = kvp.Key;
                decimal total = kvp.Value;

                switch (balanceTypeId)
                {
                    case BalanceTypeIds.MbProfitSharingDollars:
                        payProfit.CurrentIncomeYear = total;
                        break;
                    case BalanceTypeIds.MbProfitSharingHours:
                        payProfit.CurrentHoursYear = total;
                        break;
                    case BalanceTypeIds.MbProfitSharingWeeks:
                        payProfit.WeeksWorkedYear = (byte)total;
                        break;
                }
            }

            payProfit.LastUpdate = DateTime.Now;
            if (payProfit.CurrentHoursYear >= ReferenceData.MinimumHoursForContribution())
            {
                payProfit.YearsInPlan = (byte)await context.PayProfits.Where(pp => pp.DemographicId == payProfit.DemographicId)
                    .MaxAsync(pp => pp.YearsInPlan + 1, cancellationToken: cancellationToken);
            }

            int resultCount = await context.SaveChangesAsync(cancellationToken);

            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = resultCount == 0 ? ConsoleColor.Red : ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.WriteLine($"Upserted {resultCount} rows into {nameof(PayProfit)} for {demographic.ContactInfo.FullName}({demographic.EmployeeId})");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }

        }, cancellationToken);
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
        ushort limit = ushort.Min(50, _oracleHcmConfig.Limit);
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
        UriBuilder initialUriBuilder = new UriBuilder(url) { Query = await new FormUrlEncodedContent(initialQuery).ReadAsStringAsync(cancellationToken) };
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
}
