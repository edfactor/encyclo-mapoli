using System.Diagnostics;
using System.Net.Http.Json;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.Util.Extensions;
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


    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<PayrollSyncService> _logger;

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
    /// <param name="item">
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
    public async Task GetBalanceTypesForProcessResultsAsync(PayrollItem item,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        int year = DateTime.Today.Year;

        // Initialize totals dictionary for the specific Profit Sharing balance types we care about
        Dictionary<long, decimal> balanceTypeTotals = new()
        {
            { BalanceTypeIds.MbProfitSharingDollars, 0 },
            { BalanceTypeIds.MbProfitSharingHours, 0 },
            { BalanceTypeIds.MbProfitSharingWeeks, 0 }
        };

        // New single endpoint call already returns all relevant balance types
        string url = $"{_oracleHcmConfig.PayrollUrl}/{item.ObjectActionId}/child/BalanceView/?onlyData=true&finder=findByBalVar;pBalGroupUsageId1=300007039791698";
        try
        {
            await Task.Delay(new TimeSpan(0, 0, 1), cancellationToken).ConfigureAwait(false);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string errorResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("Oracle HCM API request failed: {ErrorResponse} / {ReasonPhrase}", errorResponse, response.ReasonPhrase);

                if (Debugger.IsAttached)
                {
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

                _logger.LogError("Failed to get balance types for PersonId {PersonId}/ObjectActionId {ObjectActionId}: {ResponseReasonPhrase}",
                    item.PersonId,
                    item.ObjectActionId,
                    SanitizeInput(response.ReasonPhrase));

                EndpointTelemetry.EndpointErrorsTotal.Add(1,
                    new("error.type", "HttpError"),
                    new("operation", "payroll-sync-balance-fetch"),
                    new("service", nameof(PayrollSyncService)));
            }
            else
            {
                BalanceRoot? balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(cancellationToken).ConfigureAwait(false);
                if ((balanceResults?.Items?.Any() ?? false))
                {
                    foreach (BalanceItem balanceItem in balanceResults.Items
                                 .Where(b => balanceTypeTotals.ContainsKey(b.BalanceTypeId)))
                    {
                        balanceTypeTotals[balanceItem.BalanceTypeId] = balanceItem.TotalValue1;
                    }
                }

                EndpointTelemetry.RecordCountsProcessed.Record(balanceResults?.Items?.Count ?? 0,
                    new("operation", "payroll-sync-balance-fetch"),
                    new("record.type", "balance-items"),
                    new("service", nameof(PayrollSyncService)));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching balance types for PersonId {PersonId}/ObjectActionId {ObjectActionId}: {Message}",
                item.PersonId,
                item.ObjectActionId,
                ex.Message);

            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new("error.type", ex.GetType().Name),
                new("operation", "payroll-sync-balance-fetch"),
                new("service", nameof(PayrollSyncService)));
        }
        finally
        {
            stopwatch.Stop();
            EndpointTelemetry.BusinessLogicDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds,
                new("operation", "payroll-sync-balance-fetch"),
                new("service", nameof(PayrollSyncService)));
        }

        if (balanceTypeTotals.Values.All(v => v == 0))
        {
            return;
        }

        await CalculateAndUpdatePayProfitRecord(item.PersonId, year, balanceTypeTotals, cancellationToken).ConfigureAwait(false);

        // Record successful balance processing
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "payroll-sync-balance-processed"),
            new("status", "success"),
            new("service", nameof(PayrollSyncService)));
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
            Demographic? demographic = await context.Demographics
                .TagWith($"PayrollSync-GetDemographic-OracleHcmId:{oracleHcmId}-Year:{year}")
                .Include(d => d.PayProfits.Where(p => p.ProfitYear >= year))
                .Include(demographic => demographic.ContactInfo)
                .FirstOrDefaultAsync(d => d.OracleHcmId == oracleHcmId, cancellationToken).ConfigureAwait(false);

            if (demographic == null)
            {
                return;
            }

            PayProfit? payProfit = demographic.PayProfits.FirstOrDefault(p => p.ProfitYear == year);

            if (payProfit == null)
            {
                // Create new PayProfit if it doesn't exist
                // Question? shouldn't this be cloning all columns from the prior year (if it exists) to the new year?
                payProfit = new PayProfit { ProfitYear = (short)year, DemographicId = demographic.Id, ModifiedAtUtc = DateTimeOffset.UtcNow, Etva = 0 };
                demographic.PayProfits.Add(payProfit);
            }

            // Update PayProfit with accumulated totals
            foreach (KeyValuePair<long, decimal> kvp in balanceTypeTotals)
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

            payProfit.ModifiedAtUtc = DateTimeOffset.UtcNow;

            int resultCount = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = resultCount == 0 ? ConsoleColor.Red : ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.WriteLine($"Upserted {resultCount} rows into {nameof(PayProfit)} for {demographic.ContactInfo.FullName}({demographic.BadgeNumber})");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }

        }, cancellationToken);
    }
    private static string? SanitizeInput(string? input)
    {
        return input?.Replace("\r", string.Empty).Replace("\n", string.Empty);
    }
}
