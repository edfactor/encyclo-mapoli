using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

public class BeneficiarySearchFilterEndpoint : ProfitSharingEndpoint<BeneficiarySearchFilterRequest, PaginatedResponseDto<BeneficiarySearchFilterResponse>>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiarySearchFilterEndpoint(IBeneficiaryInquiryService beneficiaryService)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("search");
        Summary(m =>
        {
            m.Summary = "Get Member result based on beneficiary search Filter";
            m.Description = "It will search member based on Member Type i-e beneficiary or employee.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<BeneficiarySearchFilterResponse>() } };
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> HandleRequestAsync(
        BeneficiarySearchFilterRequest req,
        CancellationToken ct)
    {
        var result = await _beneficiaryService.BeneficiarySearchFilter(req, ct);
        return result ?? new PaginatedResponseDto<BeneficiarySearchFilterResponse>();
    }

}
