using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Navigations;
public class NavigationService : INavigationService
{
    public readonly IProfitSharingDataContextFactory _dataContextFactory;

    public NavigationService(IProfitSharingDataContextFactory dataContextFactory)
    {
        this._dataContextFactory = dataContextFactory;
    }

    
    public async Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken)
    {
        var flatList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.Navigations
                .Include(m=>m.Items)
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
                    Items = BuildTree(x.Id)
                })
                .ToList();
        }

        return BuildTree(null); // root level
    }


    public NavigationDto GetNavigation(int navigationId)
    {
        throw new NotImplementedException();
    }
}
