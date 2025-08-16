using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;
public class BeneficiaryKindEndpoint : Endpoint<BeneficiaryKindRequestDto, BeneficiaryKindResponseDto>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryKindEndpoint(IBeneficiaryInquiryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries kind list";
            m.Description = "It will provide you list of Beneficiary kind";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryKindResponseDto() { } } };
        });
        Group<BeneficiaryKindGroup>();
    }

    public override async Task<BeneficiaryKindResponseDto> ExecuteAsync(BeneficiaryKindRequestDto req, CancellationToken ct)
    {
        var response = await _beneficiaryService.GetBeneficiaryKind(req, ct);
        return response;
    }

}
