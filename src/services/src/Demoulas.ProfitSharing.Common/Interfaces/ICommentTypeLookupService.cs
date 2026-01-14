using Demoulas.ProfitSharing.Common.Contracts.Response; // ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICommentTypeLookupService
{
    Task<ListResponseDto<CommentTypeResponse>> GetCommentTypesAsync(CancellationToken cancellationToken = default);
}
