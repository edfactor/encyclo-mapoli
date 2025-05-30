using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public class CreateBeneficiaryAndContactEndpoint : Endpoint<CreateBeneficiaryRequest, CreateBeneficiaryResponse>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public CreateBeneficiaryAndContactEndpoint(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }
    public override void Configure()
    {
        Post("/");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary, and contact.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<CreateBeneficiaryResponse> ExecuteAsync(CreateBeneficiaryRequest req, CancellationToken ct)
    {
        return _beneficiaryService.CreateBeneficiary(req, ct);
    }
}
