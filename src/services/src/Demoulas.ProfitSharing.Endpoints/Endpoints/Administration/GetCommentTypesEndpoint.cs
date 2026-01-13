using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class GetCommentTypesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<CommentTypeDto>>
{
    private readonly ICommentTypeService _commentTypeService;
    private readonly ILogger<GetCommentTypesEndpoint> _logger;

    public GetCommentTypesEndpoint(ICommentTypeService commentTypeService, ILogger<GetCommentTypesEndpoint> logger)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("comment-types");
        Summary(s =>
        {
            s.Summary = "Gets all comment types.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<IReadOnlyList<CommentTypeDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var result = await _commentTypeService.GetCommentTypesAsync(ct);

            try
            {
                var count = result.Value?.Count ?? 0;

                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "comment-types-list"),
                    new("endpoint", nameof(GetCommentTypesEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(count,
                    new("record_type", "comment-types"),
                    new("endpoint", nameof(GetCommentTypesEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult();
        });
    }
}
