using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

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
        Delete("contact/{id}");
        Summary(s =>
        {
            s.Summary = "Deletes a beneficiary contact";
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok, ProblemHttpResult>> HandleRequestAsync(IdRequest req, CancellationToken ct)
    {
        try
        {
            await _beneficiaryService.DeleteBeneficiaryContactAsync(req.Id, ct);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
