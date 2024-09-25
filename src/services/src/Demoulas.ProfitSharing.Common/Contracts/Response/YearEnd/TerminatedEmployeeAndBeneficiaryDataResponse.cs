using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;

public sealed record TerminatedEmployeeAndBeneficiaryDataResponse<TResponse> where TResponse : class
{
    public required string ReportName { get; set; }
    public required DateTimeOffset ReportDate { get; set; }

    public required decimal TotalVested { get; set; }
    public required decimal TotalForfeit { get; set; }
    public required decimal TotalEndingBalance { get; set; }
    public required decimal TotalBeneficiaryAllocation { get; set; }

    public required PaginatedResponseDto<TResponse> Response { get; set; }
}
