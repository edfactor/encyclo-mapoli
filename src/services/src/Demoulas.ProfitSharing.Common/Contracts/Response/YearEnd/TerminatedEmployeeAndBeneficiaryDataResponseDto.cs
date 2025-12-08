using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record TerminatedEmployeeAndBeneficiaryDataResponseDto : IIsExecutive
{
    public required long PSN { get; set; }

    [MaskSensitive] public required string? Name { get; set; }

    public List<TerminatedEmployeeAndBeneficiaryYearDetailDto> YearDetails { get; set; } = [];

    public bool IsExecutive { get; set; }

    /// <summary>
    /// YTD Profit Sharing Hours from the first YearDetail, used for sorting.
    /// </summary>
    public decimal YtdPsHours { get; set; }

    public static TerminatedEmployeeAndBeneficiaryDataResponseDto ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryDataResponseDto
        {
            PSN = 7771230100, // ie. Bene or Empl
            Name = "Example, Joe F",
            YtdPsHours = 980,
            YearDetails =
            [
                new TerminatedEmployeeAndBeneficiaryYearDetailDto
                {
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
