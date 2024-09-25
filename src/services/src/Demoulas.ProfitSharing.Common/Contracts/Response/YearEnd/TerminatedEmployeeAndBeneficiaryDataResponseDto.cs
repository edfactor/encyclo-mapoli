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
}
