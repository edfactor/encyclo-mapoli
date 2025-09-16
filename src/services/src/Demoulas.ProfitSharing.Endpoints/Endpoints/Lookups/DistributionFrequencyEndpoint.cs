using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
public sealed class DistributionFrequencyEndpoint : ProfitSharingResponseEndpoint<List<DistributionFrequencyResponse>>
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

    public override Task<List<DistributionFrequencyResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(c => c.DistributionFrequencies.Select(x => new DistributionFrequencyResponse { Id = x.Id, Name = x.Name }).ToListAsync(ct));
    }
}
