namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record TerminatedEmployeeAndBeneficiaryDataResponseDto
{
    public required string BadgePSn { get; set; }
    public required string Name { get; set; }
    public required decimal BeginningBalance { get; set; }
    public required decimal BeneficiaryAllocation { get; set; }
    public required decimal DistributionAmount { get; set; }
    public required decimal Forfeit { get; set; }
    public required decimal EndingBalance { get; set; }
    public required decimal VestedBalance { get; set; }
    public required DateOnly? DateTerm { get; set; }
    public required decimal YtdPsHours { get; set; }
    public required int VestedPercent { get; set; }
    public required int? Age { get; set; }
    public required byte? EnrollmentCode { get; set; }

    public static readonly TerminatedEmployeeAndBeneficiaryDataResponseDto Example = new()
    {
        BadgePSn = "777",
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
        EnrollmentCode = 4
    };

}
