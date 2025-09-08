namespace YEMatch.YEMatch.SmartActivities;

public class Pay426NCriteria
{
    internal static readonly Dictionary<string, Pay426NCriteria> _reportCriteria = new()
    {
        ["PAY426N-01"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-01",
                Title = "ALL ACTIVE/INACTIVE EMPLOYEES AGE 18-20 WITH >= 1000 PS HOURS",
                IsYearEnd = true,
                MinimumAgeInclusive = 17,
                MaximumAgeInclusive = 20,
                MinimumHoursInclusive = 999.9m,
                MaximumHoursInclusive = 4000m,
                IncludeActiveEmployees = true,
                IncludeInactiveEmployees = true,
                IncludeEmployeesTerminatedThisYear = false,
                IncludeTerminatedEmployees = false,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = true,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
                ProfitYear = 2024
            },
        ["PAY426N-02"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-02",
                Title = "ALL ACTIVE/INACTIVE EMPLOYEES >= AGE 21 WITH >= 1000 PS HOURS",
                IsYearEnd = true,
                MinimumAgeInclusive = 21,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 999.9m,
                MaximumHoursInclusive = 4000m,
                IncludeActiveEmployees = true,
                IncludeInactiveEmployees = true,
                IncludeEmployeesTerminatedThisYear = false,
                IncludeTerminatedEmployees = false,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = true,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
                ProfitYear = 2024
            },
        ["PAY426N-03"] = new Pay426NCriteria
        {
            ReportId = "PAY426N-03",
            Title = "ALL ACTIVE/INACTIVE EMPLOYEES < AGE 18",
            IsYearEnd = true,
            MinimumAgeInclusive = 0,
            MaximumAgeInclusive = 17,
            MinimumHoursInclusive = 0m,
            MaximumHoursInclusive = 4000m,
            IncludeActiveEmployees = true,
            IncludeInactiveEmployees = true,
            IncludeEmployeesTerminatedThisYear = false,
            IncludeTerminatedEmployees = false,
            IncludeBeneficiaries = false,
            IncludeEmployeesWithPriorProfitSharingAmounts = true,
            IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
            ProfitYear = 2024
        },
        ["PAY426N-04"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-04",
                Title = "ALL ACTIVE/INACTIVE EMPLOYEES >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                IsYearEnd = true,
                MinimumAgeInclusive = 18,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 0m,
                MaximumHoursInclusive = 999.99m,
                IncludeActiveEmployees = true,
                IncludeInactiveEmployees = true,
                IncludeEmployeesTerminatedThisYear = false,
                IncludeTerminatedEmployees = false,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = true,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = false,
                ProfitYear = 2024
            },
        ["PAY426N-05"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-05",
                Title = "ALL ACTIVE/INACTIVE EMPLOYEES >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                IsYearEnd = true,
                MinimumAgeInclusive = 18,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 0m,
                MaximumHoursInclusive = 999.99m,
                IncludeActiveEmployees = true,
                IncludeInactiveEmployees = true,
                IncludeEmployeesTerminatedThisYear = false,
                IncludeTerminatedEmployees = false,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = false,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
                ProfitYear = 2024
            },
        ["PAY426N-06"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-06",
                Title = "ALL TERMINATED EMPLOYEES >= AGE 18 WITH >= 1000 PS HOURS",
                IsYearEnd = true,
                MinimumAgeInclusive = 18,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 1000m,
                MaximumHoursInclusive = 4000m,
                IncludeActiveEmployees = false,
                IncludeInactiveEmployees = false,
                IncludeEmployeesTerminatedThisYear = true,
                IncludeTerminatedEmployees = true,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = true,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
                ProfitYear = 2024
            },
        ["PAY426N-07"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-07",
                Title = "ALL TERMINATED EMPLOYEES >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                IsYearEnd = true,
                MinimumAgeInclusive = 18,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 0m,
                MaximumHoursInclusive = 999.99m,
                IncludeActiveEmployees = false,
                IncludeInactiveEmployees = false,
                IncludeEmployeesTerminatedThisYear = true,
                IncludeTerminatedEmployees = true,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = false,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = true,
                ProfitYear = 2024
            },
        ["PAY426N-08"] =
            new Pay426NCriteria
            {
                ReportId = "PAY426N-08",
                Title = "ALL TERMINATED EMPLOYEES >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                IsYearEnd = true,
                MinimumAgeInclusive = 18,
                MaximumAgeInclusive = 200,
                MinimumHoursInclusive = 0m,
                MaximumHoursInclusive = 999.99m,
                IncludeActiveEmployees = false,
                IncludeInactiveEmployees = false,
                IncludeEmployeesTerminatedThisYear = true,
                IncludeTerminatedEmployees = true,
                IncludeBeneficiaries = false,
                IncludeEmployeesWithPriorProfitSharingAmounts = true,
                IncludeEmployeesWithNoPriorProfitSharingAmounts = false,
                ProfitYear = 2024
            },
        ["PAY426N-10"] = new Pay426NCriteria
        {
            ReportId = "PAY426N-10",
            Title = "ALL NON-EMPLOYEE BENEFICIARIES",
            IsYearEnd = true,
            MinimumAgeInclusive = 0,
            MaximumAgeInclusive = 200,
            MinimumHoursInclusive = 0m,
            MaximumHoursInclusive = 4000m,
            IncludeActiveEmployees = false,
            IncludeInactiveEmployees = false,
            IncludeEmployeesTerminatedThisYear = false,
            IncludeTerminatedEmployees = false,
            IncludeBeneficiaries = true,
            IncludeEmployeesWithPriorProfitSharingAmounts = false,
            IncludeEmployeesWithNoPriorProfitSharingAmounts = false,
            ProfitYear = 2024
        }
    };

    public string ReportId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsYearEnd { get; set; }
    public int MinimumAgeInclusive { get; set; }
    public int MaximumAgeInclusive { get; set; }
    public decimal MinimumHoursInclusive { get; set; }
    public decimal MaximumHoursInclusive { get; set; }
    public bool IncludeActiveEmployees { get; set; }
    public bool IncludeInactiveEmployees { get; set; }
    public bool IncludeEmployeesTerminatedThisYear { get; set; }
    public bool IncludeTerminatedEmployees { get; set; }
    public bool IncludeBeneficiaries { get; set; }
    public bool IncludeEmployeesWithPriorProfitSharingAmounts { get; set; }
    public bool IncludeEmployeesWithNoPriorProfitSharingAmounts { get; set; }
    public int ProfitYear { get; set; }
}
