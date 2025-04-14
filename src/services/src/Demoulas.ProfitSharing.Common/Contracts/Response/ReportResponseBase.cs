using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record ReportResponseBase<TResponse> where TResponse : class
{
    public required string ReportName { get; init; }
    public DateTimeOffset ReportDate { get; init; } = DateTimeOffset.Now;

    public required PaginatedResponseDto<TResponse> Response { get; set; }
}
