using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class DistributionFrequencyEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<DistributionFrequencyResponse>>
{
    private readonly IDistributionFrequencyLookupService _distributionFrequencyLookupService;
    private readonly ILogger<DistributionFrequencyEndpoint> _logger;

    public DistributionFrequencyEndpoint(IDistributionFrequencyLookupService distributionFrequencyLookupService, ILogger<DistributionFrequencyEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _distributionFrequencyLookupService = distributionFrequencyLookupService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("distribution-frequencies");
        Summary(s =>
        {
            s.Summary = "Gets all available distribution frequency values";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ListResponseDto<DistributionFrequencyResponse>.From(
                        new List<DistributionFrequencyResponse>
                        {
                            new() { Id = 'M', Name = "Monthly" }
                        })
                }
            };
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<DistributionFrequencyResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var dto = await _distributionFrequencyLookupService.GetDistributionFrequenciesAsync(ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "distribution-frequency-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            // Record result count
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(dto.Items.Count,
                new KeyValuePair<string, object?>("operation", "distribution-frequency-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            _logger.LogInformation("Distribution frequency lookup completed, returned {Count} frequencies (correlation: {CorrelationId})",
                dto.Items.Count, HttpContext.TraceIdentifier);

            var result = Result<ListResponseDto<DistributionFrequencyResponse>>.Success(dto);

            // Record successful response
            this.RecordResponseMetrics(HttpContext, _logger, dto, isSuccess: true);

            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<DistributionFrequencyResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
