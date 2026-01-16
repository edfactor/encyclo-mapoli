using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class GetStateTaxRatesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<StateTaxRateDto>>
{
    private readonly IStateTaxRatesService _stateTaxRatesService;

    public GetStateTaxRatesEndpoint(IStateTaxRatesService stateTaxRatesService)
        : base(Navigation.Constants.ManageStateTaxRates)
    {
        _stateTaxRatesService = stateTaxRatesService;
    }

    public override void Configure()
    {
        Get("state-tax-rates");
        Summary(s =>
        {
            s.Summary = "Gets all state tax rates.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<IReadOnlyList<StateTaxRateDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _stateTaxRatesService.GetStateTaxRatesAsync(ct);
        return result.ToHttpResult();
    }
}
