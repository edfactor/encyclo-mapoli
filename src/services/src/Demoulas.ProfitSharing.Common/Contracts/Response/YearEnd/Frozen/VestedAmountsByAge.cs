using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record VestedAmountsByAge : ReportResponseBase<VestedAmountsByAgeDetail>
{
    public decimal TotalFullTime100PercentAmount { get; set; }
    public decimal TotalFullTimePartialAmount { get; set; }
    public decimal TotalFullTimeNotVestedAmount { get; set; }
    public decimal TotalPartTime100PercentAmount { get; set; }
    public decimal TotalPartTimePartialAmount { get; set; }
    public decimal TotalPartTimeNotVestedAmount { get; set; }
    public ushort TotalBeneficiaryCount { get; set; }
    public decimal TotalBeneficiaryAmount { get; set; }
    public ushort TotalFullTimeCount { get; set; }
    public ushort TotalNotVestedCount { get; set; }
    public ushort TotalPartialVestedCount { get; set; }


    public static VestedAmountsByAge ResponseExample()
    {
        return new VestedAmountsByAge
        {
            ReportName = "VESTED AMOUNTS BY AGE",
            ReportDate = DateTimeOffset.Now,
            TotalFullTime100PercentAmount = 1_200_000.00m,
            TotalFullTimePartialAmount = 300_000.00m,
            TotalFullTimeNotVestedAmount = 200_000.00m,
            TotalPartTime100PercentAmount = 150_000.00m,
            TotalFullTimeCount = 50,
            TotalNotVestedCount = 20,
            TotalPartialVestedCount = 15,
            TotalBeneficiaryCount = 10,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<VestedAmountsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<VestedAmountsByAgeDetail>
                {
                    new VestedAmountsByAgeDetail
                    {
                        Age = 30,
                        FullTime100PercentCount = 25,
                        FullTime100PercentAmount = 600_000.00m,
                        FullTimePartialCount = 5,
                        FullTimePartialAmount = 50_000.00m,
                        FullTimeNotVestedCount = 10,
                        FullTimeNotVestedAmount = 100_000.00m,

                        PartTime100PercentCount = 10,
                        PartTime100PercentAmount = 50_000.00m,
                        PartTimePartialCount = 5,
                        PartTimePartialAmount = 20_000.00m,
                        PartTimeNotVestedCount = 3,
                        PartTimeNotVestedAmount = 15_000.00m,

                        BeneficiaryCount = 2,
                        BeneficiaryAmount = 25_000.00m
                    },
                    new VestedAmountsByAgeDetail
                    {
                        Age = 40,
                        FullTime100PercentCount = 30,
                        FullTime100PercentAmount = 900_000.00m,
                        FullTimePartialCount = 10,
                        FullTimePartialAmount = 150_000.00m,
                        FullTimeNotVestedCount = 5,
                        FullTimeNotVestedAmount = 50_000.00m,

                        PartTime100PercentCount = 5,
                        PartTime100PercentAmount = 30_000.00m,
                        PartTimePartialCount = 2,
                        PartTimePartialAmount = 10_000.00m,
                        PartTimeNotVestedCount = 5,
                        PartTimeNotVestedAmount = 20_000.00m,

                        BeneficiaryCount = 5,
                        BeneficiaryAmount = 50_000.00m
                    }
                }
            }
        };
    }
}
