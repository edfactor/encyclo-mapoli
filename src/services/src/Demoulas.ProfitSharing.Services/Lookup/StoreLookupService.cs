using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

/// <summary>
/// Real implementation of store lookup service using Demoulas.Common.Data.Services.
/// Provides access to store data from the enterprise warehouse database.
/// </summary>
public sealed class StoreLookupService : IStoreLookupService
{
    private readonly IStoreService _commonStoreService;
    private readonly IDemoulasCommonWarehouseContext _warehouseContext;

    public StoreLookupService(
        IStoreService commonStoreService,
        IDemoulasCommonWarehouseContext warehouseContext)
    {
        _commonStoreService = commonStoreService;
        _warehouseContext = warehouseContext;
    }

    public async Task<List<StoreListResponse>> GetStoresAsync(
        StoreListRequest request, 
        CancellationToken cancellationToken)
    {
        // Build base query based on status filter
        IQueryable<StoreInformation> query = request.Status switch
        {
            StoreStatusFilter.Unopened => await BuildUnopenedStoresQuery(cancellationToken),
            StoreStatusFilter.Active => _commonStoreService.GetActiveStoresQuery(_warehouseContext),
            _ => _commonStoreService.GetAllStoresQuery(_warehouseContext)
        };

        // Apply retail filter if requested
        if (request.StoreType == StoreTypeFilter.Retail)
        {
            query = query.Where(s => s.StoreId < 899);
        }

        // Execute query and get stores
        var stores = await query
            .OrderBy(s => s.StoreId)
            .ToListAsync(cancellationToken);

        // Map to response DTOs
        return stores.Select(store => MapToStoreListResponse(store))
            .ToList();
    }

    public async Task<StoreListResponse?> GetStoreByIdAsync(
        int storeId, 
        CancellationToken cancellationToken)
    {
        // Get max batch date
        var maxBatchDate = await _commonStoreService.GetMaxBatchDateAsync(
            _warehouseContext, 
            cancellationToken: cancellationToken) ?? DateTime.UtcNow;

        // Get single store
        var store = await _commonStoreService
            .GetStoreLocationQuery(_warehouseContext, (short)storeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (store == null)
        {
            return null;
        }

        // Get departments for this store
        var departments = await _commonStoreService
            .GetStoreDepartmentsQuery(_warehouseContext, new[] { (short)storeId })
            .Select(d => d.DepartmentId.ToString())
            .ToListAsync(cancellationToken);

        var departmentsByStore = new Dictionary<short, List<string>>
        {
            { (short)storeId, departments }
        };

        return MapToStoreListResponse(store);
    }

    private async Task<IQueryable<StoreInformation>> BuildUnopenedStoresQuery(
        CancellationToken cancellationToken)
    {
        // Get list of unopened store IDs
        var unopenedStoreIds = await _commonStoreService
            .GetUnopenedStoresQuery(_warehouseContext)
            .ToListAsync(cancellationToken);

        // Return query for these stores (or empty if none)
        return unopenedStoreIds.Any()
            ? _commonStoreService.GetStoreLocationsByNumbersQuery(_warehouseContext, unopenedStoreIds)
            : _warehouseContext.Stores.Where(s => false); // Empty query
    }

    private static StoreListResponse MapToStoreListResponse(
        StoreInformation store)
    {
        return new StoreListResponse
        {
            StoreId = store.StoreId,
            DisplayName = $"{store.StoreId} - {store.StoreName}",
            City = store.City,
            State = store.State,
            ZipCode = store.ZipCode,
            HasSpirits = store.HasSpirits ?? false,
            HasBakery = store.HasBakery ?? false,
            HasKitchen = store.HasKitchen ?? false,
            HasCafe = store.HasCafe ?? false
        };
    }
}
