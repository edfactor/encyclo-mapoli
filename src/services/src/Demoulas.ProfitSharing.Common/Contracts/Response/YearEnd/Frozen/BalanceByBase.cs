using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public abstract record BalanceByBase<TDetail> : ReportResponseBase<TDetail> where TDetail : BalanceByDetailBase
{
    public FrozenReportsByAgeRequest.Report ReportType { get; init; }

    public required ushort TotalMembers { get; init; }
    public required decimal BalanceTotalAmount { get; init; }
    public required ushort TotalBeneficiaries { get; set; }
    public decimal VestedTotalAmount { get; set; }
    public decimal TotalBeneficiariesAmount { get; set; }
    public decimal TotalBeneficiariesVestedAmount { get; set; }

    public ushort TotalEmployee
    {
        get { return (ushort)(TotalMembers - TotalBeneficiaries); }
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
