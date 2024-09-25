using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record ProfitYearRequest : PaginationRequestDto
{
    public short ProfitYear { get; set; }
}
