using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class PayClassificationLookupEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<PayClassificationResponseDto>>
{
    private readonly IPayClassificationService _payClassificationService;
    private readonly ILogger<PayClassificationLookupEndpoint> _logger;

    public PayClassificationLookupEndpoint(IPayClassificationService payClassificationService, ILogger<PayClassificationLookupEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _payClassificationService = payClassificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("pay-classifications");
        Summary(s =>
        {
            s.Summary = "Get all pay classifications";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<PayClassificationResponseDto>
                {
                    new PayClassificationResponseDto { Id = "0", Name = "Example"}
                }
            } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR}, {Role.FINANCEMANAGER}, {Role.DISTRIBUTIONSCLERK}, or {Role.HARDSHIPADMINISTRATOR}";
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<PayClassificationResponseDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var set = await _payClassificationService.GetAllPayClassificationsAsync(ct);
            var orderedSet = set.OrderBy(x => x.Name).ToList();

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "pay-classification-lookup"),
                new("endpoint", "PayClassificationLookupEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(orderedSet.Count,
                new("record_type", "pay-classifications"),
                new("endpoint", "PayClassificationLookupEndpoint"));

            _logger.LogInformation("Pay classification lookup completed, returned {ClassificationCount} classifications (correlation: {CorrelationId})",
                orderedSet.Count, HttpContext.TraceIdentifier);

            var dto = ListResponseDto<PayClassificationResponseDto>.From(orderedSet);
            var result = Result<ListResponseDto<PayClassificationResponseDto>>.Success(dto);
            var httpResult = result.ToHttpResult();

            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<PayClassificationResponseDto>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
