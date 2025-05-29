namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record TerminatedEmployeeAndBeneficiaryDataResponseDto
{
    public short ProfitYear { get; set; }
    public required int BadgeNumber { get; set; }
    public required short PsnSuffix { get; set; }

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
    public required string? Name { get; set; }
    public required decimal BeginningBalance { get; set; }
    public required decimal BeneficiaryAllocation { get; set; }
    public required decimal DistributionAmount { get; set; }
    public required decimal Forfeit { get; set; }
    public required decimal EndingBalance { get; set; }
    public required decimal VestedBalance { get; set; }
    public required DateOnly? DateTerm { get; set; }
    public required decimal YtdPsHours { get; set; }
    public required decimal VestedPercent { get; set; }
    public required int? Age { get; set; }
    public required byte? EnrollmentCode { get; set; }

    public static TerminatedEmployeeAndBeneficiaryDataResponseDto ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryDataResponseDto
        {
            BadgeNumber = 777123,
            PsnSuffix = 100,
            Name = "Example, Joe F",
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
        };
    }

}
