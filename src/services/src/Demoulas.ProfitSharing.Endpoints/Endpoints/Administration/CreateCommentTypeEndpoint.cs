using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class CreateCommentTypeEndpoint : ProfitSharingEndpoint<CreateCommentTypeRequest, Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly ICommentTypeService _commentTypeService;

    public CreateCommentTypeEndpoint(ICommentTypeService commentTypeService)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
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

    protected override async Task<Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        CreateCommentTypeRequest req,
        CancellationToken ct)
    {
        var result = await _commentTypeService.CreateCommentTypeAsync(req, ct);
        return result.ToHttpResultWithValidation();
    }
}
