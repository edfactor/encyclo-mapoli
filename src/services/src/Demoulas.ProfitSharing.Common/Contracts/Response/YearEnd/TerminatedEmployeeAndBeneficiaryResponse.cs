using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[YearEndArchiveProperty]
public sealed record TerminatedEmployeeAndBeneficiaryResponse : ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>
{
    public required decimal TotalVested { get; set; }
    public required decimal TotalForfeit { get; set; }
    public required decimal TotalEndingBalance { get; set; }
    public required decimal TotalBeneficiaryAllocation { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static TerminatedEmployeeAndBeneficiaryResponse ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryResponse
        {
            ReportName = "terminated-employee-beneficiary",
            ReportDate = DateTimeOffset.UtcNow,
            TotalVested = 125000.00m,
            TotalForfeit = 15000.00m,
            TotalEndingBalance = 450000.00m,
            TotalBeneficiaryAllocation = 75000.00m,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>
            {
                Results = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto> { TerminatedEmployeeAndBeneficiaryDataResponseDto.ResponseExample() }
            }
        };
    }

}
