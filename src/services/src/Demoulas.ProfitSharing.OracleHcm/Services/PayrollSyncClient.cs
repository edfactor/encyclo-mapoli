using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly.Timeout;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

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
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<PayrollSyncClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PayrollSyncClient(HttpClient httpClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger<PayrollSyncClient> logger)
    {
        _httpClient = httpClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _oracleHcmConfig = oracleHcmConfig;
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
            HashSet<long> list = await _profitSharingDataContextFactory.UseReadOnlyContext(c =>
                c.Demographics
                    .Select(d => d.OracleHcmId)
                    .ToHashSetAsync(cancellationToken));

            // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
            await GetPayrollProcessResultsAsync(list, GetBalanceTypesForProcessResultsAsync, cancellationToken);
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
    /// Retrieves payroll process results for a set of person IDs and processes the results using a specified callback function.
    /// </summary>
    /// <param name="personIds">
    /// A set of person IDs for which payroll process results will be retrieved.
    /// </param>
    /// <param name="getBalanceTypesForProcessResults">
    /// A callback function to process the balance types for the retrieved payroll process results.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of retrieving and processing payroll process results.
    /// </returns>
    /// <remarks>
    /// This method uses parallel processing to retrieve payroll process results for each person ID.
    /// If the retrieval is successful, the specified callback function is invoked to process the results.
    /// </remarks>
    private Task GetPayrollProcessResultsAsync(
        ISet<long> personIds,
        Func<long, HashSet<int>, CancellationToken, ValueTask> getBalanceTypesForProcessResults,
        CancellationToken cancellationToken)
    {

        const int payrollActionId = 2003;

        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 6),
            CancellationToken = cancellationToken
        };

        return Parallel.ForEachAsync(source: personIds, parallelOptions: parallelOptions, body: async (personId, token) =>
        {
            var objectActionIds = new HashSet<int>();
            bool isSuccessful = false;

            try
            {
                string query = $"{_oracleHcmConfig.PayrollUrl}?q=PersonId={personId}&fields=PayrollActionId,ObjectActionId&onlyData=true";
                using var response = await _httpClient.GetAsync(query, token);

                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadFromJsonAsync<PayrollRoot>(_jsonSerializerOptions, token);
                    if ((results?.Count ?? 0) == 0)
                    {
                        return;
                    }

                    objectActionIds = results!.Items
                        .Where(result => result is { PayrollActionId: payrollActionId, ObjectActionId: not null })
                        .Select(result => result.ObjectActionId!.Value)
                        .ToHashSet();

                    isSuccessful = objectActionIds.Any();
                }
                else
                {
                    _logger.LogError("Failed to get payroll process results for PersonId {PersonId}: {ResponseReasonPhrase}", personId, response.ReasonPhrase);
                }
            }
            catch (TimeoutRejectedException e)
            {
                _logger.LogError(e, e.Message);
            }

            if (isSuccessful)
            {
                // Wrap Task in ValueTask
                await getBalanceTypesForProcessResults(personId, objectActionIds, token);
            }
        });
    }


    /// <summary>
    /// Retrieves balance types for the specified payroll process results asynchronously.
    /// </summary>
    /// <param name="oracleHcmId">
    /// The unique identifier for the Oracle HCM entity.
    /// </param>
    /// <param name="objectActionIds">
    /// A collection of action IDs associated with payroll process results.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method processes payroll balances by fetching data from an external Oracle HCM service
    /// and updates the database with the accumulated totals for the specified balance types.
    /// </remarks>
    private async ValueTask GetBalanceTypesForProcessResultsAsync(
        long oracleHcmId,
        HashSet<int> objectActionIds,
        CancellationToken cancellationToken)
    {
        // DimensionName should be set to Relationship No Calculation Breakdown Inception to Date. That will give you the correct value for current dollars, weeks, hours.
        const string dimensionName = "Relationship No Calculation Breakdown Inception to Date";

        // Initialize totals dictionary
        var balanceTypeTotals = new ConcurrentDictionary<long, decimal>();

        int year = DateTime.Today.Year;

        foreach (int objectActionId in objectActionIds)
        {
            await Task.WhenAll(_balanceTypeIds.Select(async balanceTypeId =>
            {
                string url =
                    $"{_oracleHcmConfig.PayrollUrl}/{objectActionId}/child/BalanceView/?onlyData=true&fields=BalanceTypeId,TotalValue1,TotalValue2,DefbalId1,DimensionName&finder=findByBalVar;pBalGroupUsageId1=null,pBalGroupUsageId2=-1,pLDGId={PLDGId},pLC={PLC},pBalTypeId={balanceTypeId}&onlyData=true";

                try
                {
                    using var response = await _httpClient.GetAsync(url, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(_jsonSerializerOptions, cancellationToken);

                        decimal total = balanceResults!.Items.Where(i => string.CompareOrdinal(i.DimensionName, dimensionName) == 0)
                            .Sum(b => b.TotalValue1);

                        // Accumulate totals per balance type ID
                        balanceTypeTotals.AddOrUpdate(balanceTypeId, total, (key, oldValue) => oldValue + total);

                    }
                    else
                    {
                        _logger.LogError("Failed to get balance types for ObjectActionId {ObjectActionId}: {ResponseReasonPhrase}", objectActionId,
                            response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching balance types for ObjectActionId {ObjectActionId}: {Message}", objectActionId, ex.Message);
                }
            }));
        }

        // Single database interaction
        await _profitSharingDataContextFactory.UseWritableContext(async context =>
        {
            var demographic = await context.Demographics
                .Include(d => d.PayProfits.Where(p => p.ProfitYear == year))
                .Include(demographic => demographic.ContactInfo)
                .FirstOrDefaultAsync(d => d.OracleHcmId == oracleHcmId, cancellationToken);

            if (demographic == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Demographic with OracleHcmId {oracleHcmId} not found.");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            var payProfit = demographic.PayProfits.FirstOrDefault(p=> p.ProfitYear == year);

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
                payProfit.YearsInPlan = (byte)await context.PayProfits.Where(pp => pp.DemographicId == payProfit.DemographicId).MaxAsync(pp => pp.YearsInPlan + 1 , cancellationToken: cancellationToken);
            }

            int resultCount = await context.SaveChangesAsync(cancellationToken);

            Console.ForegroundColor = resultCount == 0 ? ConsoleColor.Red : ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine($"Inserted {resultCount} rows into {nameof(PayProfit)} for {demographic.ContactInfo.FullName}({demographic.EmployeeId})");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

        }, cancellationToken);
    }
}
