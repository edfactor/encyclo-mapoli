using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class StateTaxEndpoint : ProfitSharingEndpoint<StateTaxLookupRequest, StateTaxLookupResponse>
{
    private readonly IStateTaxLookupService _stateTaxLookupService;

    public StateTaxEndpoint(IStateTaxLookupService stateTaxLookupService) : base(Navigation.Constants.Unknown)
    {
        _stateTaxLookupService = stateTaxLookupService;
    }

    public override void Configure()
    {
        Get("/state-taxes/{state}");
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

    protected override Task<StateTaxLookupResponse> HandleRequestAsync(StateTaxLookupRequest req, CancellationToken ct)
    {
        return _stateTaxLookupService.LookupStateTaxRateAsync(req.State, ct);
    }
}
