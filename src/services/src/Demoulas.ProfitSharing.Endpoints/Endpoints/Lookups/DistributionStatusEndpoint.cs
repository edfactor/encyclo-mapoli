using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class DistributionStatusEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<DistributionStatusResponse>>
{
    private readonly IDistributionStatusLookupService _distributionStatusLookupService;
    private readonly ILogger<DistributionStatusEndpoint> _logger;

    public DistributionStatusEndpoint(IDistributionStatusLookupService distributionStatusLookupService, ILogger<DistributionStatusEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _distributionStatusLookupService = distributionStatusLookupService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("distribution-statuses");
        Summary(s =>
        {
            s.Summary = "Gets all available distribution status values";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ListResponseDto<DistributionStatusResponse>.From(
                        new List<DistributionStatusResponse>
                        {
                            new() { Id = 'O', Name = "Okay to Pay" }
                        })
                }
            };
        });
        Group<LookupGroup>();
    }

    protected override async Task<Results<Ok<ListResponseDto<DistributionStatusResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var dto = await _distributionStatusLookupService.GetDistributionStatusesAsync(ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-status-lookup"),
                new("endpoint", "DistributionStatusEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(dto.Items.Count,
                new("record_type", "distribution-statuses"),
                new("endpoint", "DistributionStatusEndpoint"));

            _logger.LogInformation("Distribution status lookup completed, returned {StatusCount} statuses (correlation: {CorrelationId})",
                dto.Items.Count, HttpContext.TraceIdentifier);

            var result = Result<ListResponseDto<DistributionStatusResponse>>.Success(dto);
            var httpResult = result.ToHttpResult();

            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<DistributionStatusResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
