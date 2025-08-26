using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Navigations;
public class NavigationService : INavigationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAppUser _appUser;

    public NavigationService(IProfitSharingDataContextFactory dataContextFactory, IAppUser appUser)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
    }


    public async Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken)
    {
        var flatList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.Navigations
                .Include(m => m.Items)
                .Include(m => m.RequiredRoles)
                .Include(m => m.NavigationStatus)
                .OrderBy(x => x.OrderNumber)
                .ToListAsync(cancellationToken)
        );

        var lookup = flatList.ToLookup(x => x.ParentId);
        List<NavigationDto> BuildTree(short? parentId)
        {
            return lookup[parentId]
                .Select(x => new NavigationDto
                {
                    Id = x.Id,
                    Icon = x.Icon,
                    OrderNumber = x.OrderNumber,
                    ParentId = x.ParentId,
                    StatusId = x.StatusId,
                    StatusName = x.NavigationStatus?.Name,
                    Title = x.Title,
                    Url = x.Url,
                    SubTitle = x.SubTitle,
                    Items = BuildTree(x.Id),
                    Disabled = x.Disabled,
                    RequiredRoles = x.RequiredRoles?.Select(m => m.Name).ToList()
                })
                .ToList();
        }

        return BuildTree(null); // root level
    }


    public NavigationDto GetNavigation(short navigationId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken)
    {
        var navigationStatusList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.NavigationStatuses.Select(x => new NavigationStatusDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellationToken)
        );
        return navigationStatusList;
    }


    public async Task<bool> UpdateNavigation(short navigationId, byte statusId, CancellationToken cancellationToken)
    {
        var success = await _dataContextFactory.UseWritableContext(async context =>
        {
            //update navigation status
            var nav = await context.Navigations.FirstOrDefaultAsync(x => x.Id == navigationId, cancellationToken);
            if (nav == null)
            {
                return 0;
            }

            nav.StatusId = statusId;

            //Navigation Tracker
            await context.NavigationTrackings.AddAsync(
                new Data.Entities.Navigations.NavigationTracking()
                {
                    NavigationId = navigationId, StatusId = statusId, Username = _appUser.UserName ?? "", LastModified = DateTimeOffset.UtcNow
                }, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
        return success > 0;
    }
}
