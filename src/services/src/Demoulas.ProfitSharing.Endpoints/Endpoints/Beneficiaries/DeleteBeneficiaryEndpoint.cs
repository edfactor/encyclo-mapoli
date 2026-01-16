using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class DeleteBeneficiaryEndpoint : ProfitSharingEndpoint<IdRequest, Results<Ok, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public DeleteBeneficiaryEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Delete("/{Id}");
        Summary(s =>
        {
            s.Summary = "Deletes a beneficiary, and their contact record if this was their only named beneficiary";
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok, ProblemHttpResult>> HandleRequestAsync(IdRequest req, CancellationToken ct)
    {
        try
        {
            await _beneficiaryService.DeleteBeneficiary(req.Id, ct);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
