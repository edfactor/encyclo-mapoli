using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class CreateCommentTypeEndpoint : ProfitSharingEndpoint<CreateCommentTypeRequest, Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly ICommentTypeService _commentTypeService;
    private readonly ILogger<CreateCommentTypeEndpoint> _logger;

    public CreateCommentTypeEndpoint(ICommentTypeService commentTypeService, ILogger<CreateCommentTypeEndpoint> logger)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/administration/comment-types");
        Summary(s =>
        {
            s.Summary = "Create a new comment type";
            s.Description = "Creates a new comment type with optional protection flag.";
            s.ExampleRequest = CreateCommentTypeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, new CommentTypeDto { Id = 1, Name = "New Comment Type", IsProtected = false } }
            };
            s.Responses[400] = "Bad Request. Validation failed (e.g., duplicate name, name too long).";
        });
    }

    public override Task<Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(
        CreateCommentTypeRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _commentTypeService.CreateCommentTypeAsync(req, ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "comment-type-create"),
                new("endpoint", nameof(CreateCommentTypeEndpoint)));

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Comment type created: ID={Id}, Name='{Name}', IsProtected={IsProtected} (correlation: {CorrelationId})",
                    result.Value!.Id, result.Value.Name, result.Value.IsProtected, HttpContext.TraceIdentifier);
            }

            return result.ToHttpResultWithValidation();
        });
    }
}
