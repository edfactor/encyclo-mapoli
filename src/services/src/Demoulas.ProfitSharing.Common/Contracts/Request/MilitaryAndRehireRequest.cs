using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record MilitaryAndRehireRequest : PaginationRequestDto
{
    public short ReportingYear { get; set; }
}
