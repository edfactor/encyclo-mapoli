using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class GetTaxCodesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<TaxCodeAdminDto>>
{
    private readonly ITaxCodeService _taxCodeService;

    public GetTaxCodesEndpoint(ITaxCodeService taxCodeService)
        : base(Navigation.Constants.ManageTaxCodes)
    {
        _taxCodeService = taxCodeService;
    }

    public override void Configure()
    {
        Get("tax-codes");
        Summary(s =>
        {
            s.Summary = "Gets all tax codes for administration.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<IReadOnlyList<TaxCodeAdminDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _taxCodeService.GetTaxCodesAsync(ct);
        return result.ToHttpResult();
    }
}
