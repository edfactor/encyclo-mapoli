using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class GetCommentTypesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<CommentTypeDto>>
{
    private readonly ICommentTypeService _commentTypeService;

    public GetCommentTypesEndpoint(ICommentTypeService commentTypeService)
        : base(Navigation.Constants.ManageCommentTypes)
    {
        _commentTypeService = commentTypeService;
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

    protected override async Task<Results<Ok<IReadOnlyList<CommentTypeDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _commentTypeService.GetCommentTypesAsync(ct);
        return result.ToHttpResult();
    }
}
