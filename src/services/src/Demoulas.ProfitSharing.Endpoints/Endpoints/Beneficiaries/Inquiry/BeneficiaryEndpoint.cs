using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

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
        Group<BeneficiariesGroup>();
    }

    protected override async Task<BeneficiaryResponse> HandleRequestAsync(BeneficiaryRequestDto req, CancellationToken ct)
    {
        var result = await _beneficiaryService.GetBeneficiaryAsync(req, ct);
        return result ?? new BeneficiaryResponse();
    }

}
