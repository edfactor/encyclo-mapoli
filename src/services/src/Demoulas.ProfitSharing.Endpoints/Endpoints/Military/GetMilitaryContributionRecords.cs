using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : Endpoint<SimpleRequest<int>, PaginatedResponseDto<MasterInquiryResponseDto>>
{
    public GetMilitaryContributionRecords()
    {
    
    }

    public override void Configure()
    {
        Get("details");
        Summary(s =>
        {
            s.Summary = "Get All Military Contribution Records";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<MasterInquiryResponseDto>() } };
        });
        Group<MilitaryGroup>();
    }

    public override Task<PaginatedResponseDto<MasterInquiryResponseDto>> ExecuteAsync(SimpleRequest<int> req, CancellationToken ct)
    {
        return Task.FromResult(new PaginatedResponseDto<MasterInquiryResponseDto>());
    }
}
