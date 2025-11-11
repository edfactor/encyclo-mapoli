using System.Data;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services;

public record YearEndChange
{
    public required int IsNew { get; init; }
    public required byte ZeroCont { get; init; }
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
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;

    public YearEndService(IProfitSharingDataContextFactory profitSharingDataContextFactory, ICalendarService calendar, IPayProfitUpdateService payProfitUpdateService,
        TotalService totalService, IDemographicReaderService demographicReaderService)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendar = calendar;
        _payProfitUpdateService = payProfitUpdateService;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
    }

    /// <summary>
    /// Returns the minimum birth date for an employee to be age 18+ on the fiscal end date.
    /// Age 18+ means DOB <= (fiscalEndDate - 18 years).
    /// COBOL PAY426.cbl line 936: IF W-AGE > 17
    /// </summary>
    private static DateOnly GetMinimumBirthDateForAge18(DateOnly fiscalEndDate)
    {
        return fiscalEndDate.AddYears(-ReferenceData.MinimumAgeForVesting());
    }

    /// <summary>
    /// Returns the minimum birth date for an employee to be age 64+ on the fiscal end date.
    /// Age 64+ means DOB <= (fiscalEndDate - 64 years).
    /// COBOL PAY426.cbl line 937: IF W-AGE NOT < 64 (age >= 64)
    /// </summary>
    private static DateOnly GetMinimumBirthDateForAge64(DateOnly fiscalEndDate)
    {
        // To include age 64+, we need DOB <= (fiscalEndDate - 64 years)
        // ReferenceData.RetirementAge() = 65, so -65 + 1 = -64
        return fiscalEndDate.AddYears(-ReferenceData.RetirementAge() + 1);
    }

    private static short CalculateAge(DateOnly dateOfBirth, DateOnly fiscalEndDate)
    {
        return dateOfBirth.Age(fiscalEndDate.ToDateTime(TimeOnly.MinValue));
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
            DateOnly minAge18BirthDate = GetMinimumBirthDateForAge18(fiscalEndDate);
            DateOnly minAge64BirthDate = GetMinimumBirthDateForAge64(fiscalEndDate);

            var frozenDemographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);

            IQueryable<PayProfitDto> query;

            if (rebuild) // rebuild is used after importing data - to rebuild the ZeroContribution
            {
                query = BuildQueryForRebuild(ctx, frozenDemographicQuery, profitYear);
            }
            else
            {
                query = BuildQueryForEligibleEmployees(ctx, frozenDemographicQuery, profitYear, fiscalEndDate, minAge18BirthDate, minAge64BirthDate);
            }

            List<PayProfitDto> employees = await query.ToListAsync(ct);

            HashSet<int> employeeSsnSet = employees.Select(pp => pp.Demographic!.Ssn).ToHashSet();

            Dictionary<int, decimal?> lastYearBalanceBySsn = await _totalService.GetTotalBalanceSet(ctx, (short)(profitYear - 1))
                .Where(pp => employeeSsnSet.Contains(pp.Ssn!))
                .ToDictionaryAsync(pt => pt.Ssn, pt => pt.TotalAmount, ct);

            Dictionary<int, short> firstContributionYearBySsn = await ctx.ProfitDetails
                .Where(pd => employeeSsnSet.Contains(pd.Ssn) &&
                             pd.ProfitCodeId == 0 &&
                             pd.Contribution > 0 &&
                             pd.ProfitYearIteration == 0)
                .GroupBy(pd => pd.Ssn)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Min(e => e.ProfitYear), ct);

            Dictionary<int, YearEndChange> changes = [];
            foreach (PayProfitDto employee in employees)
            {
                int ssn = employee.Demographic!.Ssn;
                short age = CalculateAge(employee.Demographic!.DateOfBirth, fiscalEndDate);
                short? firstYearContribution = firstContributionYearBySsn.TryGetValue(ssn, out short value) ? value : null;
                decimal lastYearBalance = lastYearBalanceBySsn.TryGetValue(ssn, out decimal? value1) ? value1 ?? 0m : 0m;

                YearEndChange change = YearEndChangeCalculator.ComputeChange(profitYear, firstYearContribution, age, lastYearBalance, employee, fiscalEndDate);
                if (change.IsChanged(employee))
                {
                    changes.Add(employee.Demographic.Id, change);
                }
            }

            OracleConnection oracleConnection = (ctx.Database.GetDbConnection() as OracleConnection)!;
            await EnsureTempPayProfitChangesTableExistsAsync(oracleConnection, ct);
            await UpdatePayProfitChanges(oracleConnection, profitYear, changes, rebuild, ct);

            // Reset IsNew field for employees who are no longer new (had first contribution in prior year)
            // This handles employees who don't meet the WHERE clause (< 1000 hours) but still need IsNew reset
            await ResetIsNewForPriorYearEmployeesAsync(ctx, profitYear, ct);

            // Reset ZeroCont/Points for ineligible employees (those excluded by WHERE clause)
            // COBOL PAY426 resets these to 0, but C# WHERE clause excludes them from processing
            await ResetIneligibleEmployeesAsync(ctx, profitYear, fiscalEndDate, ct);
        }, ct);
    }

    private static IQueryable<PayProfitDto> BuildQueryForRebuild(
        IProfitSharingDbContext ctx,
        IQueryable<Demographic> demographicQuery,
        short profitYear)
    {
        return ctx.PayProfits
            .Join(demographicQuery,
                pp => pp.DemographicId,
                d => d.Id,
                (pp, d) => new { pp, d })
            .AsNoTracking()
            .Where(x => x.pp.ProfitYear == profitYear)
            .Select(x => new PayProfitDto
            {
                ProfitYear = x.pp.ProfitYear,
                CurrentHoursYear = x.pp.CurrentHoursYear,
                HoursExecutive = x.pp.HoursExecutive,
                Demographic = x.d,
                EmployeeTypeId = x.pp.EmployeeTypeId,
                ZeroContributionReasonId = x.pp.ZeroContributionReasonId,
                PointsEarned = x.pp.PointsEarned,
                PsCertificateIssuedDate = x.pp.PsCertificateIssuedDate,
                IncomeExecutive = x.pp.IncomeExecutive,
                CurrentIncomeYear = x.pp.CurrentIncomeYear,
            });
    }

    private static IQueryable<PayProfitDto> BuildQueryForEligibleEmployees(
        IProfitSharingDbContext ctx,
        IQueryable<Demographic> demographicQuery,
        short profitYear,
        DateOnly fiscalEndDate,
        DateOnly minAge18BirthDate,
        DateOnly minAge64BirthDate)
    {
        // COBOL PAY426.cbl lines 936-944: Eligibility is (Age > 17 AND hours >= 1000) OR (Age > 63)
        return ctx.PayProfits
            .Join(demographicQuery,
                pp => pp.DemographicId,
                d => d.Id,
                (pp, d) => new { pp, d })
            .AsNoTracking()
            .Where(x =>
                x.pp.ProfitYear == profitYear &&
                (
                    (x.d.DateOfBirth <= minAge18BirthDate &&
                     (x.pp.CurrentHoursYear + x.pp.HoursExecutive) >= 1000)
                    || x.d.DateOfBirth <= minAge64BirthDate
                ) &&
                x.d.HireDate < fiscalEndDate)
            .Select(x => new PayProfitDto
            {
                ProfitYear = x.pp.ProfitYear,
                CurrentHoursYear = x.pp.CurrentHoursYear,
                HoursExecutive = x.pp.HoursExecutive,
                Demographic = x.d,
                EmployeeTypeId = x.pp.EmployeeTypeId,
                ZeroContributionReasonId = x.pp.ZeroContributionReasonId,
                PointsEarned = x.pp.PointsEarned,
                PsCertificateIssuedDate = x.pp.PsCertificateIssuedDate,
                IncomeExecutive = x.pp.IncomeExecutive,
                CurrentIncomeYear = x.pp.CurrentIncomeYear,
            });
    }

    /// <summary>
    /// Resets employee_type_id to 0 (NotNewLastYear) for employees who had their first contribution
    /// in a prior year but currently have employee_type_id = 1 (NewLastYear).
    /// This ensures employees who don't meet the hours requirement still get their IsNew field updated.
    /// Matches COBOL PAY426 behavior which processes ALL employees regardless of hours.
    /// </summary>
    private async Task ResetIsNewForPriorYearEmployeesAsync(
        IProfitSharingDbContext ctx,
        short profitYear,
        CancellationToken ct)
    {
        // Get all first contribution years (SSN -> FirstYear mapping)
        var firstContributionYears = await ctx.ProfitDetails
            .Where(pd => pd.ProfitCodeId == 0 &&
                        pd.Contribution > 0 &&
                        pd.ProfitYearIteration == 0)
            .GroupBy(pd => pd.Ssn)
            .Select(g => new
            {
                Ssn = g.Key,
                FirstYear = g.Min(pd => pd.ProfitYear)
            })
            .ToListAsync(ct);

        // Filter to SSNs where the first contribution was before this year
        var ssnsThatAreNoLongerNew = firstContributionYears
            .Where(x => x.FirstYear < profitYear)
            .Select(x => x.Ssn)
            .ToHashSet();

        if (ssnsThatAreNoLongerNew.Count == 0)
        {
            return; // Nothing to update
        }

        // Get demographic IDs for these SSNs
        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);
        var demographicIdsThatAreNoLongerNew = await demographicQuery
            .Where(d => ssnsThatAreNoLongerNew.Contains(d.Ssn))
            .Select(d => d.Id)
            .ToListAsync(ct);

        // Update employee_type_id to 0 for these employees (only if currently 1)
        await ctx.PayProfits
            .Where(pp => pp.ProfitYear == profitYear &&
                        pp.EmployeeTypeId == EmployeeType.Constants.NewLastYear &&
                        demographicIdsThatAreNoLongerNew.Contains(pp.DemographicId))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    pp => pp.EmployeeTypeId,
                    EmployeeType.Constants.NotNewLastYear),
                ct);
    }

    /// <summary>
    /// Resets zero_contribution_reason_id and points_earned to 0 for employees who don't meet
    /// the eligibility criteria (excluded by WHERE clause).
    /// COBOL PAY426 lines 1199-1203 resets employees to 0 if:
    /// 1. Hire date >= fiscal end date (future hires)
    /// 2. OR don't meet eligibility: (Age > 17 AND hours >= 1000) OR (Age > 63)
    /// </summary>
    private async Task ResetIneligibleEmployeesAsync(
        IProfitSharingDbContext ctx,
        short profitYear,
        DateOnly fiscalEndDate,
        CancellationToken ct)
    {
        DateOnly minAge18BirthDate = GetMinimumBirthDateForAge18(fiscalEndDate);
        DateOnly minAge64BirthDate = GetMinimumBirthDateForAge64(fiscalEndDate);

        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: true);

