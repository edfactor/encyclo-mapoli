using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

public class PayrollSyncClient
{
    private readonly HttpClient _httpClient;

    public PayrollSyncClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to get payroll process results for a list of person IDs
    public async Task<List<int>> GetPayrollProcessResultsAsync(List<long> personIds, CancellationToken cancellationToken)
    {
        var objectActionIds = new List<int>();

        foreach (var personId in personIds)
        {
            var response = await _httpClient.GetAsync($"?q=PersonId={personId}&fields=PayrollActionId,ObjectActionId&onlyData=true", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadFromJsonAsync<Root>(cancellationToken);

                // Assuming results is an array of process results
                objectActionIds.AddRange(results!.Items.Where(result => result.PayrollActionId == 2003)
                    .Select(result => result.ObjectActionId!.Value));
            }
            else
            {
                Console.WriteLine($"Failed to get payroll process results for PersonId {personId}: {response.ReasonPhrase}");
            }
        }

        return objectActionIds;
    }

    // Method to get balance types for each ObjectActionId
    public async Task GetBalanceTypesForProcessResultsAsync(List<int> objectActionIds, CancellationToken cancellationToken)
    {
        var balanceTypeIds = new List<long>
        {
            300000789345470, // MB Profit Sharing Dollars
            300000789345477, // MB Profit Sharing Hours
            300000785152356 // MB Profit Sharing Weeks
        };

        foreach (var objectActionId in objectActionIds)
        {
            foreach (var balanceTypeId in balanceTypeIds)
            {
                var response = await _httpClient.GetAsync(
                    $"personProcessResults/{objectActionId}/child/BalanceView/?onlyData=true&fields=BalanceTypeId,TotalValue1,TotalValue2,DefbalId1,DimensionName&finder=findByBalVar;pBalGroupUsageId1=null,pBalGroupUsageId2=-1,pLDGId=300000005030436,pLC=US,pBalTypeId={balanceTypeId}&onlyData=true",
                    cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var balanceResults = await response.Content.ReadFromJsonAsync<BalanceRoot>(cancellationToken);

#pragma warning disable S3267
                    foreach (var balance in balanceResults!.Items.Where(i => balanceTypeIds.Contains(i.BalanceTypeId)))
#pragma warning restore S3267
                    {

                        Console.WriteLine($"ObjectActionId: {objectActionId}, BalanceTypeId: {balance.BalanceTypeId}, Balance Amount: {balance.TotalValue1}, DefbalId1 : {balance.DefbalId1}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to get balance types for ObjectActionId {objectActionId}: {response.ReasonPhrase}");
                }
            }
        }
    }

    public async Task RetrievePayrollBalancesAsync(List<long> personIds, CancellationToken cancellationToken)
    {
        // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
        var objectActionIds = await GetPayrollProcessResultsAsync(personIds, cancellationToken);

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


