using System.Diagnostics;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    // Constants for SQL operations
    private const int YearEndMergeSqlTimeoutSeconds = 600; // 10 minutes max for large datasets

    private readonly ICalendarService _calendar;
    private readonly IPayProfitUpdateService _payProfitUpdateService;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly ILogger<YearEndService> _logger;

    public YearEndService(
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        ICalendarService calendar,
        IPayProfitUpdateService payProfitUpdateService,
        ILogger<YearEndService> logger)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendar = calendar;
        _payProfitUpdateService = payProfitUpdateService;
        _logger = logger;
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
        using var activity = Activity.Current?.Source.StartActivity("RunFinalYearEndUpdates");
        activity?.SetTag("profit_year", profitYear);
        activity?.SetTag("rebuild", rebuild);

        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "Starting year-end final run updates for profit year {ProfitYear} (rebuild: {Rebuild})",
            profitYear, rebuild);

        try
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
                    _logger.LogDebug(
                        "Copying ZeroContributionReason from year {ProfitYear} to year {NextYear}",
                        profitYear, profitYear + 1);

                    // Copy the current ZeroContributionReason to the Now Year. This might seem odd, but the YE process looks at ZeroContribution for
                    // last year, and handles someone differently if they had a ZercontributionReason=6 last year.
                    await using OracleCommand cmd = oracleConnection.CreateCommand();
                    cmd.CommandText = @"
                        MERGE INTO pay_profit pp
                        USING pay_profit ppp
                        ON (
                            pp.demographic_id = ppp.demographic_id
                            AND pp.profit_year = :profitYearNext
                            AND ppp.profit_year = :profitYearCurrent
                        )
                        WHEN MATCHED THEN UPDATE SET
                            pp.zero_contribution_reason_id = ppp.zero_contribution_reason_id";
                    cmd.Parameters.Add(":profitYearNext", OracleDbType.Int16).Value = profitYear + 1;
                    cmd.Parameters.Add(":profitYearCurrent", OracleDbType.Int16).Value = profitYear;

                    int zcrRowsUpdated = await cmd.ExecuteNonQueryAsync(ct);
                    _logger.LogInformation(
                        "Copied ZeroContributionReason to next year: {RowsUpdated} records updated",
                        zcrRowsUpdated);
                }
            }, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "Completed year-end final run updates for profit year {ProfitYear} in {ElapsedMs}ms",
                profitYear, stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex,
                "Year-end final run updates cancelled for profit year {ProfitYear} after {ElapsedMs}ms",
                profitYear, stopwatch.ElapsedMilliseconds);
            // Rethrow cancellation with context
            throw new OperationCanceledException(
                $"Year-end final run updates for profit year {profitYear} were cancelled after {stopwatch.ElapsedMilliseconds}ms", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Year-end final run updates failed for profit year {ProfitYear} after {ElapsedMs}ms",
                profitYear, stopwatch.ElapsedMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            // Rethrow with contextual information
            throw new InvalidOperationException(
                $"Year-end final run updates failed for profit year {profitYear} after {stopwatch.ElapsedMilliseconds}ms. See inner exception for details.", ex);
        }
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
    private async Task ExecuteYearEndMergeAsync(
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

        // Constants for EmployeeType
        const byte empTypeNotNew = EmployeeType.Constants.NotNewLastYear; // 0
        const byte empTypeNew = EmployeeType.Constants.NewLastYear; // 1

        // Employment status constants
        const char empStatusTerminated = EmploymentStatus.Constants.Terminated; // 't'

        // Build the SQL - uses CTEs for clarity and to avoid repeated subqueries
        var pc0 = ProfitCode.Constants.IncomingContributions.Id;
        var pc1 = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id;
        var pc2 = ProfitCode.Constants.OutgoingForfeitures.Id;
        var pc3 = ProfitCode.Constants.OutgoingDirectPayments.Id;
        var pc5 = ProfitCode.Constants.OutgoingXferBeneficiary.Id;
        var pc9 = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
        var minHours = ReferenceData.MinimumHoursForContribution;
        string sql = $@"
MERGE INTO pay_profit pp
USING (
    WITH
    -- Get first contribution year for each SSN (employees who have had contributions)
    first_contrib AS (
        SELECT pd.ssn, MIN(pd.profit_year) AS first_year
        FROM profit_detail pd
        WHERE pd.profit_code_id = ${pc0}
          AND pd.contribution > 0
          AND pd.profit_year_iteration = ${ProfitCode.Constants.IncomingContributions.Id}
        GROUP BY pd.ssn
    ),
    -- Get prior year total balance for each SSN
    prior_balance AS (
        SELECT pd.ssn, SUM(
            CASE WHEN pd.profit_code_id = ${pc0} THEN pd.contribution ELSE 0 END
            + CASE WHEN pd.profit_code_id = ${pc0} THEN pd.earnings ELSE 0 END
            - CASE WHEN pd.profit_code_id IN (${pc1},${pc2},${pc3},${pc5},${pc9}) THEN pd.forfeiture ELSE 0 END
        ) AS total_balance
        FROM profit_detail pd
        WHERE pd.profit_year <= :priorYear
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
            FLOOR(MONTHS_BETWEEN(:fiscalEndDate, d.date_of_birth) / 12) AS age,
            -- Is terminated before fiscal end?
            CASE WHEN d.employment_status_id = :empStatusTerminated AND d.termination_date < :fiscalEndDate THEN 1 ELSE 0 END AS is_terminated,
            -- Has minimum hours (>= ${minHours})?
            CASE WHEN pp.total_hours >= ${minHours} THEN 1 ELSE 0 END AS has_min_hours,
            -- Is eligible for processing (age >= 18 AND hours >= ${minHours}) OR (age >= 64)
            CASE WHEN (d.date_of_birth <= :minAge18BirthDate AND pp.total_hours >= ${minHours})
                      OR d.date_of_birth <= :minAge64BirthDate
                 THEN 1 ELSE 0 END AS is_eligible,
            -- Years since first contribution (plan-year based)
            CASE
                WHEN fc.first_year IS NULL THEN 0
                ELSE :profitYear - fc.first_year
            END AS years_since_first
        FROM pay_profit pp
        JOIN demographic d ON pp.demographic_id = d.id
        LEFT JOIN first_contrib fc ON d.ssn = fc.ssn
        LEFT JOIN prior_balance pb ON d.ssn = pb.ssn
        WHERE pp.profit_year = :profitYear
          AND (:includeAllHireDates = 1 OR d.hire_date < :fiscalEndDate)
    )
    SELECT
        c.demographic_id,
        c.profit_year,
        -- Calculate new EmployeeTypeId (IsNew)
        CASE
            -- Terminated employees are never new
            WHEN c.is_terminated = 1 THEN :empTypeNotNew
            -- First contribution in prior year -> not new
            WHEN c.first_year IS NOT NULL AND c.first_year < :profitYear THEN :empTypeNotNew
            -- New employee: no first contribution AND age >= 21
            WHEN c.first_year IS NULL AND c.age >= :minAgeForContribution THEN :empTypeNew
            ELSE :empTypeNotNew
        END AS new_employee_type_id,

        -- Calculate new ZeroContributionReasonId
        CASE
            -- Ineligible employees (not meeting WHERE clause criteria) get reset to 0
            WHEN c.is_eligible = 0 OR (c.hire_date >= :fiscalEndDate AND :includeAllHireDates = 0) THEN :zcrNormal
            -- Under 21 with >= 1000 hours
            WHEN c.age < :minAgeForContribution THEN :zcrUnder21
            -- Terminated before fiscal end
            WHEN c.is_terminated = 1 THEN
                CASE
                    -- Age 65+ with 5+ years vesting
                    WHEN c.age >= :retirementAge AND c.years_since_first >= :vestingYears THEN :zcr65Plus5Years
                    -- Terminated with >= 1000 hours
                    WHEN c.has_min_hours = 1 THEN :zcrTerminated
                    ELSE :zcrNormal
                END
            -- Age 64+ employees (active)
            WHEN c.age >= :retirementAgeMinus1 THEN
                CASE
                    -- Preserve existing ZCR if >= 6
                    WHEN NVL(c.current_zcr, 0) >= :zcr65Plus5Years THEN NVL(c.current_zcr, :zcrNormal)
                    -- Age 65+ with 5+ years vesting
                    WHEN c.age >= :retirementAge AND c.years_since_first >= :vestingYears THEN :zcr65Plus5Years
                    ELSE :zcrNormal
                END
            -- Active employees under 64
            ELSE :zcrNormal
        END AS new_zcr,

        -- Calculate PointsEarned = ROUND(total_income / 100)
        CASE
            -- Ineligible employees get 0 points
            WHEN c.is_eligible = 0 OR (c.hire_date >= :fiscalEndDate AND :includeAllHireDates = 0) THEN 0
            -- Under 21 gets 0 points
            WHEN c.age < :minAgeForContribution THEN 0
            -- Terminated employees get 0 points
            WHEN c.is_terminated = 1 THEN 0
            -- Age 64+ with < 1000 hours gets 0 points (COBOL PAY426.cbl lines 1219-1221)
            WHEN c.age >= :retirementAgeMinus1 AND c.has_min_hours = 0 THEN 0
            -- Active employees: ROUND(income / 100, 0, ROUND_HALF_UP)
            ELSE ROUND(c.total_income / 100, 0)
        END AS new_points_earned,

        -- Calculate PsCertificateIssuedDate (set to today if points > 0)
        CASE
            -- Ineligible employees get NULL certificate date
            WHEN c.is_eligible = 0 OR (c.hire_date >= :fiscalEndDate AND :includeAllHireDates = 0) THEN NULL
            -- Under 21 gets NULL
            WHEN c.age < :minAgeForContribution THEN NULL
            -- Terminated employees get NULL
            WHEN c.is_terminated = 1 THEN NULL
            -- Age 64+ with < 1000 hours gets NULL
            WHEN c.age >= :retirementAgeMinus1 AND c.has_min_hours = 0 THEN NULL
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
-- Only update rows where at least one field changed (using sentinel values for NULL comparisons)
-- Sentinel values: -1 for nullable numbers, 1900-01-01 for nullable dates
WHERE pp.employee_type_id != src.new_employee_type_id
   OR NVL(pp.zero_contribution_reason_id, -1) != NVL(src.new_zcr, -1)
   OR NVL(pp.points_earned, -1) != NVL(src.new_points_earned, -1)
   OR NVL(pp.ps_certificate_issued_date, DATE '1900-01-01') != NVL(src.new_cert_date, DATE '1900-01-01')";

        _logger.LogDebug(
            "Executing year-end MERGE for profit year {ProfitYear} (fiscal end date: {FiscalEndDate}, rebuild: {Rebuild})",
            profitYear, fiscalEndDate, rebuild);

        try
        {
            await using OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = YearEndMergeSqlTimeoutSeconds;

            // Bind all parameters to prevent SQL injection
            cmd.Parameters.Add(":profitYear", OracleDbType.Int16).Value = profitYear;
            cmd.Parameters.Add(":priorYear", OracleDbType.Int16).Value = profitYear - 1;
            cmd.Parameters.Add(":fiscalEndDate", OracleDbType.Date).Value = fiscalEndDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add(":minAge18BirthDate", OracleDbType.Date).Value = minAge18BirthDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add(":minAge64BirthDate", OracleDbType.Date).Value = minAge64BirthDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add(":includeAllHireDates", OracleDbType.Int16).Value = rebuild ? 1 : 0;
            cmd.Parameters.Add(":empStatusTerminated", OracleDbType.Char).Value = empStatusTerminated;

            // Employee type constants
            cmd.Parameters.Add(":empTypeNotNew", OracleDbType.Byte).Value = empTypeNotNew;
            cmd.Parameters.Add(":empTypeNew", OracleDbType.Byte).Value = empTypeNew;

            // Zero contribution reason constants
            cmd.Parameters.Add(":zcrNormal", OracleDbType.Byte).Value = zcrNormal;
            cmd.Parameters.Add(":zcrUnder21", OracleDbType.Byte).Value = zcrUnder21;
            cmd.Parameters.Add(":zcrTerminated", OracleDbType.Byte).Value = zcrTerminated;
            cmd.Parameters.Add(":zcr65Plus5Years", OracleDbType.Byte).Value = zcr65Plus5Years;

            // Business rule constants
            cmd.Parameters.Add(":minAgeForContribution", OracleDbType.Int16).Value = ReferenceData.MinimumAgeForContribution;
            cmd.Parameters.Add(":retirementAge", OracleDbType.Int16).Value = ReferenceData.RetirementAge;
            cmd.Parameters.Add(":retirementAgeMinus1", OracleDbType.Int16).Value = ReferenceData.RetirementAge - 1;
            cmd.Parameters.Add(":vestingYears", OracleDbType.Int16).Value = ReferenceData.VestingYears;

            var sqlStopwatch = Stopwatch.StartNew();
            int rowsAffected = await cmd.ExecuteNonQueryAsync(ct);
            sqlStopwatch.Stop();

            _logger.LogInformation(
                "Year-end MERGE completed: {RowsAffected} records updated for profit year {ProfitYear} in {ElapsedMs}ms",
                rowsAffected, profitYear, sqlStopwatch.ElapsedMilliseconds);
        }
        catch (OracleException ex) when (ex.Number == -2) // ORA-00002: Timeout
        {
            _logger.LogError(ex,
                "Year-end MERGE timed out after {TimeoutSeconds}s for profit year {ProfitYear}",
                YearEndMergeSqlTimeoutSeconds, profitYear);
            throw new InvalidOperationException(
                $"Year-end MERGE operation timed out after {YearEndMergeSqlTimeoutSeconds}s for profit year {profitYear}. " +
                "Consider increasing timeout or optimizing data volume.", ex);
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex,
                "Year-end MERGE failed for profit year {ProfitYear}: Oracle error {OracleErrorNumber}",
                profitYear, ex.Number);
            throw new InvalidOperationException(
                $"Year-end MERGE operation failed for profit year {profitYear}: Oracle error {ex.Number} - {ex.Message}", ex);
        }
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
