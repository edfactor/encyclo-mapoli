using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
public sealed class StateTaxEndpoint : ProfitSharingEndpoint<StateTaxLookupRequest, StateTaxLookupResponse>
{
    private readonly IStateTaxLookupService _stateTaxLookupService;

    public StateTaxEndpoint(IStateTaxLookupService stateTaxLookupService): base(Navigation.Constants.Unknown)
    {
        _stateTaxLookupService = stateTaxLookupService;
    }

    public override void Configure()
    {
        Get("/state-taxes/:State");
        Group<LookupGroup>();
        Summary(s =>
        {
            s.Description = "Returns the default state tax rate for a given state abbreviation.";
            s.Summary = "Lookup state tax rate by state abbreviation";
            s.ExampleRequest = new StateTaxLookupRequest { State = "MA" };
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new StateTaxLookupResponse
                {
                    State = "MA",
                    StateTaxRate = 5.05m
                }
            } };
        });
    }

    public override Task<StateTaxLookupResponse> ExecuteAsync(StateTaxLookupRequest req, CancellationToken ct) => _stateTaxLookupService.LookupStateTaxRate(req.State, ct);
}
