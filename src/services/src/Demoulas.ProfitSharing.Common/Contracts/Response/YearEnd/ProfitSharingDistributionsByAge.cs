using System.Collections.Frozen;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ProfitSharingDistributionsByAge : ReportResponseBase<ProfitSharingDistributionsByAgeDetail>
{
    public ProfitSharingDistributionsByAge()
    {
        ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public short TotalEmployees { get; set; }
    public short HardshipTotalEmployees { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal HardshipTotalAmount { get; set; }
    public decimal DistributionTotalAmount { get; set; }


    public short FullTimeTotalEmployees { get; set; }
    public short FullTimeHardshipTotalEmployees { get; set; }
    public decimal FullTimeTotalAmount { get; set; }
    public decimal FullTimeHardshipTotalAmount { get; set; }
    public decimal FullTimeDistributionTotalAmount { get; set; }

    public short PartTimeTotalEmployees { get; set; }
    public short PartTimeHardshipTotalEmployees { get; set; }
    public decimal PartTimeTotalAmount { get; set; }
    public decimal PartTimeHardshipTotalAmount { get; set; }
    public decimal PartTimeDistributionTotalAmount { get; set; }

    public IEnumerable<ProfitSharingDistributionsByAgeDetail> FullTimeResults { get; init; } = FrozenSet<ProfitSharingDistributionsByAgeDetail>.Empty;
    public IEnumerable<ProfitSharingDistributionsByAgeDetail> PartTimeResults { get; init; } = FrozenSet<ProfitSharingDistributionsByAgeDetail>.Empty;


    public static ProfitSharingDistributionsByAge ResponseExample()
    {
        return new ProfitSharingDistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            TotalEmployees = 93,
            HardshipTotalEmployees = 18,
            TotalAmount = (decimal)1_855_156.09,
            HardshipTotalAmount = (decimal)386_243.46,
            DistributionTotalAmount = 0,
            
            Response = new PaginatedResponseDto<ProfitSharingDistributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() }
            },
            FullTimeResults = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() },
            PartTimeResults = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() }
        };
    }
}
