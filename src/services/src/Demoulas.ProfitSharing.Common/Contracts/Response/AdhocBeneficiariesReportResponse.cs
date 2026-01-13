using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record AdhocBeneficiariesReportResponse : ReportResponseBase<BeneficiaryReportDto>
{
    public decimal TotalEndingBalance { get; init; }

    public static AdhocBeneficiariesReportResponse ResponseExample() => new()
    {
        ReportName = "Adhoc Beneficiaries Report",
        ReportDate = DateTimeOffset.UtcNow,
        StartDate = new DateOnly(2024, 1, 1),
        EndDate = new DateOnly(2024, 12, 31),
        TotalEndingBalance = 125000.50m,
        Response = new PaginatedResponseDto<BeneficiaryReportDto>
        {
            Results = new List<BeneficiaryReportDto> { BeneficiaryReportDto.ResponseExample() }
        }
    };
}
