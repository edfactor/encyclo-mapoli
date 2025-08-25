using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : ProfitSharingEndpoint<MilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;
    private readonly IAuditService _auditService;

    private const string ReportName = "Get All Military Contribution Records";

    public GetMilitaryContributionRecords(IMilitaryService militaryService, IAuditService auditService) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Get(string.Empty);
        Summary(s =>
        {
            s.Summary = ReportName;
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<MilitaryContributionResponse>() } };
        });
        Group<MilitaryGroup>();
    }

    public override async Task<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>> ExecuteAsync(MilitaryContributionRequest req, CancellationToken ct)
    {
        var response = await _auditService.ArchiveCompletedReportAsync(ReportName,
            req.ProfitYear,
            req,
            (archiveReq, isArchiveRequest, cancellationToken) => _militaryService.GetMilitaryServiceRecordAsync(archiveReq, isArchiveRequest, cancellationToken),
            ct);

        return response.Match<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error)
        );
    }
}