#pragma warning disable S125
        /*
        // COBOL PAY426 lines 1199-1203: Reset if (hire date >= fiscal end) OR (not eligible)
        // Step 1: Get DemographicIds to reset (Oracle doesn't allow ExecuteUpdate on JOINs)
        // Note: Eligibility logic inlined here because EF Core can't translate custom methods to SQL
        // Eligibility is: (Age > 17 AND hours >= 1000) OR (Age > 63)
        */
#pragma warning restore S125
        var demographicIdsToReset = await ctx.PayProfits
            .Join(demographicQuery,
                pp => pp.DemographicId,
                d => d.Id,
                (pp, d) => new { pp, d })
            .Where(x =>
                x.pp.ProfitYear == profitYear &&
                (
                    // Future hires: hired on or after fiscal end date
                    x.d.HireDate >= fiscalEndDate ||
                    // OR ineligible: NOT eligible (inlined logic from IsEligibleForProcessing)
                    !(
                        (x.d.DateOfBirth <= minAge18BirthDate && (x.pp.CurrentHoursYear + x.pp.HoursExecutive) >= 1000)
                        || x.d.DateOfBirth <= minAge64BirthDate
                    )
                ))
            .Select(x => x.pp.DemographicId)
            .ToListAsync(ct);

        // Step 2: Update by composite key (DemographicId + ProfitYear) - avoids Oracle ORA-01779 error
        if (demographicIdsToReset.Any())
        {
            await ctx.PayProfits
                .Where(pp => pp.ProfitYear == profitYear && demographicIdsToReset.Contains(pp.DemographicId))
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(pp => pp.ZeroContributionReasonId, (byte)0)
                        .SetProperty(pp => pp.PointsEarned, 0m)
                        .SetProperty(pp => pp.PsCertificateIssuedDate, (DateOnly?)null),
                    ct);
        }
    }

    public Task UpdateEnrollmentId(short profitYear, CancellationToken ct)
    {
        return _payProfitUpdateService.SetEnrollmentId(profitYear, ct);
    }

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

    /*
     * To update oracle quickly:
     * 1) we bulk insert our changes into temp table
     * 2) merge in changes
     */
    private static async Task UpdatePayProfitChanges(OracleConnection connection,
        int profitYear,
        Dictionary<int, YearEndChange> changes, bool rebuild, CancellationToken cancellation)
    {
        DataTable table = new();
        table.Columns.Add("demographic_id", typeof(int));
        table.Columns.Add("profit_year", typeof(int));
        table.Columns.Add("employee_type_id", typeof(int));
        table.Columns.Add("zero_contribution_reason_id", typeof(byte));
        table.Columns.Add("points_earned", typeof(decimal));
        table.Columns.Add("ps_certificate_issued_date", typeof(DateTime));

        foreach ((int demographicId, YearEndChange change) in changes)
        {
            table.Rows.Add(
                demographicId,
                profitYear,
                change.IsNew, // employee_type_id
                change.ZeroCont,
                change.EarnPoints,
                change.PsCertificateIssuedDate == null ? null : change.PsCertificateIssuedDate.Value.ToDateTime(TimeOnly.MinValue)
            );
        }

        using OracleBulkCopy bulkCopy = new(connection) { DestinationTableName = "temp_pay_profit_changes" };

        bulkCopy.WriteToServer(table);

        await using OracleCommand? cmd = connection.CreateCommand();
        cmd.CommandText = @"
            MERGE INTO pay_profit tgt
            USING temp_pay_profit_changes tmp
            ON (
                tgt.demographic_id = tmp.demographic_id
                AND tgt.profit_year = tmp.profit_year
            )
            WHEN MATCHED THEN UPDATE SET
                tgt.employee_type_id           = tmp.employee_type_id,
                tgt.zero_contribution_reason_id = tmp.zero_contribution_reason_id,
                tgt.points_earned              = tmp.points_earned,
                tgt.ps_certificate_issued_date = tmp.ps_certificate_issued_date";

        await cmd.ExecuteNonQueryAsync(cancellation);

        if (!rebuild)
        {
            // Copy the current ZeroContributionReason to the Now Year.    This might seem odd, but the YE process looks at ZeroContribution for
            // last year, and handles someone differently if they had a ZercontributionReason=6  last year.
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

            await cmd.ExecuteNonQueryAsync(cancellation);
        }
    }

    /*
     * We create a table on demand here.  This table is not part of the EF Frameworks migration
     * because it is used outside EF. Note that its contents are automatically deleted on commit.
     * The table is created on demand at first use.  If the schema of the table is to be altered, this
     * method would need to handle that.   This table's use is isolated to this service.
     */
    private static async Task EnsureTempPayProfitChangesTableExistsAsync(OracleConnection conn, CancellationToken cancellation)
    {
        const string checkSql = @"
            SELECT COUNT(*) FROM all_tables
            WHERE table_name = 'TEMP_PAY_PROFIT_CHANGES' AND owner = USER";

        await using OracleCommand checkCmd = new(checkSql, conn);
        bool exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellation)) > 0;

        if (!exists)
        {
            const string createSql = @"
                CREATE GLOBAL TEMPORARY TABLE temp_pay_profit_changes (
                    demographic_id                NUMBER(10),
                    profit_year                   NUMBER(4),
                    employee_type_id              NUMBER(5),
                    zero_contribution_reason_id   NUMBER(5),
                    points_earned                 NUMBER(7,2),
                    ps_certificate_issued_date    DATE
                ) ON COMMIT DELETE ROWS";

            await using OracleCommand createCmd = new(createSql, conn);
            await createCmd.ExecuteNonQueryAsync(cancellation);
        }
    }
}
