using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class GetMilitaryContributionRecords : ProfitSharingEndpoint<MilitaryContributionRequest, Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>
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

    public override async Task<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>> ExecuteAsync(MilitaryContributionRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _auditService.ArchiveCompletedReportAsync(ReportName,
                req.ProfitYear,
                req,
                (archiveReq, isArchiveRequest, cancellationToken) => _militaryService.GetMilitaryServiceRecordAsync(archiveReq, isArchiveRequest, cancellationToken),
                ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "military-contribution-query"),
                new("endpoint", "GetMilitaryContributionRecords"));

            if (response.IsSuccess)
            {
                var resultCount = response.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                    new("record_type", "military-contributions"),
                    new("endpoint", "GetMilitaryContributionRecords"));

                _logger.LogInformation("Military contribution records query completed for ProfitYear: {ProfitYear}, returned {ResultCount} records (correlation: {CorrelationId})",
                    req.ProfitYear, resultCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Military contribution records query failed for ProfitYear: {ProfitYear} - {Error} (correlation: {CorrelationId})",
                    req.ProfitYear, response.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = response.Match<Results<Ok<PaginatedResponseDto<MilitaryContributionResponse>>, ProblemHttpResult>>(
                success => TypedResults.Ok(success),
                error => TypedResults.Problem(error)
            );

            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
