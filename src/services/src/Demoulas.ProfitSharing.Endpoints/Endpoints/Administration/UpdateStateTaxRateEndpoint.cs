using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class UpdateStateTaxRateEndpoint : ProfitSharingEndpoint<UpdateStateTaxRateRequest, Results<Ok<StateTaxRateDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_stateTaxNotFound = Error.EntityNotFound("State tax");

    private readonly IStateTaxRatesService _stateTaxRatesService;

    public UpdateStateTaxRateEndpoint(IStateTaxRatesService stateTaxRatesService)
        : base(Navigation.Constants.ManageStateTaxRates)
    {
        _stateTaxRatesService = stateTaxRatesService;
    }

    public override void Configure()
    {
        Put("state-tax-rates");
        Validator<UpdateStateTaxRateRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates a single state tax rate.";
            s.ExampleRequest = new UpdateStateTaxRateRequest { Abbreviation = "MA", Rate = 5.00m };
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<StateTaxRateDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(UpdateStateTaxRateRequest req, CancellationToken ct)
    {
        var result = await _stateTaxRatesService.UpdateStateTaxRateAsync(req, ct);
        return result.ToHttpResultWithValidation(s_stateTaxNotFound);
    }
}
