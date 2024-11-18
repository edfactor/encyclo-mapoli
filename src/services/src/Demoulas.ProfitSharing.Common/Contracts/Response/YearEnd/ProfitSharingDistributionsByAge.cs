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


    public short FullTimeHardshipTotalEmployees { get; set; }
    public decimal FullTimeRegularAmount { get; set; }
    public short FullTimeRegularEmployees { get; set; }
    public decimal FullTimeHardshipTotalAmount { get; set; }

    public decimal FullTimeDistributionTotalAmount
    {
        get
        {
            return FullTimeRegularAmount + FullTimeHardshipTotalAmount;
        }
    }

    public short PartTimeRegularEmployees { get; set; }
    public decimal PartTimeRegularAmount { get; set; }
    public short PartTimeHardshipTotalEmployees { get; set; }
    public decimal PartTimeHardshipTotalAmount { get; set; }
    public decimal PartTimeDistributionTotalAmount
    {
        get
        {
            return PartTimeRegularAmount + PartTimeHardshipTotalAmount;
        }
    }

    public IEnumerable<ProfitSharingDistributionsByAgeDetail> TotalResults { get; init; } = FrozenSet<ProfitSharingDistributionsByAgeDetail>.Empty;
    public IEnumerable<ProfitSharingDistributionsByAgeDetail> FullTimeResults { get; init; } = FrozenSet<ProfitSharingDistributionsByAgeDetail>.Empty;
    public IEnumerable<ProfitSharingDistributionsByAgeDetail> PartTimeResults { get; init; } = FrozenSet<ProfitSharingDistributionsByAgeDetail>.Empty;
   


    public static ProfitSharingDistributionsByAge ResponseExample()
    {
        return new ProfitSharingDistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            RegularTotalEmployees = 93,
            HardshipTotalEmployees = 18,
            RegularTotalAmount = (decimal)1_855_156.09,
            HardshipTotalAmount = (decimal)386_243.46,
            
            Response = new PaginatedResponseDto<ProfitSharingDistributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() }
            },
            FullTimeResults = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() },
            PartTimeResults = new List<ProfitSharingDistributionsByAgeDetail> { ProfitSharingDistributionsByAgeDetail.ResponseExample() }
        };
    }
}
