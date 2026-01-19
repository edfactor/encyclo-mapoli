using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public sealed class UpdateBeneficiaryEndpoint : ProfitSharingEndpoint<UpdateBeneficiaryRequest, Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public UpdateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.Description = "Updates an existing beneficiary's information. Requires beneficiary ID in request body.";
            s.ExampleRequest = UpdateBeneficiaryRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        UpdateBeneficiaryRequest req,
        CancellationToken ct)
    {
        var result = await _beneficiaryService.UpdateBeneficiary(req, ct);
        return result is null
            ? Result<UpdateBeneficiaryResponse>.Failure(Error.EntityNotFound("Beneficiary"))
                .ToHttpResult(Error.EntityNotFound("Beneficiary"))
            : Result<UpdateBeneficiaryResponse>.Success(result).ToHttpResult();
    }
}
