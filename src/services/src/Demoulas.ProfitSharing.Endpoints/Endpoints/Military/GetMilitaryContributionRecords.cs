using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : ProfitSharingEndpoint<GetMilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;
    private readonly IAuditService _auditService;
    private readonly ILogger<GetMilitaryContributionRecords> _logger;

    private const string ReportName = "Get All Military Contribution Records";

    public GetMilitaryContributionRecords(IMilitaryService militaryService, IAuditService auditService, ILogger<GetMilitaryContributionRecords> logger) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService;
        _auditService = auditService;
        _logger = logger;
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

    public override Task<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>> ExecuteAsync(GetMilitaryContributionRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var currentYear = (short)DateTimeOffset.UtcNow.Year;
            var response = await _auditService.ArchiveCompletedReportAsync(ReportName,
                currentYear,
                req,
                (archiveReq, isArchiveRequest, cancellationToken) => _militaryService.GetMilitaryServiceRecordAsync(archiveReq, isArchiveRequest, cancellationToken),
                ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "military-contribution-query"),
                new("endpoint", nameof(GetMilitaryContributionRecords)));

            if (response.IsSuccess)
            {
                var resultCount = response.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                    new("record_type", "military-contributions"),
                    new("endpoint", nameof(GetMilitaryContributionRecords)));

                _logger.LogInformation("Military contribution records query completed for Year: {Year}, returned {ResultCount} records (correlation: {CorrelationId})",
                    currentYear, resultCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Military contribution records query failed for Year: {Year} - {Error} (correlation: {CorrelationId})",
                    currentYear, response.Error?.Description, HttpContext.TraceIdentifier);
            }

            return response.Match<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>(
                success => TypedResults.Ok(success),
                error => TypedResults.Problem(error.Detail));
        }, "BadgeNumber");
    }
}
