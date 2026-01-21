using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

/// <summary>
/// Real implementation of store lookup service using Demoulas.Common.Data.Services.
/// Provides access to store data from the enterprise warehouse database.
/// </summary>
public sealed class StoreLookupService : IStoreLookupService
{
    private readonly IStoreService _commonStoreService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public StoreLookupService(
        IStoreService commonStoreService,
        IProfitSharingDataContextFactory dataContextFactory)
    {
        _commonStoreService = commonStoreService;
        _dataContextFactory = dataContextFactory;
    }

    public Task<List<StoreListResponse>> GetStoresAsync(
        StoreListRequest request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWarehouseContext(async ctx =>
        {
            // Build base query based on status filter
            IQueryable<StoreInformation> query = request.Status switch
            {
                StoreStatusFilter.Unopened => await BuildUnopenedStoresQuery(ctx, cancellationToken),
                StoreStatusFilter.Active => _commonStoreService.GetActiveStoresQuery(ctx),
                _ => _commonStoreService.GetAllStoresQuery(ctx)
            };

            // Apply retail filter if requested
            if (request.StoreType == StoreTypeFilter.Retail)
            {
                query = query.Where(s => s.StoreId < 899);
            }

            // Apply virtual stores filter
            if (!request.IncludeVirtualStores)
            {
                // Virtual stores are typically in the 900+ range
                query = query.Where(s => s.StoreId < 900);
            }

            // Execute query and get stores
            var stores = await query
                .OrderBy(s => s.StoreId)
                .ToListAsync(cancellationToken);

            // Map to response DTOs
            return stores.Select(store => MapToStoreListResponse(store))
                .ToList();
        });
    }

    public Task<StoreListResponse?> GetStoreByIdAsync(
        int storeId,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWarehouseContext(async ctx =>
        {
            // Get single store
            var store = await _commonStoreService
                .GetStoreLocationQuery(ctx, (short)storeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (store == null)
            {
                return null;
            }

            return MapToStoreListResponse(store);
        });
    }

    private async Task<IQueryable<StoreInformation>> BuildUnopenedStoresQuery(
        IDemoulasCommonWarehouseContext ctx,
        CancellationToken cancellationToken)
    {
        // Get list of unopened store IDs
        var unopenedStoreIds = await _commonStoreService
            .GetUnopenedStoresQuery(ctx)
            .ToListAsync(cancellationToken);

        // Return query for these stores (or empty if none)
        return unopenedStoreIds.Any()
            ? _commonStoreService.GetStoreLocationsByNumbersQuery(ctx, unopenedStoreIds)
            : ctx.Stores.Where(s => false); // Empty query
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
