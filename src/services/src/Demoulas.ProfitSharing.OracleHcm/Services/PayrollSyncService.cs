using System.Diagnostics;
using System.Net.Http.Json;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;
internal class PayrollSyncService
{
    public static class BalanceTypeIds
    {
        public const long MbProfitSharingDollars = 300000789345470; // MB Profit Sharing Dollars
        public const long MbProfitSharingHours = 300000789345477; // MB Profit Sharing Hours
        public const long MbProfitSharingWeeks = 300000785152356; // MB Profit Sharing Weeks
    }

    private readonly List<long> _balanceTypeIds =
        [BalanceTypeIds.MbProfitSharingDollars, BalanceTypeIds.MbProfitSharingHours, BalanceTypeIds.MbProfitSharingWeeks];

    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<PayrollSyncService> _logger;

    // Constants for other parameters
    private const string PLDGId = "300000005030436";
    private const string PLC = "US";

    public PayrollSyncService(HttpClient httpClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger<PayrollSyncService> logger)
    {
        _httpClient = httpClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _oracleHcmConfig = oracleHcmConfig;
        _logger = logger;
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
    public async ValueTask GetBalanceTypesForProcessResultsAsync(IReadOnlyList<PayrollItem> items,
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
                        var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(token);

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
}
