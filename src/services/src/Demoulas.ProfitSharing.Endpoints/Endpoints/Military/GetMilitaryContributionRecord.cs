using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecord : Endpoint<SimpleRequest<int>, MasterInquiryResponseDto>
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
            s.ResponseExamples = new Dictionary<int, object> { { 200, new MasterInquiryResponseDto() } };
        });
        Group<MilitaryGroup>();
    }

    public override Task<MasterInquiryResponseDto> ExecuteAsync(SimpleRequest<int> req, CancellationToken ct)
    {
        return Task.FromResult(new MasterInquiryResponseDto());
    }
}
