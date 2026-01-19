using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : ProfitSharingEndpoint<GetMilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;

    private const string ReportName = "Get All Military Contribution Records";

    public GetMilitaryContributionRecords(IMilitaryService militaryService, IProfitSharingAuditService profitSharingAuditService) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService;
        _profitSharingAuditService = profitSharingAuditService;
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
        var currentYear = (short)DateTimeOffset.UtcNow.Year;
        var result = await _profitSharingAuditService.ArchiveCompletedReportAsync(ReportName,
            currentYear,
            req,
            (archiveReq, isArchiveRequest, cancellationToken) => _militaryService.GetMilitaryServiceRecordAsync(archiveReq, isArchiveRequest, cancellationToken),
            ct);

        return result.Match<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error.Detail));
    }
}
