using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Polly.Timeout;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

public class PayrollSyncClient
{

    // Define constants for balance type IDs
    private static class BalanceTypeIds
    {
        public const long MbProfitSharingDollars = 300000789345470; // MB Profit Sharing Dollars
        public const long MbProfitSharingHours = 300000789345477; // MB Profit Sharing Hours
        public const long MbProfitSharingWeeks = 300000785152356; // MB Profit Sharing Weeks
    }

    // Constants for other parameters
    private const string PLDGId = "300000005030436";
    private const string PLC = "US";

    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public PayrollSyncClient(HttpClient httpClient, IProfitSharingDataContextFactory contextFactory)
    {
        _httpClient = httpClient;
        _contextFactory = contextFactory;
    }

    // Method to get payroll process results for a list of person IDs
    public async IAsyncEnumerable<KeyValuePair<long, List<int>>> GetPayrollProcessResultsAsync(
        List<long> personIds, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var personId in personIds)
        {
            List<int> objectActionIds = new List<int>();
            bool isSuccessful = false;

            try
            {
                var response = await _httpClient.GetAsync(
                    $"?q=PersonId={personId}&fields=PayrollActionId,ObjectActionId&onlyData=true",
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadFromJsonAsync<Root>(cancellationToken);

                    objectActionIds = results!.Items
                        .Where(result => result is { PayrollActionId: 2003, ObjectActionId: not null })
                        .Select(result => result.ObjectActionId!.Value)
                        .ToList();

                    isSuccessful = true;
                }
                else
                {
                    Console.WriteLine($"Failed to get payroll process results for PersonId {personId}: {response.ReasonPhrase}");
                }
            }
            catch (TimeoutRejectedException e)
            {
                var startingColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = startingColor;
            }

            if (isSuccessful)
            {
                yield return new KeyValuePair<long, List<int>>(personId, objectActionIds);
            }
        }
    }



    // Method to get balance types for each ObjectActionId
    public async Task GetBalanceTypesForProcessResultsAsync(
        long oracleHcmId,
        List<int> objectActionIds,
        CancellationToken cancellationToken)
    {



        var balanceTypeIds = new List<long> { BalanceTypeIds.MbProfitSharingDollars, BalanceTypeIds.MbProfitSharingHours, BalanceTypeIds.MbProfitSharingWeeks };

        // Initialize totals dictionary
        var balanceTypeTotals = new Dictionary<long, decimal>
        {
            { BalanceTypeIds.MbProfitSharingDollars, 0 }, { BalanceTypeIds.MbProfitSharingHours, 0 }, { BalanceTypeIds.MbProfitSharingWeeks, 0 }
        };

        int year = DateTime.Today.Year;

        foreach (var objectActionId in objectActionIds)
        {
            foreach (var balanceTypeId in balanceTypeIds)
            {
                var url = $"personProcessResults/{objectActionId}/child/BalanceView/" +
                          $"?onlyData=true&fields=BalanceTypeId,TotalValue1,TotalValue2,DefbalId1,DimensionName" +
                          $"&finder=findByBalVar;pBalGroupUsageId1=null,pBalGroupUsageId2=-1," +
                          $"pLDGId={PLDGId},pLC={PLC},pBalTypeId={balanceTypeId}&onlyData=true";

                try
                {
                    var response = await _httpClient.GetAsync(url, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(cancellationToken);

                        var total = balanceResults!.Items
                            .Sum(b => b.TotalValue1 + b.TotalValue2);

                        // Accumulate totals per balance type ID
                        balanceTypeTotals[balanceTypeId] += total;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get balance types for ObjectActionId {objectActionId}: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching balance types for ObjectActionId {objectActionId}: {ex.Message}");
                }
            }
        }

        // Single database interaction
        await _contextFactory.UseWritableContext(async context =>
        {
            var demographic = await context.Demographics
                .Include(d => d.PayProfits.Where(p => p.ProfitYear == year))
                .FirstOrDefaultAsync(d => d.OracleHcmId == oracleHcmId, cancellationToken);

            if (demographic == null)
            {
                Console.WriteLine($"Demographic with OracleHcmId {oracleHcmId} not found.");
                return;
            }

            var payProfit = demographic.PayProfits.FirstOrDefault();

            if (payProfit == null)
            {
                // Create new PayProfit if it doesn't exist
                payProfit = new PayProfit { ProfitYear = (short)year, DemographicId = demographic.Id, EarningsEtvaValue = 0, LastUpdate = DateTime.Now };
                demographic.PayProfits.Add(payProfit);
            }

            // Update PayProfit with accumulated totals
            foreach (var kvp in balanceTypeTotals)
            {
                var balanceTypeId = kvp.Key;
                var total = kvp.Value;

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

            await context.SaveChangesAsync(cancellationToken);

        }, cancellationToken);
    }

    public async Task RetrievePayrollBalancesAsync(CancellationToken cancellationToken)
    {
        List<long> list = await _contextFactory.UseReadOnlyContext(c =>
            c.Demographics
                .Select(d => d.OracleHcmId)
                .ToListAsync(cancellationToken));

        // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
        await foreach (KeyValuePair<long, List<int>> emp in GetPayrollProcessResultsAsync(list, cancellationToken))
        {
            // Step 2: Get specific balance types for each ObjectActionId
            await GetBalanceTypesForProcessResultsAsync(emp.Key, emp.Value, cancellationToken);
        }
    }
}



public record Item(
    [property: JsonPropertyName("PayrollActionId")] int? PayrollActionId,
    [property: JsonPropertyName("ObjectActionId")] int? ObjectActionId
);

public record Link(
    [property: JsonPropertyName("rel")] string Rel,
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("kind")] string Kind
);

public record Root(
    [property: JsonPropertyName("items")] IReadOnlyList<Item> Items,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);


public record BalanceItem(
    [property: JsonPropertyName("BalanceTypeId")] long BalanceTypeId,
    [property: JsonPropertyName("TotalValue1")] decimal TotalValue1,
    [property: JsonPropertyName("TotalValue2")] decimal TotalValue2,
    [property: JsonPropertyName("DefbalId1")] long DefbalId1,
    [property: JsonPropertyName("DimensionName")] string DimensionName
);

public record BalanceLink(
    [property: JsonPropertyName("rel")] string Rel,
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("kind")] string Kind
);

public record BalanceRoot(
    [property: JsonPropertyName("items")] IReadOnlyList<BalanceItem> Items,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset,
    [property: JsonPropertyName("links")] IReadOnlyList<BalanceLink> Links
);


