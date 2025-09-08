using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
public sealed class MissiveLookupEndpoint : ProfitSharingResponseEndpoint<List<MissiveResponse>>
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

    public override Task<List<MissiveResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _missiveService.GetAllMissives(ct);
    }
}
