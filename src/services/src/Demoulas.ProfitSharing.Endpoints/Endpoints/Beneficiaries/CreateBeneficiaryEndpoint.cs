using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class CreateBeneficiaryAndContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryRequest, Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public CreateBeneficiaryAndContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }
    public override void Configure()
    {
        Post("/");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary";
            s.ExampleRequest = CreateBeneficiaryRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        CreateBeneficiaryRequest req,
        CancellationToken ct)
    {
        var result = await _beneficiaryService.CreateBeneficiary(req, ct);
        return Result<CreateBeneficiaryResponse>.Success(result).ToHttpResult();
    }
}
