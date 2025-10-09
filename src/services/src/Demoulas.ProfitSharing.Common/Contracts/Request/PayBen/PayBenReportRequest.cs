using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
public record PayBenReportRequest : SortedPaginationRequestDto
{
    public int? Id { get; set; }
}
