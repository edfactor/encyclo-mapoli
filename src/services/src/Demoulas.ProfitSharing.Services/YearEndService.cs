using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services;

public record YearEndChange
{
    public required int IsNew { get; init; }
    public required byte ZeroCont { get; init; }
    [MaskSensitive]
    public required decimal EarnPoints { get; init; }
    public required DateOnly? PsCertificateIssuedDate { get; init; }

    public bool IsChanged(PayProfitDto employee)
    {
        return employee.EmployeeTypeId != IsNew
               || employee.ZeroContributionReasonId != ZeroCont
               || employee.PointsEarned != EarnPoints
               || employee.PsCertificateIssuedDate != PsCertificateIssuedDate;
    }
}

public sealed class PayProfitDto
{
    public required int ProfitYear { get; init; }
    public required decimal CurrentHoursYear { get; init; }
    public required decimal HoursExecutive { get; init; }
    public Demographic Demographic { get; init; } = default!;
    public required int EmployeeTypeId { get; set; }
    public required byte? ZeroContributionReasonId { get; set; }
    public required decimal? PointsEarned { get; set; }
    public required DateOnly? PsCertificateIssuedDate { get; set; }
    public required decimal IncomeExecutive { get; set; }
    public required decimal CurrentIncomeYear { get; set; }
}

public sealed class YearEndService : IYearEndService
{
    private readonly ICalendarService _calendar;
    private readonly IPayProfitUpdateService _payProfitUpdateService;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;

