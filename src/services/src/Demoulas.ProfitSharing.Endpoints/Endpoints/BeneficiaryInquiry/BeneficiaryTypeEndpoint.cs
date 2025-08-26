using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;
public class BeneficiaryTypeEndpoint : ProfitSharingEndpoint<BeneficiaryTypesRequestDto, BeneficiaryTypesResponseDto>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryTypeEndpoint(IBeneficiaryInquiryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries type list";
            m.Description = "It will provide you list of Beneficiary Types";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<BeneficiaryDto>() } };
        });
        Group<BeneficiaryTypeGroup>();
    }

    public override async Task<BeneficiaryTypesResponseDto> ExecuteAsync(BeneficiaryTypesRequestDto req, CancellationToken ct)
    {
        var response = await _beneficiaryService.GetBeneficiaryTypes(req, ct);
        return response;
    }

}
