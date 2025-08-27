using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public class UpdateBeneficiaryContactEndpoint:ProfitSharingEndpoint<UpdateBeneficiaryContactRequest, UpdateBeneficiaryContactResponse>
{
    private readonly IBeneficiaryService _beneficiaryService;
    public UpdateBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("/contact");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary contact information";
            s.ExampleRequest = UpdateBeneficiaryContactRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, UpdateBeneficiaryContactResponse.SampleResponse() }
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<UpdateBeneficiaryContactResponse> ExecuteAsync(UpdateBeneficiaryContactRequest req, CancellationToken ct)
    {
        return _beneficiaryService.UpdateBeneficiaryContact(req, ct);
    }
}
