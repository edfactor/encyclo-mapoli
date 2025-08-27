using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public class CreateBeneficiaryContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryContactRequest, CreateBeneficiaryContactResponse>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public CreateBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }
    public override void Configure()
    {
        Post("/contact");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary contact";
            s.ExampleRequest = CreateBeneficiaryContactRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryContactResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<CreateBeneficiaryContactResponse> ExecuteAsync(CreateBeneficiaryContactRequest req, CancellationToken ct)
    {
        return _beneficiaryService.CreateBeneficiaryContact(req, ct);
    }
}
