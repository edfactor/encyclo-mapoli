using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Report;

public enum YearEndProfitSharingReportId : byte
{
    [Description("AGE 18-20 WITH >= 1000 PS HOURS")]
    Age18To20With1000Hours = 1,
    [Description(">= AGE 21 WITH >= 1000 PS HOURS")]
    Age21OrOlderWith1000Hours = 2,
    [Description("< AGE 18")]
    Under18 = 3,
    [Description(">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT")]
    Age18OrOlderWithLessThan1000HoursAndPriorAmount = 4,
    [Description(">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT")]
    Age18OrOlderWithLessThan1000HoursAndNoPriorAmount = 5,
    [Description(">= AGE 18 WITH >= 1000 PS HOURS (TERMINATED)")]
    TerminatedAge18OrOlderWith1000Hours = 6,
    [Description(">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT (TERMINATED)")]
    TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount = 7,
    [Description(">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT (TERMINATED)")]
    TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount = 8,
    [Description("NON-EMPLOYEE BENEFICIARIES")]
    NonEmployeeBeneficiaries = 10,
    [Description("< AGE 18 WAGES > 0 (TERMINATED)")]
    TerminatedUnder18WagesGreaterThanZero = 11,
    [Description("EMPLOYEES WITH 0 WAGES, POSITIVE BALANCE ")]
    EmployeesWithZeroWagesPositiveBalance = 12
}
