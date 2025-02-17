using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : Endpoint<MilitaryContributionRequest, PaginatedResponseDto<MasterInquiryResponseDto>>
{
    private readonly IMilitaryService _militaryService;

    public GetMilitaryContributionRecords(IMilitaryService militaryService)
    {
        _militaryService = militaryService;
    }

    public override void Configure()
    {
        Get(string.Empty);
        Summary(s =>
        {
            s.Summary = "Get All Military Contribution Records";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<MasterInquiryResponseDto>() } };
        });
        Group<MilitaryGroup>();
    }

    public override Task<PaginatedResponseDto<MasterInquiryResponseDto>> ExecuteAsync(MilitaryContributionRequest req, CancellationToken ct)
    {
        return _militaryService.GetMilitaryServiceRecordAsync(req, ct);
    }
}
