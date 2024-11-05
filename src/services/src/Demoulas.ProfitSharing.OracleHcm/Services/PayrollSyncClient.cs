using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Polly.Timeout;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

public class PayrollSyncClient
{
    private readonly HttpClient _httpClient;
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public PayrollSyncClient(HttpClient httpClient, IProfitSharingDataContextFactory contextFactory)
    {
        _httpClient = httpClient;
        _contextFactory = contextFactory;
    }

    // Method to get payroll process results for a list of person IDs
    public async Task<Dictionary<long, List<int>>> GetPayrollProcessResultsAsync(List<long> personIds, CancellationToken cancellationToken)
    {
        var objectActionIds = new Dictionary<long, List<int>>();

        foreach (var personId in personIds)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?q=PersonId={personId}&fields=PayrollActionId,ObjectActionId&onlyData=true", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadFromJsonAsync<Root>(cancellationToken);

                    // Assuming results is an array of process results
                    objectActionIds.TryAdd(personId, new List<int>());
                    objectActionIds[personId].AddRange(results!.Items.Where(result => result.PayrollActionId == 2003)
                        .Select(result => result.ObjectActionId!.Value));
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
        }

        return objectActionIds;
    }

    // Method to get balance types for each ObjectActionId
    public async Task GetBalanceTypesForProcessResultsAsync(Dictionary<long, List<int>> objectActionIds, CancellationToken cancellationToken)
    {
        var balanceTypeIds = new List<long>
        {
            300000789345470, // MB Profit Sharing Dollars
            300000789345477, // MB Profit Sharing Hours
            300000785152356 // MB Profit Sharing Weeks
        };

        int year = DateTime.Today.Year;
        foreach (var employee in objectActionIds)
        {
            foreach (var objectActionId in employee.Value)
            {
                foreach (var balanceTypeId in balanceTypeIds)
                {
                    var response = await _httpClient.GetAsync(
                        $"personProcessResults/{objectActionId}/child/BalanceView/?onlyData=true&fields=BalanceTypeId,TotalValue1,TotalValue2,DefbalId1,DimensionName&finder=findByBalVar;pBalGroupUsageId1=null,pBalGroupUsageId2=-1,pLDGId=300000005030436,pLC=US,pBalTypeId={balanceTypeId}&onlyData=true",
                        cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(cancellationToken);

                        var total = balanceResults!.Items.Where(i => balanceTypeIds.Contains(i.BalanceTypeId)).Sum(b => b.TotalValue1 + b.TotalValue2);

                        await _contextFactory.UseWritableContext(async context =>
                        {
                            var d = await context.Demographics
                                .Include(d => d.PayProfits.Where(p => p.ProfitYear == year))
                                .Where(d => d.OracleHcmId == employee.Key)
                                .FirstAsync(cancellationToken: cancellationToken);


                            var pp = d.PayProfits.FirstOrDefault();
                            if (pp != null)
                            {
                                switch (balanceTypeId)
                                {
                                    case 300000789345470:
                                        pp.CurrentIncomeYear = total;
                                        break;
                                    case 300000789345477:
                                        pp.CurrentHoursYear = total;
                                        break;
                                    case 300000785152356:
                                        pp.WeeksWorkedYear = (byte)total;
                                        break;
                                }
                                d.PayProfits[0] = pp;
                            }
                            else
                            {
                               var newPayProfit =  new PayProfit { ProfitYear = (short)year, DemographicId = d.Id, EarningsEtvaValue = 0};

                                switch (balanceTypeId)
                                {
                                    case 300000789345470:
                                        newPayProfit.CurrentIncomeYear = total;
                                        break;
                                    case 300000789345477:
                                        newPayProfit.CurrentHoursYear = total;
                                        break;
                                    case 300000785152356:
                                        newPayProfit.WeeksWorkedYear = (byte)total;
                                        break;
                                }

                                d.PayProfits.Add(newPayProfit);
                            }

                            await context.SaveChangesAsync(cancellationToken);

                        }, cancellationToken);

                    }
                    else
                    {
                        Console.WriteLine($"Failed to get balance types for ObjectActionId {objectActionId}: {response.ReasonPhrase}");
                    }
                }
            }
        }
    }

    public async Task RetrievePayrollBalancesAsync(CancellationToken cancellationToken)
    {
        List<long> list = await _contextFactory.UseReadOnlyContext(c => c.Demographics.Select(d => d.OracleHcmId).ToListAsync(cancellationToken));

        // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
        Dictionary<long, List<int>> objectActionIds = await GetPayrollProcessResultsAsync(list, cancellationToken);

        // Step 2: Get specific balance types for each ObjectActionId
        await GetBalanceTypesForProcessResultsAsync(objectActionIds, cancellationToken);
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


