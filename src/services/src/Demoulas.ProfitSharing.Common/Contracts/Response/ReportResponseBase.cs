using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

[NoMemberDataExposed]
public record ReportResponseBase<TResponse> : IHasDateRange where TResponse : class
{
    public required string ReportName { get; init; }
    public DateTimeOffset ReportDate { get; init; } = DateTimeOffset.Now;
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public required PaginatedResponseDto<TResponse> Response { get; set; }
}
