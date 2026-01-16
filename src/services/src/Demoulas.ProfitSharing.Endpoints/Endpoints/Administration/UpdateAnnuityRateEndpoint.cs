using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class UpdateAnnuityRateEndpoint : ProfitSharingEndpoint<UpdateAnnuityRateRequest, Results<Ok<AnnuityRateDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_annuityRateNotFound = Error.EntityNotFound("Annuity rate");

    private readonly IAnnuityRatesService _annuityRatesService;

    public UpdateAnnuityRateEndpoint(IAnnuityRatesService annuityRatesService)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
    }

    public override void Configure()
    {
        Put("annuity-rates");
        Summary(s =>
        {
            s.Summary = "Updates a single annuity rate.";
            s.ExampleRequest = new UpdateAnnuityRateRequest { Year = 2024, Age = 65, SingleRate = 13.0000m, JointRate = 15.0000m };
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<AnnuityRateDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(UpdateAnnuityRateRequest req, CancellationToken ct)
    {
        var result = await _annuityRatesService.UpdateAnnuityRateAsync(req, ct);
        return result.ToHttpResultWithValidation(s_annuityRateNotFound);
    }
}
