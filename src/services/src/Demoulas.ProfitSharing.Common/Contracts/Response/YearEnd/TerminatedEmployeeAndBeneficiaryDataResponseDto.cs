using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record TerminatedEmployeeAndBeneficiaryDataResponseDto : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public required short PsnSuffix { get; set; }

    [MaskSensitive] public required string? Name { get; set; }

    public string BadgePSn
    {
        get
        {
            if (PsnSuffix == 0)
            {
                return BadgeNumber.ToString();
            }
            return $"{BadgeNumber}{PsnSuffix:0000}";
        }
    }

    public List<TerminatedEmployeeAndBeneficiaryYearDetailDto> YearDetails { get; set; } = [];
    public bool IsExecutive { get; set; }

    public static TerminatedEmployeeAndBeneficiaryDataResponseDto ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryDataResponseDto
        {
            BadgeNumber = 777123,
            PsnSuffix = 100,
            Name = "Example, Joe F",
            YearDetails =
            [
                new TerminatedEmployeeAndBeneficiaryYearDetailDto
                {
                    ProfitYear = 2024,
                    BeginningBalance = 100,
                    BeneficiaryAllocation = 200,
                    DistributionAmount = 300,
                    Forfeit = 400,
                    EndingBalance = 500,
                    VestedBalance = 600,
                    DateTerm = null,
                    YtdPsHours = 980,
                    VestedPercent = 20,
                    Age = 44,
                    HasForfeited = false
                }
            ]
        };
    }
}

public sealed record TerminatedEmployeeAndBeneficiaryYearDetailDto : IIsExecutive, IProfitYearRequest
{
    public decimal BeginningBalance { get; set; }
    public decimal BeneficiaryAllocation { get; set; }
    public decimal DistributionAmount { get; set; }
    public decimal Forfeit { get; set; }
    public decimal EndingBalance { get; set; }
    public decimal VestedBalance { get; set; }
    public DateOnly? DateTerm { get; set; }
    public decimal YtdPsHours { get; set; }
    public decimal VestedPercent { get; set; }
    public int? Age { get; set; }
    public bool HasForfeited { get; set; }
    public decimal? SuggestedForfeit { get; set; }
    public bool IsExecutive { get; set; }
    public short ProfitYear { get; set; }
}
