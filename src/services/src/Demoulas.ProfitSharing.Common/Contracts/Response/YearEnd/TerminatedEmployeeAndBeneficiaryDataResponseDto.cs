namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record TerminatedEmployeeAndBeneficiaryDataResponseDto
{
    public required int BadgeNumber { get; set; }
    public required short PsnSuffix { get; set; }
    public required string? Name { get; set; }
    public bool IsExecutive { get; init; }

    public string BadgePSn
    {
        get
        {
            if (PsnSuffix == 0)
            {
                return BadgeNumber.ToString();
            }
            return $"{BadgeNumber}{PsnSuffix}";
        }
    }

    public List<TerminatedEmployeeAndBeneficiaryYearDetailDto> YearDetails { get; set; } = new();

    public static TerminatedEmployeeAndBeneficiaryDataResponseDto ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryDataResponseDto
        {
            BadgeNumber = 777123,
            PsnSuffix = 100,
            Name = "Example, Joe F",
            YearDetails = new List<TerminatedEmployeeAndBeneficiaryYearDetailDto>
            {
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
                    EnrollmentCode = 4,
                }
            }
        };
    }
}

public sealed record TerminatedEmployeeAndBeneficiaryYearDetailDto
{
    public short ProfitYear { get; set; }
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
    public byte? EnrollmentCode { get; set; }
    public decimal? SuggestedForfeit { get; set; }
    public bool IsExecutive { get; set; }
}
