using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Navigations;
public class NavigationService : INavigationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public NavigationService(IProfitSharingDataContextFactory dataContextFactory)
    {
        this._dataContextFactory = dataContextFactory;
    }

    
    public async Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken)
    {
        var flatList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.Navigations
                .Include(m=>m.Items)
                .Include(m=>m.RequiredRoles)
                .OrderBy(x => x.OrderNumber)
                .ToListAsync(cancellationToken)
        );

        var lookup = flatList.ToLookup(x => x.ParentId);
        List<NavigationDto> BuildTree(int? parentId)
        {
            return lookup[parentId]
                .Select(x => new NavigationDto
                {
                    Id = x.Id,
                    Icon = x.Icon,
                    OrderNumber = x.OrderNumber,
                    ParentId = x.ParentId,
                    StatusId = x.StatusId,
                    Title = x.Title,
                    Url = x.Url,
                    SubTitle = x.SubTitle,
                    Items = BuildTree(x.Id),
                    Disabled = x.Disabled,
                    RequiredRoles = x.RequiredRoles?.Select(m=>m.Name).ToList()
                })
                .ToList();
        }

        return BuildTree(null); // root level
    }


    public NavigationDto GetNavigation(int navigationId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken)
    {
        var navigationStatusList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.NavigationStatuses.ToListAsync(cancellationToken)
        );
        return navigationStatusList.Select(x => new NavigationStatusDto { Id = x.Id, Name = x.Name }).ToList();
    }


    public async Task<bool> UpdateNavigation(int navigationId,byte statusId, CancellationToken cancellationToken)
    {
        var success = await _dataContextFactory.UseWritableContext(context =>
        context.Navigations.Where(x => x.Id == navigationId)
        .ExecuteUpdateAsync(x => x.SetProperty(p => p.StatusId, statusId)), cancellationToken
        );
        return success ==1;
    }

}
