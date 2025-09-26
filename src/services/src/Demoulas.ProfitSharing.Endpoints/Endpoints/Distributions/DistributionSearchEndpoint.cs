using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionSearchEndpoint : ProfitSharingEndpoint<DistributionSearchRequest, PaginatedResponseDto<DistributionSearchResponse>>
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

    public override Task<PaginatedResponseDto<DistributionSearchResponse>> ExecuteAsync(DistributionSearchRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _distributionService.SearchAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-search"),
                new("endpoint", "DistributionSearchEndpoint"));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "distribution-search-results"),
                new("endpoint", "DistributionSearchEndpoint"));

            _logger.LogInformation("Distribution search completed for Badge: {BadgeNumber}, PSN Suffix: {PsnSuffix}, returned {ResultCount} results (correlation: {CorrelationId})",
                req.BadgeNumber, req.PsnSuffix, resultCount, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<DistributionSearchResponse>();
        }, "SSN");
    }
}
