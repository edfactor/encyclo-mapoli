using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CommentTypeEndpoint : EndpointWithoutRequest<List<CommentTypeResponse>>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CommentTypeEndpoint(IProfitSharingDataContextFactory dataContextFactory)
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
    }

    public override Task<List<CommentTypeResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(c => c.CommentTypes.Select(x => new CommentTypeResponse { Id = x.Id, Name = x.Name }).ToListAsync(ct));
    }
}
