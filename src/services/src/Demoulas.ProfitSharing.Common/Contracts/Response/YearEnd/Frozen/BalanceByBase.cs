using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public abstract record BalanceByBase<TDetail> : ReportResponseBase<TDetail> where TDetail : BalanceByDetailBase
{
    public FrozenReportsByAgeRequest.Report ReportType { get; init; }

    public required short TotalMembers { get; init; }
    public required decimal BalanceTotalAmount { get; init; }
    public required short TotalBeneficiaries { get; set; }
    public decimal VestedTotalAmount { get; set; }
    public decimal TotalBeneficiariesAmount { get; set; }
    public decimal TotalBeneficiariesVestedAmount { get; set; }

    public short TotalEmployee
    {
        get { return (short)(TotalMembers - TotalBeneficiaries); }
    }
    public decimal TotalEmployeeAmount
    {
        get { return BalanceTotalAmount - TotalBeneficiariesAmount; }
    }

    public decimal TotalEmployeesVestedAmount
    {
        get { return VestedTotalAmount - TotalBeneficiariesVestedAmount; }
    }

    public int TotalFullTimeCount { get; set; }
    public int TotalPartTimeCount { get; set; }
}
