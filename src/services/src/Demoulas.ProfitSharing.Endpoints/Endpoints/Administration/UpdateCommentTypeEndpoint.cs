using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class UpdateCommentTypeEndpoint : ProfitSharingEndpoint<UpdateCommentTypeRequest, Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_commentTypeNotFound = Error.EntityNotFound("Comment type");

    private readonly ICommentTypeService _commentTypeService;

    public UpdateCommentTypeEndpoint(ICommentTypeService commentTypeService)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
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

    protected override async Task<Results<Ok<CommentTypeDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(UpdateCommentTypeRequest req, CancellationToken ct)
    {
        var result = await _commentTypeService.UpdateCommentTypeAsync(req, ct);
        return result.ToHttpResultWithValidation(s_commentTypeNotFound);
    }
}
