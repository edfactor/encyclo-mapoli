using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Entities.Navigation;
using Demoulas.Common.Data.Services.Service.Navigation;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NavigationStatusIds = Demoulas.ProfitSharing.Common.Constants.NavigationStatusIds;

namespace Demoulas.ProfitSharing.Services.Services.SystemInfo.Navigations;

public sealed class ProfitSharingNavigationService : NavigationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    public ProfitSharingNavigationService(
        IProfitSharingDataContextFactory dataContextFactory,
        IAppUser appUser,
        IDistributedCache? distributedCache,
        ILoggerFactory loggerFactory)
        : base(
            (navigationId, statusId, username, ct) => UpdateNavigationAsync(dataContextFactory, navigationId, statusId, username ?? string.Empty, ct),
            ct => ResetAllStatusesAsync(dataContextFactory, ct),
            appUser,
            distributedCache ?? new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
            loggerFactory.CreateLogger<NavigationService>())
    {
        _dataContextFactory = dataContextFactory;
    }

    protected override async Task<IEnumerable<Navigation>> GetNavigationListAsync(CancellationToken cancellationToken = default)
    {
        var navigationList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.Navigations
                .OrderBy(n => n.OrderNumber)
                .Include(n => n.RequiredRoles)
                .Include(n => n.NavigationStatus)
                .Include(n => n.CustomSettings)
                .Include(n => n.PrerequisiteNavigations!)
                    .ThenInclude(p => p.NavigationStatus)
                .Include(n => n.PrerequisiteNavigations!)
                    .ThenInclude(p => p.RequiredRoles)
                .ToListAsync(cancellationToken),
            cancellationToken);

        return navigationList;
    }

    protected override async Task<IEnumerable<NavigationStatus>> GetNavigationStatusListAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return await _dataContextFactory.UseReadOnlyContext(context =>
                context.NavigationStatuses.ToListAsync(cancellationToken),
            cancellationToken);
    }

    private static async Task<bool> UpdateNavigationAsync(
        IProfitSharingDataContextFactory dataContextFactory,
        short navigationId,
        byte statusId,
        string username,
        CancellationToken cancellationToken)
    {
        int success = await dataContextFactory.UseWritableContext(async context =>
        {
            var nav = await context.Navigations.FirstOrDefaultAsync(x => x.Id == navigationId, cancellationToken);
            if (nav == null)
            {
                return 0;
            }

            nav.StatusId = statusId;

            await context.NavigationTrackings.AddAsync(
                new NavigationTracking
                {
                    NavigationId = navigationId,
                    StatusId = statusId,
                    Username = username ?? string.Empty,
                    LastModified = DateTimeOffset.UtcNow
                }, cancellationToken);

            return await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return success > 0;
    }

    private static Task<int> ResetAllStatusesAsync(
        IProfitSharingDataContextFactory dataContextFactory,
        CancellationToken cancellationToken)
    {
        return dataContextFactory.UseWritableContext(context =>
            context.Navigations
                .Where(n => n.StatusId != null && n.StatusId != NavigationStatusIds.NotStarted)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(n => n.StatusId, NavigationStatusIds.NotStarted),
                    cancellationToken),
            cancellationToken);
    }
}
