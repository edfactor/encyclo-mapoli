using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Calculates year-end changes for employee records.
/// Implements COBOL PAY426.cbl logic for computing IsNew, ZeroCont, EarnPoints, and PsCertificateIssuedDate.
/// </summary>
internal static class YearEndChangeCalculator
{
    /// <summary>
    /// Calculates the Year End Change for a single employee.
    /// Very closely follows PAY426.cbl, 405-calculate-points
    /// </summary>
    public static YearEndChange ComputeChange(
        short profitYear,
        short? firstContributionYear,
        short age,
        decimal currentBalance,
        PayProfitDto employee,
        DateOnly fiscalEnd)
    {
        // Early exit for under 21
        if (age < ReferenceData.MinimumAgeForContribution)
        {
            return ComputeChangeForUnder21(firstContributionYear, age);
        }

        // Handle terminated employees
        if (IsTerminatedBeforeFiscalEnd(employee, fiscalEnd))
        {
            return ComputeChangeForTerminated(employee, age, profitYear, firstContributionYear, currentBalance);
        }

        // Handle age 64+ employees
        if (age >= (ReferenceData.RetirementAge - 1))
        {
            return ComputeChangeForAge64Plus(employee, age, profitYear, firstContributionYear, currentBalance, fiscalEnd);
        }

        // Handle active employees under age 64
        return ComputeChangeForActiveEmployeeUnder64(employee, age, firstContributionYear);
    }

    private static YearEndChange ComputeChangeForUnder21(short? firstContributionYear, short age)
    {
        int isNew = DetermineIsNewEmployee(firstContributionYear, age);
        byte zeroCont = ZeroContributionReason.Constants.Under21WithOver1Khours;

        return new YearEndChange
        {
            IsNew = isNew,
            ZeroCont = zeroCont,
            EarnPoints = 0,
            PsCertificateIssuedDate = null
        };
    }

    private static YearEndChange ComputeChangeForTerminated(
        PayProfitDto employee,
        short age,
        short profitYear,
        short? firstContributionYear,
        decimal currentBalance)
    {
        byte zeroCont = HasMinimumHours(employee)
            ? ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested
            : ZeroContributionReason.Constants.Normal;

        // Terminated employees under 64 return immediately
        if (age < (ReferenceData.RetirementAge - 1))
        {
            return new YearEndChange
            {
                IsNew = EmployeeType.Constants.NotNewLastYear,
                ZeroCont = zeroCont,
                EarnPoints = 0,
                PsCertificateIssuedDate = null
            };
        }

        // Terminated employees age 64+ may get vesting codes 6/7
        zeroCont = DetermineZeroContributionReasonForAge64Plus(employee, age, profitYear, firstContributionYear, currentBalance, zeroCont);

        return new YearEndChange
        {
            IsNew = EmployeeType.Constants.NotNewLastYear,
            ZeroCont = zeroCont,
            EarnPoints = 0,
            PsCertificateIssuedDate = null
        };
    }

    private static YearEndChange ComputeChangeForActiveEmployeeUnder64(
        PayProfitDto employee,
        short age,
        short? firstContributionYear)
    {
        int isNew = DetermineIsNewEmployee(firstContributionYear, age);
        decimal points = CalculateEarnPoints(employee);
        DateOnly? certDate = points > 0 ? DateOnly.FromDateTime(DateTime.Now) : null;

        return new YearEndChange
        {
            IsNew = isNew,
            ZeroCont = ZeroContributionReason.Constants.Normal,
            EarnPoints = points,
            PsCertificateIssuedDate = certDate
        };
    }

