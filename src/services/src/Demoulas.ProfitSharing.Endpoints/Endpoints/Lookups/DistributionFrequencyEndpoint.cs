using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class DistributionFrequencyEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<DistributionFrequencyResponse>>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public DistributionFrequencyEndpoint(IProfitSharingDataContextFactory dataContextFactory) : base(Navigation.Constants.Inquiries)
    {
        _dataContextFactory = dataContextFactory;
    }

    public override void Configure()
    {
        Get("distribution-frequencies");
        Summary(s =>
        {
            s.Summary = "Gets all available distribution frequency values";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DistributionFrequencyResponse>
                {
                    new DistributionFrequencyResponse { Id = DistributionFrequency.Constants.Monthly, Name="Monthly"}
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

    public override async Task<Results<Ok<ListResponseDto<DistributionFrequencyResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var items = await _dataContextFactory.UseReadOnlyContext(c => c.DistributionFrequencies
                .OrderBy(x => x.Name)
                .Select(x => new DistributionFrequencyResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(ct));
            var dto = ListResponseDto<DistributionFrequencyResponse>.From(items);
            var result = Result<ListResponseDto<DistributionFrequencyResponse>>.Success(dto);
            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<ListResponseDto<DistributionFrequencyResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
