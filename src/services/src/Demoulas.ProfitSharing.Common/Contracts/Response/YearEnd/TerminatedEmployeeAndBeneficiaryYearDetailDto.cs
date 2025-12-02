using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record TerminatedEmployeeAndBeneficiaryYearDetailDto : IIsExecutive
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
    [MaskSensitive] public required short? Age { get; set; }
    public bool HasForfeited { get; set; }
    public decimal? SuggestedForfeit { get; set; }
    public bool IsExecutive { get; set; }
    public byte EnrollmentId { get; set; }
}
