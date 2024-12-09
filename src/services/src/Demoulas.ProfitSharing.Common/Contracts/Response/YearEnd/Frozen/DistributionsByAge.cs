using System.Collections.Frozen;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record DistributionsByAge : ReportResponseBase<DistributionsByAgeDetail>
{
    public DistributionsByAge()
    {
        ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; set; }

    public short HardshipTotalEmployees { get; set; }
    public decimal RegularTotalAmount { get; set; }
    public short RegularTotalEmployees { get; set; }
    public decimal HardshipTotalAmount { get; set; }

    public decimal DistributionTotalAmount
    {
        get
        {
            return RegularTotalAmount + HardshipTotalAmount;
        }
    }





    public static DistributionsByAge ResponseExample()
    {
        return new DistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            RegularTotalEmployees = 93,
            HardshipTotalEmployees = 18,
            RegularTotalAmount = (decimal)1_855_156.09,
            HardshipTotalAmount = (decimal)386_243.46,

            Response = new PaginatedResponseDto<DistributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<DistributionsByAgeDetail> { DistributionsByAgeDetail.ResponseExample() }
            }
        };
    }
}
