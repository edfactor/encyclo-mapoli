using Demoulas.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record ReportResponseBase<TResponse> where TResponse : class
{
    public required string ReportName { get; set; }
    public required DateTimeOffset ReportDate { get; set; }

    public required PaginatedResponseDto<TResponse> Response { get; set; }
}
