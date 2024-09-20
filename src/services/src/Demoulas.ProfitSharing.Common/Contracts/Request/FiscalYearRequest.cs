using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record FiscalYearRequest : PaginationRequestDto
{
    public short ReportingYear { get; set; }
}
