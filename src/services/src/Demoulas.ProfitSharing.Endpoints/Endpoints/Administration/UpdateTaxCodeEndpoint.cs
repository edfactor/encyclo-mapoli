using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class UpdateTaxCodeEndpoint : ProfitSharingEndpoint<UpdateTaxCodeRequest, Results<Ok<TaxCodeAdminDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_taxCodeNotFound = Error.EntityNotFound("Tax code");

    private readonly ITaxCodeService _taxCodeService;

    public UpdateTaxCodeEndpoint(ITaxCodeService taxCodeService)
        : base(Navigation.Constants.ManageTaxCodes)
    {
        _taxCodeService = taxCodeService;
    }

    public override void Configure()
    {
        Put("tax-codes");
        Validator<UpdateTaxCodeRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates a tax code.";
            s.ExampleRequest = UpdateTaxCodeRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<TaxCodeAdminDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        UpdateTaxCodeRequest req,
        CancellationToken ct)
    {
        var result = await _taxCodeService.UpdateTaxCodeAsync(req, ct);
        return result.ToHttpResultWithValidation(s_taxCodeNotFound);
    }
}