    private static YearEndChange ComputeChangeForAge64Plus(
        PayProfitDto employee,
        short age,
        short profitYear,
        short? firstContributionYear,
        decimal currentBalance,
        DateOnly fiscalEnd)
    {
        int isNew = DetermineIsNewEmployee(firstContributionYear, age);

        // COBOL PAY426.cbl lines 1219-1221: Set points to 0 for age 64+ with < 1000 hours
        decimal points = HasMinimumHours(employee) ? CalculateEarnPoints(employee) : 0;
        DateOnly? certDate = points > 0 ? DateOnly.FromDateTime(DateTime.Now) : null;

        byte zeroCont = DetermineZeroContributionReasonForAge64Plus(employee, age, profitYear, firstContributionYear, currentBalance, ZeroContributionReason.Constants.Normal);

        // Set IsNew to 0 for terminated employees
        if (IsTerminatedBeforeFiscalEnd(employee, fiscalEnd))
        {
            isNew = EmployeeType.Constants.NotNewLastYear;
        }

        return new YearEndChange
        {
            IsNew = isNew,
            ZeroCont = zeroCont,
            EarnPoints = points,
            PsCertificateIssuedDate = certDate
        };
    }

    #region Calculation Helpers

    private static int DetermineIsNewEmployee(short? firstContributionYear, short age)
    {
        if (firstContributionYear == null && age >= ReferenceData.MinimumAgeForContribution)
        {
            return EmployeeType.Constants.NewLastYear;
        }
        return EmployeeType.Constants.NotNewLastYear;
    }

    private static decimal CalculateEarnPoints(PayProfitDto employee)
    {
        decimal income = employee.IncomeExecutive + employee.CurrentIncomeYear;
        return Math.Round(income / 100.0m, 0, MidpointRounding.AwayFromZero);
    }

    private static byte DetermineZeroContributionReasonForAge64Plus(
        PayProfitDto employee,
        short age,
        short profitYear,
        short? firstContributionYear,
        decimal currentBalance,
        byte defaultZeroCont)
    {
        // COBOL PAY426.cbl lines 1199-1203: Preserve existing ZeroCont if >= 6
        byte zeroCont = employee.ZeroContributionReasonId >= ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
            ? employee.ZeroContributionReasonId ?? defaultZeroCont
            : defaultZeroCont;

        // Reset if between 3-5 (invalid for age 64+)
        if (zeroCont > ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested
            && zeroCont != ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            zeroCont = ZeroContributionReason.Constants.Normal;
        }

        int yearsSinceFirst = CalculateYearsSinceFirstContribution(profitYear, firstContributionYear, currentBalance);

        // Age 65+ with 5+ years vesting
        if (yearsSinceFirst >= ReferenceData.VestingYears && age >= ReferenceData.RetirementAge)
        {
            return ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested;
        }

        // Age 65+ with 4 years vesting (vesting next year)
        if (yearsSinceFirst == (ReferenceData.VestingYears - 1) && age >= ReferenceData.RetirementAge)
        {
            return ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay;
        }

        // Age 64 with 4+ years vesting
        if (yearsSinceFirst >= (ReferenceData.VestingYears - 1) && age == (ReferenceData.RetirementAge - 1))
        {
            return ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay;
        }

        return zeroCont;
    }

    private static int CalculateYearsSinceFirstContribution(short profitYear, short? firstContributionYear, decimal currentBalance)
    {
        int yearsSince = profitYear - (firstContributionYear ?? profitYear);

        // COBOL PAY426.cbl lines 1273: Bump up years by 1 if employee had money last year
        if (currentBalance > 0)
        {
            yearsSince++;
        }

        return yearsSince;
    }

    #endregion

    #region Employee Status Helpers

    private static bool IsTerminatedBeforeFiscalEnd(PayProfitDto employee, DateOnly fiscalEnd)
    {
        return employee.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated
            && employee.Demographic.TerminationDate < fiscalEnd;
    }

    private static decimal GetTotalHours(PayProfitDto employee)
    {
        return employee.CurrentHoursYear + employee.HoursExecutive;
    }

    private static bool HasMinimumHours(PayProfitDto employee)
    {
        return GetTotalHours(employee) >= 1000;
    }

    #endregion
}
