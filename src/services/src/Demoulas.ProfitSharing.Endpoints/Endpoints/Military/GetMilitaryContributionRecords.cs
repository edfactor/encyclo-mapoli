using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : ProfitSharingEndpoint<GetMilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;
    private readonly IAuditService _auditService;
    private readonly TimeProvider _timeProvider;

    private const string ReportName = "Get All Military Contribution Records";

    public GetMilitaryContributionRecords(IMilitaryService militaryService, IAuditService auditService, TimeProvider timeProvider) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService;
        _auditService = auditService;
        _timeProvider = timeProvider;
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

    protected override async Task<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>> HandleRequestAsync(
        GetMilitaryContributionRequest req,
        CancellationToken ct)
    {
        var currentYear = (short)_timeProvider.GetUtcNow().Year;
        var result = await _auditService.ArchiveCompletedReportAsync(ReportName,
            currentYear,
            req,
            (archiveReq, isArchiveRequest, cancellationToken) => _militaryService.GetMilitaryServiceRecordAsync(archiveReq, isArchiveRequest, cancellationToken),
            ct);

        return result.Match<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error.Detail));
    }
}
