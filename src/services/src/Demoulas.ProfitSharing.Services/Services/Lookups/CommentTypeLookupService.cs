using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
//  ListResponseDto

namespace Demoulas.ProfitSharing.Services.Services.Lookups;

public sealed class CommentTypeLookupService(IProfitSharingDataContextFactory factory) : ICommentTypeLookupService
{
    public Task<ListResponseDto<CommentTypeResponse>> GetCommentTypesAsync(CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var items = await ctx.CommentTypes
                .OrderBy(x => x.Name)
                .Select(x => new CommentTypeResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
#pragma warning restore DSMPS001

            return ListResponseDto<CommentTypeResponse>.From(items);
        }, cancellationToken);
    }
}
