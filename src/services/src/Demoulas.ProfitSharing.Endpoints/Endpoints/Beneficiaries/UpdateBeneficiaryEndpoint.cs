using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public sealed class UpdateBeneficiaryEndpoint : ProfitSharingEndpoint<UpdateBeneficiaryRequest, UpdateBeneficiaryResponse>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public UpdateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("/");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.ExampleRequest = UpdateBeneficiaryRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<UpdateBeneficiaryResponse> ExecuteAsync(UpdateBeneficiaryRequest req, CancellationToken ct)
    {
        return _beneficiaryService.UpdateBeneficiary(req, ct);
    }
}
