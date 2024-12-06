using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record VestedAmountsByAge : ReportResponseBase<VestedAmountsByAgeDetail>
{
    public FrozenReportsByAgeRequest.Report ReportType { get; set; }
    public decimal TotalFullTimeAmount { get; set; }
    public decimal TotalNotVestedAmount { get; set; }
    public decimal TotalPartialVestedAmount { get; set; }
    public decimal TotalBeneficiaryAmount { get; set; }
    public short TotalFullTimeCount { get; set; }
    public short TotalNotVestedCount { get; set; }
    public short TotalPartialVestedCount { get; set; }
    public short TotalBeneficiaryCount { get; set; }
}
