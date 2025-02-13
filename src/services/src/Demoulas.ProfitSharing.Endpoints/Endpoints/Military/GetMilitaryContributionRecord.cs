using System.Security.Cryptography.X509Certificates;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecord : Endpoint<SimpleRequest<int>, List<MasterInquiryResponseDto>>
{
    public GetMilitaryContributionRecord()
    {
    
    }

    public override void Configure()
    {
        Get("detail");
        Summary(s =>
        {
            s.Summary = "Get Military Contribution Record";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<MasterInquiryResponseDto>() } };
        });
        Group<MilitaryGroup>();
    }

    public override Task<List<MasterInquiryResponseDto>> ExecuteAsync(SimpleRequest<int> req, CancellationToken ct)
    {
        return Task.FromResult(new List<MasterInquiryResponseDto>());
    }
}
