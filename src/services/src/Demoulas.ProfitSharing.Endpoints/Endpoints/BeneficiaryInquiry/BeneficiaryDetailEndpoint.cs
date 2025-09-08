using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

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

    public override async Task<BeneficiaryDetailResponse> ExecuteAsync(BeneficiaryDetailRequest req, CancellationToken ct)
    {
        
        var response = await _beneficiaryService.GetBeneficiaryDetail(req, ct);
        return response;
    }

}