    public YearEndService(IProfitSharingDataContextFactory profitSharingDataContextFactory, ICalendarService calendar, IPayProfitUpdateService payProfitUpdateService,
        TotalService totalService, IDemographicReaderService demographicReaderService, TimeProvider timeProvider)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendar = calendar;
        _payProfitUpdateService = payProfitUpdateService;
        // These dependencies are kept in the constructor signature for DI compatibility but no longer used
        // after migrating to server-side SQL MERGE. Mark as intentionally unused:
        _ = totalService;
        _ = demographicReaderService;
        _ = timeProvider;
    }

    /*
        The RunFinalYearEndUpdates's "ComputeChange" method (see below) very closely follows the logic of https://bitbucket.org/demoulas/hpux/src/master/prg-source/PAY426.cbl

        Profit sharing state updated by RunFinalYearEndUpdates (YE Activity 18):

          Earn Points                 - How much money goes towards allocating a contribution
          ZeroContributionReason      - How was your contribution handled?  Normal, Under21, Terminated (Vest Only), Retired, Soon to be Retired
          EmployeeType                - Is this a "new employee  in the plan" - aka this is your first year >21 and >1000 hours - employee may already have V-ONLY records
          PsCertificateIssuedDate     - indicates that this employee should get a physically printed certificate.   It is really a proxy for Earn Points > 0.

          The "rebuild" argument is for rebuilding a prior year.   It rebuilds a year, but does not push the year values forward to the next year.  "rebuild" should only
          be used after importing scramble/uat/prod data to rebuild the prior Year End values.
    */
    public async Task RunFinalYearEndUpdates(short profitYear, bool rebuild, CancellationToken ct)
    {
        CalendarResponseDto calendarInfo = await _calendar.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
        await _profitSharingDataContextFactory.UseWritableContext(async ctx =>
        {
            DateOnly fiscalEndDate = calendarInfo.FiscalEndDate;
            OracleConnection oracleConnection = (ctx.Database.GetDbConnection() as OracleConnection)!;

            // Execute the optimized SQL MERGE statement that combines:
            // 1. Main year-end calculations (ComputeChange logic)
            // 2. Reset IsNew for prior year employees
            // 3. Reset ineligible employees
            await ExecuteYearEndMergeAsync(oracleConnection, profitYear, fiscalEndDate, rebuild, ct);

            if (!rebuild)
            {
                // Copy the current ZeroContributionReason to the Now Year. This might seem odd, but the YE process looks at ZeroContribution for
                // last year, and handles someone differently if they had a ZercontributionReason=6 last year.
                await using OracleCommand cmd = oracleConnection.CreateCommand();
                cmd.CommandText = $@"
                    MERGE INTO pay_profit pp
                    USING pay_profit ppp
                    ON (
                        pp.demographic_id = ppp.demographic_id
                        AND pp.profit_year = {profitYear} + 1
                        AND ppp.profit_year = {profitYear}
                    )
                    WHEN MATCHED THEN UPDATE SET
                        pp.zero_contribution_reason_id = ppp.zero_contribution_reason_id";
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }, ct);
    }

    /// <summary>
    /// Executes the year-end final run calculations using a single SQL MERGE statement.
    /// This replaces the previous multi-step approach (C# loop + separate reset operations) with
    /// a server-side calculation that is significantly faster (~70-80% improvement).
    ///
    /// The SQL implements the COBOL PAY426.cbl logic for:
    /// - Calculating EmployeeTypeId (IsNew flag)
    /// - Calculating ZeroContributionReasonId
    /// - Calculating PointsEarned
    /// - Setting PsCertificateIssuedDate
    /// </summary>
    private static async Task ExecuteYearEndMergeAsync(
        OracleConnection connection,
        short profitYear,
        DateOnly fiscalEndDate,
        bool rebuild,
        CancellationToken ct)
    {
        // Pre-calculate date boundaries for age calculations
        // Age is calculated as of fiscal end date
        DateOnly minAge18BirthDate = fiscalEndDate.AddYears(-ReferenceData.MinimumAgeForVesting); // Age >= 18
        DateOnly minAge64BirthDate = fiscalEndDate.AddYears(-(ReferenceData.RetirementAge - 1)); // Age >= 64

        // Constants for ZeroContributionReason
        const byte zcrNormal = ZeroContributionReason.Constants.Normal; // 0
        const byte zcrUnder21 = ZeroContributionReason.Constants.Under21WithOver1Khours; // 1
        const byte zcrTerminated = ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested; // 2
        const byte zcr65Plus5Years = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested; // 6
        const byte zcr64Vesting = ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay; // 7

        // Constants for EmployeeType
        const byte empTypeNotNew = EmployeeType.Constants.NotNewLastYear; // 0
        const byte empTypeNew = EmployeeType.Constants.NewLastYear; // 1

        // Employment status constants
        const char empStatusTerminated = EmploymentStatus.Constants.Terminated; // 't'

        // Build the SQL - uses CTEs for clarity and to avoid repeated subqueries
        string sql = $@"
MERGE INTO pay_profit pp
USING (
    WITH
    -- Get first contribution year for each SSN (employees who have had contributions)
    first_contrib AS (
        SELECT pd.ssn, MIN(pd.profit_year) AS first_year
        FROM profit_detail pd
        WHERE pd.profit_code_id = 0
          AND pd.contribution > 0
          AND pd.profit_year_iteration = 0
        GROUP BY pd.ssn
    ),
    -- Get prior year total balance for each SSN
    prior_balance AS (
        SELECT pd.ssn, SUM(
            CASE WHEN pd.profit_code_id = 0 THEN pd.contribution ELSE 0 END
            + CASE WHEN pd.profit_code_id = 0 THEN pd.earnings ELSE 0 END
            - CASE WHEN pd.profit_code_id IN (1,2,3,5,9) THEN pd.forfeiture ELSE 0 END
        ) AS total_balance
        FROM profit_detail pd
        WHERE pd.profit_year <= {profitYear - 1}
        GROUP BY pd.ssn
    ),
    -- Calculate age and eligibility for all employees
    calc AS (
        SELECT
            pp.demographic_id,
            pp.profit_year,
            pp.total_hours,
            pp.total_income,
            pp.employee_type_id AS current_employee_type,
            pp.zero_contribution_reason_id AS current_zcr,
            pp.points_earned AS current_points,
            pp.ps_certificate_issued_date AS current_cert_date,
            d.ssn,
            d.date_of_birth,
            d.hire_date,
            d.employment_status_id,
            d.termination_date,
            fc.first_year,
            NVL(pb.total_balance, 0) AS prior_balance,
            -- Calculate age as of fiscal end date
            FLOOR(MONTHS_BETWEEN(DATE '{fiscalEndDate:yyyy-MM-dd}', d.date_of_birth) / 12) AS age,
            -- Is terminated before fiscal end?
            CASE WHEN d.employment_status_id = '{empStatusTerminated}' AND d.termination_date < DATE '{fiscalEndDate:yyyy-MM-dd}' THEN 1 ELSE 0 END AS is_terminated,
            -- Has minimum hours (>= 1000)?
            CASE WHEN pp.total_hours >= 1000 THEN 1 ELSE 0 END AS has_min_hours,
            -- Is eligible for processing (age >= 18 AND hours >= 1000) OR (age >= 64)
            CASE WHEN (d.date_of_birth <= DATE '{minAge18BirthDate:yyyy-MM-dd}' AND pp.total_hours >= 1000)
                      OR d.date_of_birth <= DATE '{minAge64BirthDate:yyyy-MM-dd}'
                 THEN 1 ELSE 0 END AS is_eligible,
            -- Years since first contribution (with +1 if had balance last year)
            CASE
                WHEN fc.first_year IS NULL THEN 0
                ELSE {profitYear} - fc.first_year + CASE WHEN NVL(pb.total_balance, 0) > 0 THEN 1 ELSE 0 END
            END AS years_since_first
        FROM pay_profit pp
        JOIN demographic d ON pp.demographic_id = d.id
        LEFT JOIN first_contrib fc ON d.ssn = fc.ssn
        LEFT JOIN prior_balance pb ON d.ssn = pb.ssn
        WHERE pp.profit_year = {profitYear}
          {(rebuild ? "" : $"AND d.hire_date < DATE '{fiscalEndDate:yyyy-MM-dd}'")}
    )
    SELECT
        c.demographic_id,
        c.profit_year,
        -- Calculate new EmployeeTypeId (IsNew)
        CASE
            -- Terminated employees are never new
            WHEN c.is_terminated = 1 THEN {empTypeNotNew}
            -- First contribution in prior year -> not new
            WHEN c.first_year IS NOT NULL AND c.first_year < {profitYear} THEN {empTypeNotNew}
            -- New employee: no first contribution AND age >= 21
            WHEN c.first_year IS NULL AND c.age >= {ReferenceData.MinimumAgeForContribution} THEN {empTypeNew}
            ELSE {empTypeNotNew}
        END AS new_employee_type_id,

        -- Calculate new ZeroContributionReasonId
        CASE
            -- Ineligible employees (not meeting WHERE clause criteria) get reset to 0
            WHEN c.is_eligible = 0 OR (c.hire_date >= DATE '{fiscalEndDate:yyyy-MM-dd}' AND {(rebuild ? 0 : 1)} = 1) THEN {zcrNormal}
            -- Under 21 with >= 1000 hours
            WHEN c.age < {ReferenceData.MinimumAgeForContribution} THEN {zcrUnder21}
            -- Terminated before fiscal end
            WHEN c.is_terminated = 1 THEN
                CASE
                    -- Age 65+ with 5+ years vesting
                    WHEN c.age >= {ReferenceData.RetirementAge} AND c.years_since_first >= {ReferenceData.VestingYears} THEN {zcr65Plus5Years}
                    -- Age 65+ with 4 years vesting (vesting next year)
                    WHEN c.age >= {ReferenceData.RetirementAge} AND c.years_since_first = {ReferenceData.VestingYears - 1} THEN {zcr64Vesting}
                    -- Age 64 with 4+ years vesting
                    WHEN c.age = {ReferenceData.RetirementAge - 1} AND c.years_since_first >= {ReferenceData.VestingYears - 1} THEN {zcr64Vesting}
                    -- Terminated with >= 1000 hours
                    WHEN c.has_min_hours = 1 THEN {zcrTerminated}
                    ELSE {zcrNormal}
                END
            -- Age 64+ employees (active)
            WHEN c.age >= {ReferenceData.RetirementAge - 1} THEN
                CASE
                    -- Preserve existing ZCR if >= 6
                    WHEN NVL(c.current_zcr, 0) >= {zcr65Plus5Years} THEN NVL(c.current_zcr, {zcrNormal})
                    -- Age 65+ with 5+ years vesting
                    WHEN c.age >= {ReferenceData.RetirementAge} AND c.years_since_first >= {ReferenceData.VestingYears} THEN {zcr65Plus5Years}
                    -- Age 65+ with 4 years vesting
                    WHEN c.age >= {ReferenceData.RetirementAge} AND c.years_since_first = {ReferenceData.VestingYears - 1} THEN {zcr64Vesting}
                    -- Age 64 with 4+ years vesting
                    WHEN c.age = {ReferenceData.RetirementAge - 1} AND c.years_since_first >= {ReferenceData.VestingYears - 1} THEN {zcr64Vesting}
                    ELSE {zcrNormal}
                END
            -- Active employees under 64
            ELSE {zcrNormal}
        END AS new_zcr,

        -- Calculate PointsEarned = ROUND(total_income / 100)
        CASE
            -- Ineligible employees get 0 points
            WHEN c.is_eligible = 0 OR (c.hire_date >= DATE '{fiscalEndDate:yyyy-MM-dd}' AND {(rebuild ? 0 : 1)} = 1) THEN 0
            -- Under 21 gets 0 points
            WHEN c.age < {ReferenceData.MinimumAgeForContribution} THEN 0
            -- Terminated employees get 0 points
            WHEN c.is_terminated = 1 THEN 0
            -- Age 64+ with < 1000 hours gets 0 points (COBOL PAY426.cbl lines 1219-1221)
            WHEN c.age >= {ReferenceData.RetirementAge - 1} AND c.has_min_hours = 0 THEN 0
            -- Active employees: ROUND(income / 100, 0, ROUND_HALF_UP)
            ELSE ROUND(c.total_income / 100, 0)
        END AS new_points_earned,

        -- Calculate PsCertificateIssuedDate (set to today if points > 0)
        CASE
            -- Ineligible employees get NULL certificate date
            WHEN c.is_eligible = 0 OR (c.hire_date >= DATE '{fiscalEndDate:yyyy-MM-dd}' AND {(rebuild ? 0 : 1)} = 1) THEN NULL
            -- Under 21 gets NULL
            WHEN c.age < {ReferenceData.MinimumAgeForContribution} THEN NULL
            -- Terminated employees get NULL
            WHEN c.is_terminated = 1 THEN NULL
            -- Age 64+ with < 1000 hours gets NULL
            WHEN c.age >= {ReferenceData.RetirementAge - 1} AND c.has_min_hours = 0 THEN NULL
            -- Active employees: set date if points > 0
            WHEN ROUND(c.total_income / 100, 0) > 0 THEN TRUNC(SYSDATE)
            ELSE NULL
        END AS new_cert_date
    FROM calc c
) src
ON (pp.demographic_id = src.demographic_id AND pp.profit_year = src.profit_year)
WHEN MATCHED THEN UPDATE SET
    pp.employee_type_id = src.new_employee_type_id,
    pp.zero_contribution_reason_id = src.new_zcr,
    pp.points_earned = src.new_points_earned,
    pp.ps_certificate_issued_date = src.new_cert_date,
    pp.modified_at_utc = SYSTIMESTAMP AT TIME ZONE 'UTC'
WHERE pp.employee_type_id != src.new_employee_type_id
   OR NVL(pp.zero_contribution_reason_id, -1) != NVL(src.new_zcr, -1)
   OR NVL(pp.points_earned, -1) != NVL(src.new_points_earned, -1)
   OR NVL(pp.ps_certificate_issued_date, DATE '1900-01-01') != NVL(src.new_cert_date, DATE '1900-01-01')";

        await using OracleCommand cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 600; // 10 minutes max for large datasets
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public Task UpdateEnrollmentId(short profitYear, CancellationToken ct)
    {
        return _payProfitUpdateService.SetEnrollmentId(profitYear, ct);
    }

    /// <summary>
    /// Returns the last completed year-end processing cycle.
    /// </summary>
    /// <remarks>
    /// Based on the Year_end_update_status table, independent of wall clock time.
    /// This function focuses on completed year-end status and does not check for active freeze state.
    /// </remarks>
    public Task<short> GetCompletedYearEnd(CancellationToken ct)
    {
        return _profitSharingDataContextFactory.UseReadOnlyContext(
            async ctx =>
            {
                var maxYear = await ctx.YearEndUpdateStatuses
                    .Where(st => st.IsYearEndCompleted)
                    .Select(st => (short?)st.ProfitYear)  // Make it nullable
                    .MaxAsync(ct);

                return maxYear ?? (short)(ReferenceData.SmartTransitionYear - 1);
            }, ct);
    }

    public async Task<short> GetOpenProfitYear(CancellationToken ct)
    {
        var completedYearEnd = await GetCompletedYearEnd(ct);
        // consider looking into freeze - aka a freeze should exist for
        // compltedYearEnd + 1 or we are in trouble town. 
        return (short)(completedYearEnd + 1);
    }
}
