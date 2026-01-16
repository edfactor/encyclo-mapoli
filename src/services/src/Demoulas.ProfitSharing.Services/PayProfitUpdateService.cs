using System.Data;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.EnrollmentFlag;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services;

public sealed class PayProfitUpdateService : IPayProfitUpdateService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly ILogger _logger;
    private readonly TotalService _totalService;
    private readonly Demoulas.ProfitSharing.Common.Interfaces.IVestingScheduleService _vestingScheduleService;

    public PayProfitUpdateService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        ITotalService totalService,
        ICalendarService calendarService,
        Demoulas.ProfitSharing.Common.Interfaces.IVestingScheduleService vestingScheduleService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _logger = loggerFactory.CreateLogger<PayProfitUpdateService>();
        _totalService = (TotalService)totalService;
        _vestingScheduleService = vestingScheduleService;
    }

    public async Task SetZeroContributionReason(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Beginning Set Zero Contribution Reason to {0}", zeroContributionReasonId))
        {
            await records.ExecuteUpdateAsync(x => x
                .SetProperty(pp => pp.ZeroContributionReasonId, zeroContributionReasonId)
                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow), cancellationToken);
        }
    }

    public async Task SetEnrollmentId(short profitYear, CancellationToken ct)
    {
        using (_logger.BeginScope("Setting EnrollmentId for ProfitYear {0}", profitYear))
        {
            await _dataContextFactory.UseWritableContext(async ctx =>
            {
                var allPayProfits = await ctx.PayProfits
                    .Where(pp => pp.ProfitYear == profitYear)
                    .Include(p => p.Demographic)
                    .OrderBy(pp => pp.Demographic!.Ssn)
                    .ToListAsync(ct);

                if (allPayProfits.Count == 0)
                {
                    _logger.LogInformation("No PayProfits found for year {ProfitYear}", profitYear);
                    return;
                }

                // Load years of service
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
                var yearsBySsn = await _totalService.GetYearsOfService(ctx, profitYear, calInfo.FiscalEndDate)
                    .ToDictionaryAsync(t => new { t.DemographicId, t.Ssn }, t => t.Years, ct);

                var totalPayProfits = allPayProfits.Count;
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var allSsns = allPayProfits.Select(pp => pp.Demographic!.Ssn).ToList();

                _logger.LogInformation("Loading ProfitDetails for {TotalEmployees} employees in batches...", totalPayProfits);
                var dbStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var allProfitDetails = new List<ProfitDetail>();
                const int batchSize = 2000;

                int batchNumber = 0;
                foreach (var ssnBatch in allSsns.Chunk(batchSize))
                {
                    batchNumber++;
                    var batchProfitDetails = await ctx.ProfitDetails
                        .Where(pd => pd.ProfitYear <= profitYear && ssnBatch.Contains(pd.Ssn))
                        .AsNoTracking()
                        .ToListAsync(ct);
                    allProfitDetails.AddRange(batchProfitDetails);

                    _logger.LogDebug("Loaded batch {BatchNumber} ({BatchStart}-{BatchEnd}) with {BatchCount} ProfitDetails",
                        batchNumber, (batchNumber - 1) * batchSize + 1, Math.Min(batchNumber * batchSize, allSsns.Count), batchProfitDetails.Count);
                }
                dbStopwatch.Stop();

                _logger.LogInformation("Loaded {ProfitDetailCount} ProfitDetails in {DbTime:mm\\:ss}", allProfitDetails.Count, dbStopwatch.Elapsed);

                var profitDetailBySsn = allProfitDetails
                    .GroupBy(pd => pd.Ssn)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderBy(pd => pd.ProfitYear)
                            .ThenBy(pd => pd.ProfitYearIteration)
                            .ThenBy(pd => pd.ProfitCodeId)
                            .ToList());

                _logger.LogInformation("Computing enrollment status for {TotalEmployees} employees...", totalPayProfits);
                var enrollmentComputationStopWatch = System.Diagnostics.Stopwatch.StartNew();
                var payProfitEnrollmentUpdates = new Dictionary<(int DemographicId, short ProfitYear), (int VestingScheduleId, bool HasForfeited)>();

                foreach (var pp in allPayProfits)
                {
                    int ssn = pp.Demographic!.Ssn;
                    int demographicId = pp.DemographicId;

                    var pds = profitDetailBySsn.TryGetValue(ssn, out var list) ? list : [];

                    byte years = yearsBySsn.GetValueOrDefault(new { DemographicId = demographicId, Ssn = ssn }, (byte)0);

                    byte newEnrollmentId = 0;
                    if (pds.Count > 0)
                    {
                        var enrollmentSummarizer = new EnrollmentSummarizer(_vestingScheduleService);
                        newEnrollmentId = await enrollmentSummarizer.ComputeEnrollmentAsync(pp, years, pds, ct);
                    }

                    // Convert new enrollment ID to (VestingScheduleId, HasForfeited) tuple
                    var (newVestingScheduleId, newHasForfeited) = newEnrollmentId switch
                    {
                        EnrollmentConstants.NotEnrolled => (0, false),
                        EnrollmentConstants.OldVestingPlanHasContributions => (VestingSchedule.Constants.OldPlan, false),
                        EnrollmentConstants.NewVestingPlanHasContributions => (VestingSchedule.Constants.NewPlan, false),
                        EnrollmentConstants.OldVestingPlanHasForfeitureRecords => (VestingSchedule.Constants.OldPlan, true),
                        EnrollmentConstants.NewVestingPlanHasForfeitureRecords => (VestingSchedule.Constants.NewPlan, true),
                        _ => (0, false)
                    };

                    // Compare with current PayProfit state
                    if (pp.VestingScheduleId != newVestingScheduleId || pp.HasForfeited != newHasForfeited)
                    {
                        payProfitEnrollmentUpdates[(pp.DemographicId, pp.ProfitYear)] = (newVestingScheduleId, newHasForfeited);
                    }
                }

                enrollmentComputationStopWatch.Stop();

                _logger.LogInformation("Computed enrollment IDs in {LogicTime:mm\\:ss}. {UpdateCount} pay profits need updates.", enrollmentComputationStopWatch.Elapsed,
                    payProfitEnrollmentUpdates.Count);

                // Bulk update PAY_PROFIT enrollment fields
                if (payProfitEnrollmentUpdates.Count > 0)
                {
                    var bulkStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var oracleConnection = (ctx.Database.GetDbConnection() as OracleConnection)!;

                    await EnsureTempPayProfitEnrollmentUpdatesTableExistsAsync(oracleConnection, ct);
                    await BulkUpdatePayProfitEnrollments(oracleConnection, payProfitEnrollmentUpdates, ct);
                    bulkStopwatch.Stop();

                    _logger.LogInformation("Bulk updated {UpdateCount} pay profit enrollment status in {BulkTime:mm\\:ss}", payProfitEnrollmentUpdates.Count, bulkStopwatch.Elapsed);
                }
                else
                {
                    _logger.LogInformation("No enrollment status updates needed - all values are current");
                }

                _logger.LogInformation(
                    "Enrollment Status Update completed in {Elapsed:mm\\:ss} for {TotalRecords} records. " +
                    "Time breakdown - DB Load: {DbTime:mm\\:ss} | Enrollment Status: {LogicTime:mm\\:ss} | Updates: {UpdateCount}",
                    stopwatch.Elapsed,
                    totalPayProfits,
                    dbStopwatch.Elapsed,
                    enrollmentComputationStopWatch.Elapsed,
                    payProfitEnrollmentUpdates.Count);

                // Mark year end as completed.
                YearEndUpdateStatus? yeus = await ctx.YearEndUpdateStatuses.FirstOrDefaultAsync(yeStatus => yeStatus.ProfitYear == profitYear);
                if (yeus != null)
                {
                    yeus.IsYearEndCompleted = true;
                }

                await ctx.SaveChangesAsync(ct);
            }, ct);
        }
    }

    private static async Task EnsureTempPayProfitEnrollmentUpdatesTableExistsAsync(OracleConnection conn, CancellationToken cancellation)
    {
        const string checkSql = @"
            SELECT COUNT(*) FROM all_tables
            WHERE table_name = 'TEMP_PAY_PROFIT_ENROLLMENT_UPDATES' AND owner = USER";

        await using OracleCommand checkCmd = new(checkSql, conn);
        bool exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellation)) > 0;

        if (!exists)
        {
            const string createSql = @"
                CREATE GLOBAL TEMPORARY TABLE temp_pay_profit_enrollment_updates (
                    demographic_id           NUMBER(9),
                    profit_year              NUMBER(4),
                    vesting_schedule_id      NUMBER(10),
                    has_forfeited            NUMBER(1)
                ) ON COMMIT DELETE ROWS";

            await using OracleCommand createCmd = new(createSql, conn);
            await createCmd.ExecuteNonQueryAsync(cancellation);
        }
    }

    private static async Task BulkUpdatePayProfitEnrollments(OracleConnection connection,
        Dictionary<(int DemographicId, short ProfitYear), (int VestingScheduleId, bool HasForfeited)> payProfitEnrollmentUpdates,
        CancellationToken cancellation)
    {
        // Create DataTable for bulk insert
        using DataTable table = new();
        table.Columns.Add("demographic_id", typeof(int));
        table.Columns.Add("profit_year", typeof(short));
        table.Columns.Add("vesting_schedule_id", typeof(int));
        table.Columns.Add("has_forfeited", typeof(byte));

        foreach (((int demographicId, short profitYear), (int vestingScheduleId, bool hasForfeited)) in payProfitEnrollmentUpdates)
        {
            table.Rows.Add(
                demographicId,
                profitYear,
                vestingScheduleId,
                hasForfeited ? (byte)1 : (byte)0);
        }

        // Bulk insert into temp table
        using OracleBulkCopy bulkCopy = new(connection) { DestinationTableName = "temp_pay_profit_enrollment_updates" };
        bulkCopy.WriteToServer(table);

        // MERGE update using temp table - updates PAY_PROFIT enrollment fields
        await using OracleCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            MERGE INTO pay_profit tgt
            USING temp_pay_profit_enrollment_updates src
            ON (tgt.demographic_id = src.demographic_id AND tgt.profit_year = src.profit_year)
            WHEN MATCHED THEN UPDATE SET
                tgt.vesting_schedule_id = src.vesting_schedule_id,
                tgt.has_forfeited = src.has_forfeited,
                tgt.modified_at_utc = SYS_EXTRACT_UTC(SYSTIMESTAMP)";

        await cmd.ExecuteNonQueryAsync(cancellation);
    }
}
