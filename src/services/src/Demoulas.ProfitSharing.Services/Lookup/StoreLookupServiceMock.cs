using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Services.Lookup;

/// <summary>
/// Mock implementation of store lookup service for development.
/// NOTE: Replace with StoreLookupServiceReal once Common.Data.Services is configured.
/// </summary>
public sealed class StoreLookupServiceMock : IStoreLookupService
{
    public Task<List<StoreListResponse>> GetStoresAsync(StoreListRequest request, CancellationToken cancellationToken)
    {
        var allStores = GetMockStores();

        // Apply status filter
        var filteredStores = request.Status switch
        {
            StoreStatusFilter.Active => allStores.Where(s => s.StoreId <= 140),
            StoreStatusFilter.Unopened => allStores.Where(s => s.StoreId > 900),
            _ => allStores
        };

        // Apply type filter
        if (request.StoreType == StoreTypeFilter.Retail)
        {
            filteredStores = filteredStores.Where(s => s.StoreId < 899);
        }

        return Task.FromResult(filteredStores.OrderBy(s => s.StoreId).ToList());
    }

    public Task<StoreListResponse?> GetStoreByIdAsync(int storeId, CancellationToken cancellationToken)
    {
        var store = GetMockStores().FirstOrDefault(s => s.StoreId == storeId);
        return Task.FromResult(store);
    }

    private static List<StoreListResponse> GetMockStores()
    {
        return new List<StoreListResponse>
        {
            new()
            {
                StoreId = 1,
                DisplayName = "1 - FLETCHER",
                City = "Chelmsford",
                State = "MA",
                ZipCode = "01824",
                HasSpirits = false
            },
            new()
            {
                StoreId = 2,
                DisplayName = "2 - BRIDGE ST.",
                City = "Salem",
                State = "MA",
                ZipCode = "01970",
                HasSpirits = false
            },
            new()
            {
                StoreId = 10,
                DisplayName = "10 - PLAISTOW",
                City = "Plaistow",
                State = "NH",
                ZipCode = "03865",
                HasSpirits = true
            }
        };
    }
}
