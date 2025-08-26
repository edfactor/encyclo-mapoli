using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;
public class BeneficiaryEndpoint : ProfitSharingEndpoint<BeneficiaryRequestDto, BeneficiaryResponse>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryEndpoint(IBeneficiaryInquiryService beneficiaryService)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries by PSN_SUFFIX & BADEGE_NUMBER";
            m.Description = "Pass psn_suffix and badge number and get beneficiaries.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryResponse() } };
        });
        Group<BeneficiaryGroup>();
    }

    public override async Task<BeneficiaryResponse> ExecuteAsync(BeneficiaryRequestDto req, CancellationToken ct)
    {
        var beneficiaryList = await _beneficiaryService.GetBeneficiary(req, ct);
        return beneficiaryList;
    }

}
