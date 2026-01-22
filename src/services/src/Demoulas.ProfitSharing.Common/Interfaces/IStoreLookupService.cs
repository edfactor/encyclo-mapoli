using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for store lookup operations using the Common.Data.Services IStoreService.
/// Provides simplified access to store data with filtering capabilities.
/// </summary>
public interface IStoreLookupService
{
    /// <summary>
    /// Gets a list of stores with optional filtering by status and type.
    /// </summary>
    Task<List<StoreResponse>> GetStoresAsync(StoreListRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single store by ID.
    /// </summary>
    Task<StoreResponse?> GetStoreByIdAsync(int storeId, CancellationToken cancellationToken);
}
