using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class CreateTaxCodeEndpoint : ProfitSharingEndpoint<CreateTaxCodeRequest, Results<Ok<TaxCodeAdminDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly ITaxCodeService _taxCodeService;

    public CreateTaxCodeEndpoint(ITaxCodeService taxCodeService)
        : base(Navigation.Constants.ManageTaxCodes)
    {
        _taxCodeService = taxCodeService;
    }

    public override void Configure()
    {
        Post("tax-codes");
        Validator<CreateTaxCodeRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Creates a new tax code.";
            s.ExampleRequest = CreateTaxCodeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, TaxCodeAdminDto.ResponseExample() }
            };
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<TaxCodeAdminDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        CreateTaxCodeRequest req,
        CancellationToken ct)
    {
        var result = await _taxCodeService.CreateTaxCodeAsync(req, ct);
        return result.ToHttpResultWithValidation();
    }
}
