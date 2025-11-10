namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record AdhocBeneficiariesReportResponse : ReportResponseBase<BeneficiaryReportDto>
{
    public decimal TotalEndingBalance { get; init; }
}
