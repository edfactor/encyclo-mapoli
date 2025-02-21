using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : Endpoint<MilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, ProblemHttpResult>>
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

    public override async Task<Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, ProblemHttpResult>> ExecuteAsync(MilitaryContributionRequest req, CancellationToken ct)
    {
        var response = await _militaryService.GetMilitaryServiceRecordAsync(req, ct);

        return response.Match<Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error)
        );
    }
}
