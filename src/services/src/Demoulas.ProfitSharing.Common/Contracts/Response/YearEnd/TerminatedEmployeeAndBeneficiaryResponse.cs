using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[YearEndArchiveProperty]
public sealed record TerminatedEmployeeAndBeneficiaryResponse : ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>
{
    public required decimal TotalVested { get; set; }
    public required decimal TotalForfeit { get; set; }
    public required decimal TotalEndingBalance { get; set; }
    public required decimal TotalBeneficiaryAllocation { get; set; }

}
