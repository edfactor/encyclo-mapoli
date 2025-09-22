using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CommentTypeEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<CommentTypeResponse>>
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
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ListResponseDto<CommentTypeResponse>.From(new List<CommentTypeResponse>
                    {
                        new() { Name = CommentType.Constants.Military.Name }
                    })
                }
            };
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(o => o.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<CommentTypeResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var items = await _dataContextFactory.UseReadOnlyContext(c =>
                c.CommentTypes
                 .OrderBy(x => x.Name)
                 .Select(x => new CommentTypeResponse { Id = x.Id, Name = x.Name })
                 .ToListAsync(ct));

            var dto = ListResponseDto<CommentTypeResponse>.From(items);
            // Always success (lookup); still using Result<T> pattern for uniformity.
            var result = Result<ListResponseDto<CommentTypeResponse>>.Success(dto);
            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<ListResponseDto<CommentTypeResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
