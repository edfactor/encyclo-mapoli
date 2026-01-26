using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionSearchEndpoint : ProfitSharingEndpoint<DistributionSearchRequest, Results<Ok<PaginatedResponseDto<DistributionSearchResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionSearchEndpoint> _logger;

    public DistributionSearchEndpoint(IDistributionService distributionService, ILogger<DistributionSearchEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }
    public override void Configure()
    {
        Post("search");
        Group<DistributionGroup>();
        Summary(s =>
        {
            s.Summary = "Search for profit sharing members with filters and pagination.";
            s.Description =
                "Returns a paginated list of members (employees or beneficiaries) matching the provided search criteria, such as profit year, name, SSN, and other filters. Use MemberType=1 for employees and MemberType=2 for beneficiaries.";
            s.ExampleRequest = DistributionSearchRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<DistributionSearchResponse>
                    {
                        Results = new List<DistributionSearchResponse>
                        {
                            DistributionSearchResponse.SampleResponse()
                        },
                        Total = 1
                    }
                }
            };
            s.Responses[400] = "Bad Request. Invalid search parameters provided.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Not Found. No members matched the search criteria.";
        });
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<DistributionSearchResponse>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        DistributionSearchRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var result = await _distributionService.SearchAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-search"),
                new("endpoint", "DistributionSearchEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "distributions"),
                    new("endpoint", "DistributionSearchEndpoint"));

                _logger.LogInformation("Distribution search completed, returned {Count} results (correlation: {CorrelationId})",
                    recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution search failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.SsnNotFound, Error.BadgeNumberNotFound);
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
