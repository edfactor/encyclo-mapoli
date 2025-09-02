using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record ForfeituresByAge : ReportResponseBase<ForfeituresByAgeDetail>
{
    public ForfeituresByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; init; }

    public required short TotalEmployees { get; init; }
    public required decimal TotalAmount { get; init; }
    


    public static ForfeituresByAge ResponseExample()
    {
        return new ForfeituresByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            TotalAmount = (decimal)1_855_156.09,
            TotalEmployees = 63,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<ForfeituresByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ForfeituresByAgeDetail> { ForfeituresByAgeDetail.ResponseExample() }
            }
        };
    }
}
