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
            DateOnly fiscalEndDate /*Fiscal End Date*/ = calendarInfo.FiscalEndDate;
            DateOnly vestingOver18 = fiscalEndDate.AddYears(-ReferenceData.MinimumAgeForVesting());
            DateOnly almostRetired64 = fiscalEndDate.AddYears(-ReferenceData.RetirementAge() - 1);

            var frozenDemographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);

            IQueryable<PayProfitDto> query;

            if (rebuild) // rebuild is used after importing data - to rebuild the ZeroContribution 
            {
                query = ctx.PayProfits
                    .Join(frozenDemographicQuery,
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
            else
            {
                query = ctx.PayProfits
                    .Join(frozenDemographicQuery,
                        pp => pp.DemographicId,
                        d => d.Id,
                        (pp, d) => new { pp, d })
                    .AsNoTracking()
                    .Where(x =>
                        x.pp.ProfitYear == profitYear &&
                        (
                            (x.d.DateOfBirth <= vestingOver18 &&
                             (x.pp.CurrentHoursYear + x.pp.HoursExecutive) >= 1000)
                            || x.d.DateOfBirth < almostRetired64
                        ) &&
                        x.d.HireDate < fiscalEndDate &&
                        !(
                            x.d.DateOfBirth <= almostRetired64 &&
                            (x.pp.CurrentHoursYear + x.pp.HoursExecutive) < 1000
                        ))
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
                short age = employee.Demographic!.DateOfBirth.Age(fiscalEndDate.ToDateTime(TimeOnly.MinValue));
                short? firstYearContribution = firstContributionYearBySsn.TryGetValue(ssn, out short value) ? value : null;
                decimal lastYearBalance = lastYearBalanceBySsn.TryGetValue(ssn, out decimal? value1) ? value1 ?? 0m : 0m;

                YearEndChange change = ComputeChange(profitYear, firstYearContribution, age, lastYearBalance, employee, fiscalEndDate);
                if (change.IsChanged(employee))
                {
                    changes.Add(employee.Demographic.Id, change);
                }
            }

            OracleConnection oracleConnection = (ctx.Database.GetDbConnection() as OracleConnection)!;
            await EnsureTempPayProfitChangesTableExistsAsync(oracleConnection, ct);
            await UpdatePayProfitChanges(oracleConnection, profitYear, changes, rebuild, ct);
        }, ct);
    }

    public Task UpdateEnrollmentId(short profitYear, CancellationToken ct)
    {
        return _payProfitUpdateService.SetEnrollmentId(profitYear, ct);
    }


    // Calculates the Year End Change for a single employee.
    //  Very closely follows PAY456.cbl, 405-calculate-points
    private YearEndChange ComputeChange(short profitYear, short? firstContributionYear, short age, decimal currentBalance, PayProfitDto employee, DateOnly fiscalEnd)
    {
        int newEmpl = 0;
        decimal points = 0;
        DateOnly? certificationDate = null;
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        byte? zeroContributionReason = null; 
        if (firstContributionYear == null && age >= ReferenceData.MinimumAgeForContribution())
        {
            newEmpl = /*1*/ EmployeeType.Constants.NewLastYear;
        }

        if (zeroContributionReason == null || zeroContributionReason < /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            zeroContributionReason = /*0*/ ZeroContributionReason.Constants.Normal;
        }

        if (age < /*21*/ ReferenceData.MinimumAgeForContribution())
        {
            zeroContributionReason = /*1*/ ZeroContributionReason.Constants.Under21WithOver1Khours;
            return new YearEndChange { IsNew = newEmpl, ZeroCont = (byte)zeroContributionReason, EarnPoints = 0, PsCertificateIssuedDate = certificationDate };
        }

        if (employee.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && employee.Demographic.TerminationDate < fiscalEnd) // assuming Frozen access here.
        {
            zeroContributionReason = /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested;
        }
        else
        {
            points = Math.Round((employee.IncomeExecutive + employee.CurrentIncomeYear) / 100.0m, MidpointRounding.AwayFromZero);
            if (points > 0)
            {
                certificationDate = today;
            }
        }

        if (age < /*64*/ (ReferenceData.RetirementAge() - 1))
        {
            if (employee.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && employee.Demographic.TerminationDate < fiscalEnd)
            {
                newEmpl = /*0*/ EmployeeType.Constants.NotNewLastYear;
            }

            return new YearEndChange { IsNew = newEmpl, ZeroCont = (byte)zeroContributionReason, EarnPoints = points, PsCertificateIssuedDate = certificationDate };
        }

        if (employee.ZeroContributionReasonId > /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested
            && employee.ZeroContributionReasonId != /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            zeroContributionReason = /*0*/ ZeroContributionReason.Constants.Normal;
        }

        int yearsSinceFirstContribution = profitYear - (firstContributionYear ?? profitYear);
        // bump up years 1 if an employee had money last year.   See PY-PS-AMT at PAY426.cbl#lines-1273
        if (currentBalance > 0)
        {
            yearsSinceFirstContribution++;
        }

        if (yearsSinceFirstContribution >= ReferenceData.VestingYears() && age >= ReferenceData.RetirementAge())
        {
            zeroContributionReason = /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested;
        }

        if (yearsSinceFirstContribution == (ReferenceData.VestingYears() - 1) && age >= ReferenceData.RetirementAge())
        {
            zeroContributionReason = /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay;
        }

        if (yearsSinceFirstContribution >= (ReferenceData.VestingYears() - 1) && age == (ReferenceData.RetirementAge() - 1))
        {
            zeroContributionReason = /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay;
        }

        if (employee.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && employee.Demographic.TerminationDate < fiscalEnd)
        {
            newEmpl = EmployeeType.Constants.NotNewLastYear;
        }

        return new YearEndChange { IsNew = newEmpl, ZeroCont = (byte)zeroContributionReason, EarnPoints = points, PsCertificateIssuedDate = certificationDate };
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
