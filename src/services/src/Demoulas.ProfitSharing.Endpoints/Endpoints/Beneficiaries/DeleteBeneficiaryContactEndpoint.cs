using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class DeleteBeneficiaryContactEndpoint : ProfitSharingEndpoint<IdRequest, Results<Ok, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public DeleteBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Delete("contact/{Id}");
        Summary(s =>
        {
            s.Summary = "Deletes a beneficiary contact";
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        try
        {
            await _beneficiaryService.DeleteBeneficiaryContact(req.Id, ct);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
