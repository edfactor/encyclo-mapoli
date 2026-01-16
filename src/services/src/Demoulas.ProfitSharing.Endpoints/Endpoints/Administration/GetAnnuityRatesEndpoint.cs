using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class GetAnnuityRatesEndpoint : ProfitSharingEndpoint<GetAnnuityRatesRequest, Results<Ok<IReadOnlyList<AnnuityRateDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IAnnuityRatesService _annuityRatesService;

    public GetAnnuityRatesEndpoint(IAnnuityRatesService annuityRatesService)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
    }

    public override void Configure()
    {
        Get("annuity-rates");
        Summary(s =>
        {
            s.Summary = "Gets all annuity rates.";
            s.ExampleRequest = GetAnnuityRatesRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<IReadOnlyList<AnnuityRateDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        GetAnnuityRatesRequest req,
        CancellationToken ct)
    {
        var request = new GetAnnuityRatesRequest
        {
            SortBy = string.IsNullOrWhiteSpace(req.SortBy) ? "Year" : req.SortBy,
            IsSortDescending = req.IsSortDescending ?? true,
        };

        var result = await _annuityRatesService.GetAnnuityRatesAsync(request, ct);
        return result.ToHttpResult();
    }
}
