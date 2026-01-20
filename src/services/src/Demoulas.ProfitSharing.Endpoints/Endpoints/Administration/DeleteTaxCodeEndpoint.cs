using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class DeleteTaxCodeEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_taxCodeNotFound = Error.EntityNotFound("Tax code");

    private readonly ITaxCodeService _taxCodeService;

    public DeleteTaxCodeEndpoint(ITaxCodeService taxCodeService)
        : base(Navigation.Constants.ManageTaxCodes)
    {
        _taxCodeService = taxCodeService;
    }

    public override void Configure()
    {
        Delete("tax-codes/{id}");
        Summary(s =>
        {
            s.Summary = "Deletes a tax code.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var idValue = Route<string>("id");
        if (string.IsNullOrWhiteSpace(idValue) || idValue.Trim().Length != 1)
        {
            var validation = Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
            {
                ["Id"] = ["Id must be a single character."]
            }));
            return validation.ToHttpResultWithValidation(s_taxCodeNotFound);
        }

        var id = idValue.Trim().ToUpperInvariant()[0];
        var result = await _taxCodeService.DeleteTaxCodeAsync(id, ct);
        return result.ToHttpResultWithValidation(s_taxCodeNotFound);
    }
}
