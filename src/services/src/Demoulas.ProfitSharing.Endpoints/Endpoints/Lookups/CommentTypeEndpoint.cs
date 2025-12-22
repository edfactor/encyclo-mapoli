using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class CommentTypeEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<CommentTypeResponse>>
{
    private readonly ICommentTypeLookupService _commentTypeLookupService;
    private readonly ILogger<CommentTypeEndpoint> _logger;

    public CommentTypeEndpoint(ICommentTypeLookupService commentTypeLookupService, ILogger<CommentTypeEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _commentTypeLookupService = commentTypeLookupService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("comment-types");
        Summary(s =>
        {
            s.Summary = "Gets all available comment types";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ListResponseDto<CommentTypeResponse>.From(new List<CommentTypeResponse>
                    {
                        new() { Name = "Military" }
                    })
                }
            };
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(o => o.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<CommentTypeResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var dto = await _commentTypeLookupService.GetCommentTypesAsync(ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "comment-types-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            // Record result count
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(dto.Items.Count,
                new KeyValuePair<string, object?>("operation", "comment-types-lookup"),
                new KeyValuePair<string, object?>("endpoint.category", "lookups"));

            _logger.LogInformation("Comment types lookup completed, returned {Count} types (correlation: {CorrelationId})",
                dto.Items.Count, HttpContext.TraceIdentifier);

            // Always success (lookup); still using Result<T> pattern for uniformity.
            var result = Result<ListResponseDto<CommentTypeResponse>>.Success(dto);

            // Record successful response
            this.RecordResponseMetrics(HttpContext, _logger, dto, isSuccess: true);

            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<CommentTypeResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
