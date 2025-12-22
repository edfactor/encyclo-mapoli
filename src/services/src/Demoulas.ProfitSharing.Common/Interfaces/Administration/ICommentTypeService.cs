using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

namespace Demoulas.ProfitSharing.Common.Interfaces.Administration;

public interface ICommentTypeService
{
    Task<Result<IReadOnlyList<CommentTypeDto>>> GetCommentTypesAsync(CancellationToken cancellationToken);

    Task<Result<CommentTypeDto>> UpdateCommentTypeAsync(UpdateCommentTypeRequest request, CancellationToken cancellationToken);
}
