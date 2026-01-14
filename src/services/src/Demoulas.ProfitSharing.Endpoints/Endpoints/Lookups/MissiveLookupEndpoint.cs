using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class MissiveLookupEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<MissiveResponse>>
{
    private readonly IMissiveService _missiveService;
    private readonly ILogger<MissiveLookupEndpoint> _logger;

    public MissiveLookupEndpoint(IMissiveService missiveService, ILogger<MissiveLookupEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _missiveService = missiveService;
        _logger = logger;
    }
    public override void Configure()
    {
        Get("missives");
        Summary(s =>
        {
            s.Summary = "Geta all available missives";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<MissiveResponse>
                {
                    new MissiveResponse {Id = Missive.Constants.VestingIncreasedOnCurrentBalance, Message = "***Vesting Increased***", Description="More descriptive text explaining the warning", Severity = "Error" }
                }
            } };
        });

        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<MissiveResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var items = await _missiveService.GetAllMissives(ct);
            var orderedItems = items.OrderBy(x => x.Message).ToList();

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "missive-lookup"),
                new("endpoint", "MissiveLookupEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(orderedItems.Count,
                new("record_type", "missives"),
                new("endpoint", "MissiveLookupEndpoint"));

            _logger.LogInformation("Missive lookup completed, returned {MissiveCount} missives (correlation: {CorrelationId})",
                orderedItems.Count, HttpContext.TraceIdentifier);

            var dto = ListResponseDto<MissiveResponse>.From(orderedItems);
            var result = Result<ListResponseDto<MissiveResponse>>.Success(dto);
            var httpResult = result.ToHttpResult();

            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<MissiveResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
