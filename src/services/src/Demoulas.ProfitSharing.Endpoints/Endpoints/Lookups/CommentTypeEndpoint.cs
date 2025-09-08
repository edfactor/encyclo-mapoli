using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CommentTypeEndpoint : ProfitSharingResponseEndpoint<List<CommentTypeResponse>>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CommentTypeEndpoint(IProfitSharingDataContextFactory dataContextFactory) : base(Navigation.Constants.Inquiries)
    {
        _dataContextFactory = dataContextFactory;
    }

    public override void Configure()
    {
        Get("comment-types");
        Summary(s =>
        {
            s.Summary = "Gets all available comment types";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<CommentTypeResponse>
                {
                    new CommentTypeResponse { Name = CommentType.Constants.Military.Name}
                }
            } };
        });
    Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override Task<List<CommentTypeResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(c => c.CommentTypes.Select(x => new CommentTypeResponse { Id = x.Id, Name = x.Name }).ToListAsync(ct));
    }
}
