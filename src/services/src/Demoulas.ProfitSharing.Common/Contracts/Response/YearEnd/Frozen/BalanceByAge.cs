using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record BalanceByAge : ReportResponseBase<BalanceByAgeDetail>
{
    public BalanceByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

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


    public static BalanceByAge ResponseExample()
    {
        return new BalanceByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            BalanceTotalAmount = (decimal)1_855_156.09,
            TotalMembers = 63,
            TotalBeneficiaries = 13,

            Response = new PaginatedResponseDto<BalanceByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<BalanceByAgeDetail> { BalanceByAgeDetail.ResponseExample() }
            }
        };
    }
}
