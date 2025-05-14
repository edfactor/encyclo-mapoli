using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Request.Naviations;
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
public class BeneficiaryEndpoint : Endpoint<BeneficiaryRequestDto, BeneficiaryResponseDto>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryEndpoint(IBeneficiaryInquiryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries by PSN_SUFFIX & BADEGE_NUMBER";
            m.Description = "Pass psn_suffix and badge number and get beneficiaries.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryResponseDto() } };
        });
        Group<BeneficiaryGroup>();
    }

    public override async Task<BeneficiaryResponseDto> ExecuteAsync(BeneficiaryRequestDto req, CancellationToken ct)
    {
        var beneficiaryList = await _beneficiaryService.GetBeneficiary(req, ct);
        var response = new BeneficiaryResponseDto { BeneficiaryList = beneficiaryList };
        return response;
    }

}
