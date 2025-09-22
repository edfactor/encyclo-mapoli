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
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class DistributionStatusEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<DistributionStatusResponse>>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public DistributionStatusEndpoint(IProfitSharingDataContextFactory dataContextFactory) : base(Navigation.Constants.Inquiries)
    {
        _dataContextFactory = dataContextFactory;
    }

    public override void Configure()
    {
        Get("distribution-statuses");
        Summary(s =>
        {
            s.Summary = "Gets all available distribution status values";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DistributionStatusResponse>
                {
                    new DistributionStatusResponse { Id = DistributionStatus.Constants.OkayToPay, Name="Okay to Pay"}
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

    public override async Task<Results<Ok<ListResponseDto<DistributionStatusResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var items = await _dataContextFactory.UseReadOnlyContext(c => c.DistributionStatuses
                .OrderBy(x => x.Name)
                .Select(x => new DistributionStatusResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(ct));
            var dto = ListResponseDto<DistributionStatusResponse>.From(items);
            var result = Result<ListResponseDto<DistributionStatusResponse>>.Success(dto);
            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<ListResponseDto<DistributionStatusResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
