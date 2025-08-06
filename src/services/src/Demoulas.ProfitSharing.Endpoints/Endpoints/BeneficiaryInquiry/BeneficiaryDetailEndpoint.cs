using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
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
public class BeneficiaryDetailEndpoint : Endpoint<BeneficiaryDetailRequest, BeneficiaryDetailResponse>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiaryDetailEndpoint(IBeneficiaryInquiryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Get("detail");
        Summary(m =>
        {
            m.Summary = "Get Beneficiary Detail Object";
            m.Description = "It will return Beneficiary Detail object depends on PSN.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryDetailResponse() } };
        });
        Group<BeneficiaryGroup>();
    }

    public override async Task<BeneficiaryDetailResponse> ExecuteAsync(BeneficiaryDetailRequest req, CancellationToken ct)
    {
        
        var response = await _beneficiaryService.GetBeneficiaryDetail(req, ct);
        return response;
    }

}
