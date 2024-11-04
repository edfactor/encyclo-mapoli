namespace Demoulas.ProfitSharing.OracleHcm.Services;

public class PayrollSyncClient
{
    private readonly HttpClient _httpClient;

    public PayrollSyncClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to get payroll process results for a list of person IDs
    public async Task<List<string>> GetPayrollProcessResultsAsync(List<long> personIds)
    {
        var objectActionIds = new List<string>();

        foreach (var personId in personIds)
        {
            var response = await _httpClient.GetAsync($"?q=PersonId={personId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var results = await System.Text.Json.JsonSerializer.DeserializeAsync<dynamic>(content);

                // Assuming results is an array of process results
                foreach (var result in results!.items)
                {
                    objectActionIds.Add((string)result.ObjectActionId);
                }
            }
            else
            {
                Console.WriteLine($"Failed to get payroll process results for PersonId {personId}: {response.ReasonPhrase}");
            }
        }

        return objectActionIds;
    }

    // Method to get balance types for each ObjectActionId
    public async Task GetBalanceTypesForProcessResultsAsync(List<string> objectActionIds)
    {
        var balanceTypeIds = new List<string>
        {
            "300000789345470", // MB Profit Sharing Dollars
            "300000789345477", // MB Profit Sharing Hours
            "300000785152356"  // MB Profit Sharing Weeks
        };

        foreach (var objectActionId in objectActionIds)
        {
            var response = await _httpClient.GetAsync($"/{objectActionId}/child/PersonProcessResultsBalanceType");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var balanceResults = await System.Text.Json.JsonSerializer.DeserializeAsync<dynamic>(content);

                foreach (var balance in balanceResults!.items)
                {
                    string balanceTypeId = balance.BalanceTypeId;
                    if (balanceTypeIds.Contains(balanceTypeId))
                    {
                        Console.WriteLine($"ObjectActionId: {objectActionId}, BalanceTypeId: {balanceTypeId}, Balance Amount: {balance.BalanceAmount}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to get balance types for ObjectActionId {objectActionId}: {response.ReasonPhrase}");
            }
        }
    }

    public async Task RetrievePayrollBalancesAsync(List<long> personIds)
    {
        // Step 1: Get payroll process results (ObjectActionIds) for each PersonId
        var objectActionIds = await GetPayrollProcessResultsAsync(personIds);

        // Step 2: Get specific balance types for each ObjectActionId
        await GetBalanceTypesForProcessResultsAsync(objectActionIds);
    }
}
