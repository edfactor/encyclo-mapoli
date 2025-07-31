using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
