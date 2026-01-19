using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public sealed class BeneficiaryDisbursementEndpoint : ProfitSharingEndpoint<BeneficiaryDisbursementRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBeneficiaryDisbursementService _beneficiaryDisbursementService;

    public BeneficiaryDisbursementEndpoint(IBeneficiaryDisbursementService beneficiaryDisbursementService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryDisbursementService = beneficiaryDisbursementService;
    }

    public override void Configure()
    {
        Post("/disbursement");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.ExampleRequest = BeneficiaryDisbursementRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        BeneficiaryDisbursementRequest req,
        CancellationToken ct)
    {
        var result = await _beneficiaryDisbursementService.DisburseFundsToBeneficiaries(req, ct);
        return result.ToHttpResultWithValidation();
    }
}
