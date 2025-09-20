using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class MissiveLookupEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<MissiveResponse>>
{
    private readonly IMissiveService _missiveService;

    public MissiveLookupEndpoint(IMissiveService missiveService) : base(Navigation.Constants.Inquiries)
    {
        _missiveService = missiveService;
    }
    public override void Configure()
    {
        Get("missives");
        Summary(s =>
        {
            s.Summary = "Geta all available missives";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<MissiveResponse>
                {
                    new MissiveResponse {Id = Missive.Constants.VestingIncreasedOnCurrentBalance, Message = "***Vesting Increased***", Description="More descriptive text explaining the warning", Severity = "Error" }
                }
            } };
        });

        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<MissiveResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var items = await _missiveService.GetAllMissives(ct);
            var dto = ListResponseDto<MissiveResponse>.From(items.OrderBy(x => x.Message));
            var result = Result<ListResponseDto<MissiveResponse>>.Success(dto);
            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<ListResponseDto<MissiveResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
