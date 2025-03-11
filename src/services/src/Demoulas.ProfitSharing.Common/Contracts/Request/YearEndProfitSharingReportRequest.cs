using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearEndProfitSharingReportRequest:ProfitYearRequest
{
    [DefaultValue(true)]
    public bool IsYearEnd { get; set; }

    [DefaultValue(18)]
    public short? MinimumAgeInclusive { get; set; }

    [DefaultValue(100)]
    public short? MaximumAgeInclusive { get; set; }
    public decimal? MinimumHoursInclusive { get; set; }
    public decimal? MaximumHoursInclusive { get; set; }
    [DefaultValue(true)]
    public bool IncludeActiveEmployees { get; set; } = true;
    [DefaultValue(true)]
    public bool IncludeInactiveEmployees { get; set; } = true;
    [DefaultValue(false)]
    public bool IncludeEmployeesTerminatedThisYear { get; set; }
    [DefaultValue(false)]
    public bool IncludeTerminatedEmployees { get; set; }
    [DefaultValue(false)]
    public bool IncludeBeneficiaries { get; set; }
    [DefaultValue(true)]
    public bool IncludeEmployeesWithPriorProfitSharingAmounts { get; set; } = true;
    [DefaultValue(true)]
    public bool IncludeEmployeesWithNoPriorProfitSharingAmounts { get; set; } = true;
    [DefaultValue(true)]
    public bool IncludeTotals { get; set; } = true;
    [DefaultValue(true)]
    public bool IncludeDetails { get; set; } = true;
}
