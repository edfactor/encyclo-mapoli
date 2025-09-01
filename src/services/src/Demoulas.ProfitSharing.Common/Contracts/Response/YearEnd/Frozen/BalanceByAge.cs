using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record BalanceByAge : BalanceByBase<BalanceByAgeDetail>
{
    public BalanceByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public static BalanceByAge ResponseExample()
    {
        return new BalanceByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            BalanceTotalAmount = (decimal)1_855_156.09,
            TotalMembers = 63,
            TotalBeneficiaries = 13,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<BalanceByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<BalanceByAgeDetail> { BalanceByAgeDetail.ResponseExample() }
            }
        };
    }
}
