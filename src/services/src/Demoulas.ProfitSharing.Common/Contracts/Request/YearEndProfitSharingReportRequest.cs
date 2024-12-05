namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearEndProfitSharingReportRequest:ProfitYearRequest
{
    public bool IsYearEnd { get; set; }
    public short? MinimumAgeInclusive { get; set; }
    public short? MaximumAgeInclusive { get; set; }
    public decimal? MinimumHoursInclusive { get; set; }
    public decimal? MaximumHoursInclusive { get; set; }
    public bool IncludeActiveEmployees { get; set; } = true;
    public bool IncludeInactiveEmployees { get; set; } = true;
    public bool IncludeEmployeesTerminatedThisYear { get; set; }
    public bool IncludeTerminatedEmployees { get; set; }
    public bool IncludeBeneficiaries { get; set; } 
    public bool IncludeEmployeesWithPriorProfitSharingAmounts { get; set; } = true;
    public bool IncludeEmployeesWithNoPriorProfitSharingAmounts { get; set; } = true;
}
