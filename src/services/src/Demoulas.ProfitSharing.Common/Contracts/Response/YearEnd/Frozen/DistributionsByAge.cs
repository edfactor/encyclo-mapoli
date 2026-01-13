using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record DistributionsByAge : ReportResponseBase<DistributionsByAgeDetail>
{
    public DistributionsByAge()
    {
        ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; set; }

    public ushort HardshipTotalEmployees { get; set; }
    public decimal RegularTotalAmount { get; set; }
    public ushort RegularTotalEmployees { get; set; }
    public decimal HardshipTotalAmount { get; set; }
    public ushort TotalEmployees { get; init; }
    public ushort BothHardshipAndRegularEmployees { get; init; }


    public decimal DistributionTotalAmount
    {
        get
        {
            return RegularTotalAmount + HardshipTotalAmount;
        }
    }

    public decimal BothHardshipAndRegularAmount { get; set; }


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
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<DistributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<DistributionsByAgeDetail> { DistributionsByAgeDetail.ResponseExample() }
            }
        };
    }
}
