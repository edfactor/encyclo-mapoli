using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportManualChecksEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportManualChecksEndpoint> _logger;

    public DistributionRunReportManualChecksEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportManualChecksEndpoint> logger) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("distribution-run-report/manual-checks");
        Summary(s =>
        {
            s.Description = "Gets the manual check portion of a distribution run.";
            s.Summary = "Manual check distributions in distribution run report";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<ManualChecksWrittenResponse>()
                {
                    ManualChecksWrittenResponse.ResponseExample()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    public override Task<Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _distributionService.GetManualCheckDistributions(req, ct);

            // Record distribution manual check report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "distribution-run-report-manual-checks"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportManualChecksEndpoint"));

            _logger.LogInformation("Distribution manual checks report retrieved(correlation: {CorrelationId})",
                 HttpContext.TraceIdentifier);

            return result
                .ToHttpResult(Error.EntityNotFound("ManualCheckDistributions"));
        }); // No sensitive fields accessed
    }
}
