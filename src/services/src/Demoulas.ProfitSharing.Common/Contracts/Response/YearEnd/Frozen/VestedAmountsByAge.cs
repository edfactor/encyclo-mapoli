using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record VestedAmountsByAge : ReportResponseBase<VestedAmountsByAgeDetail>
{
    public FrozenReportsByAgeRequest.Report ReportType { get; set; }
    public decimal TotalFullTimeAmount { get; set; }
    public decimal TotalNotVestedAmount { get; set; }
    public decimal TotalPartialVestedAmount { get; set; }
    public decimal TotalBeneficiaryAmount { get; set; }
    public short TotalFullTimeCount { get; set; }
    public short TotalNotVestedCount { get; set; }
    public short TotalPartialVestedCount { get; set; }
    public short TotalBeneficiaryCount { get; set; }

    public static VestedAmountsByAge ResponseExample()
    {
        return new VestedAmountsByAge
        {
            ReportName = "VESTED AMOUNTS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = FrozenReportsByAgeRequest.Report.Total,
            TotalFullTimeAmount = 1_200_000.00m,
            TotalNotVestedAmount = 300_000.00m,
            TotalPartialVestedAmount = 200_000.00m,
            TotalBeneficiaryAmount = 150_000.00m,
            TotalFullTimeCount = 50,
            TotalNotVestedCount = 20,
            TotalPartialVestedCount = 15,
            TotalBeneficiaryCount = 10,

            Response = new PaginatedResponseDto<VestedAmountsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<VestedAmountsByAgeDetail>
                {
                    new VestedAmountsByAgeDetail
                    {
                        Age = 30,
                        FullTimeCount = 20,
                        FullTimeAmount = 500_000.00m,
                        NotVestedCount = 5,
                        NotVestedAmount = 100_000.00m,
                        PartialVestedCount = 3,
                        PartialVestedAmount = 50_000.00m,
                        BeneficiaryCount = 2,
                        BeneficiaryAmount = 25_000.00m
                    },
                    new VestedAmountsByAgeDetail
                    {
                        Age = 40,
                        FullTimeCount = 30,
                        FullTimeAmount = 700_000.00m,
                        NotVestedCount = 15,
                        NotVestedAmount = 200_000.00m,
                        PartialVestedCount = 12,
                        PartialVestedAmount = 150_000.00m,
                        BeneficiaryCount = 8,
                        BeneficiaryAmount = 125_000.00m
                    }
                }
            }
        };
    }
}
