using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record BalanceByYears : BalanceByBase<BalanceByYearsDetail>
{
    public BalanceByYears()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY YEARS";
        ReportDate = DateTimeOffset.Now;
    }

    public static BalanceByYears ResponseExample()
    {
        return new BalanceByYears
        {
            ReportName = "PROFIT SHARING FORFEITURES BY YEARS",
            ReportDate = DateTimeOffset.Now,
            BalanceTotalAmount = (decimal)1_855_156.09,
            TotalMembers = 63,
            TotalBeneficiaries = 13,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<BalanceByYearsDetail>(new PaginationRequestDto())
            {
                Results = new List<BalanceByYearsDetail> { BalanceByYearsDetail.ResponseExample() }
            }
        };
    }
}
