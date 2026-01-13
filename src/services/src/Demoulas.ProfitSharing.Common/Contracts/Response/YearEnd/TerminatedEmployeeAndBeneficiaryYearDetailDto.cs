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

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static TerminatedEmployeeAndBeneficiaryYearDetailDto ResponseExample()
    {
        return new TerminatedEmployeeAndBeneficiaryYearDetailDto
        {
            BeginningBalance = 50000.00m,
            BeneficiaryAllocation = 0.00m,
            DistributionAmount = 25000.00m,
            Forfeit = 5000.00m,
            EndingBalance = 20000.00m,
            VestedBalance = 18000.00m,
            DateTerm = null,
            YtdPsHours = 1500.00m,
            VestedPercent = 90.00m,
            Age = 55,
            HasForfeited = false,
            SuggestedForfeit = 0.00m,
            IsExecutive = false,
            EnrollmentId = 3
        };
    }
}
