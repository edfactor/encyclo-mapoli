using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

public class BeneficiaryDetailEndpoint : ProfitSharingEndpoint<BeneficiaryDetailRequest, BeneficiaryDetailResponse>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryDetailEndpoint(IBeneficiaryInquiryService beneficiaryService)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("detail");
        Summary(m =>
        {
            m.Summary = "Get Beneficiary Detail ";
            m.Description = "It will return Beneficiary Detail depends on PSN.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryDetailResponse() } };
        });
        Group<BeneficiariesGroup>();
    }

    protected override Task<BeneficiaryDetailResponse> HandleRequestAsync(BeneficiaryDetailRequest req, CancellationToken ct)
    {
        return _beneficiaryService.GetBeneficiaryDetailAsync(req, ct);
    }

}
