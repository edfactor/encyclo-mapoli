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
public class BeneficiarySearchFilterEndpoint : Endpoint<BeneficiarySearchFilterRequest, PaginatedResponseDto<BeneficiarySearchFilterResponse>>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;

    public BeneficiarySearchFilterEndpoint(IBeneficiaryInquiryService beneficiaryService)
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
        Group<BeneficiaryGroup>();
    }

    public override async Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> ExecuteAsync(BeneficiarySearchFilterRequest req, CancellationToken ct)
    {
        var response = await _beneficiaryService.BeneficiarySearchFilter(req, ct);
        return response;
    }
          
}
