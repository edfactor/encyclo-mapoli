using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
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

public sealed class UpdateCommentTypeEndpoint : ProfitSharingEndpoint<UpdateCommentTypeRequest, Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_commentTypeNotFound = Error.EntityNotFound("Comment type");

    private readonly ICommentTypeService _commentTypeService;
    private readonly ILogger<UpdateCommentTypeEndpoint> _logger;

    public UpdateCommentTypeEndpoint(ICommentTypeService commentTypeService, ILogger<UpdateCommentTypeEndpoint> logger)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("comment-types");
        Summary(s =>
        {
            s.Summary = "Updates a single comment type.";
            s.ExampleRequest = new UpdateCommentTypeRequest { Id = 1, Name = "Updated Comment Type" };
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateCommentTypeRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _commentTypeService.UpdateCommentTypeAsync(req, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "comment-type-update"),
                    new("endpoint", nameof(UpdateCommentTypeEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "comment-type"),
                    new("endpoint", nameof(UpdateCommentTypeEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(s_commentTypeNotFound);
        });
    }
}
