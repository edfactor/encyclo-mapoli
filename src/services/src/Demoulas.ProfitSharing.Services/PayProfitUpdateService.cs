using System.Data;
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


    public PayProfitUpdateService(IProfitSharingDataContextFactory dataContextFactory, ILoggerFactory loggerFactory, ITotalService totalService, ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _logger = loggerFactory.CreateLogger<PayProfitUpdateService>();
        _totalService = (TotalService)totalService;
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
                var enrollmentUpdates = new Dictionary<int, byte>();

                foreach (var pp in allPayProfits)
                {
                    int ssn = pp.Demographic!.Ssn;
                    int demographicId = pp.DemographicId;

                    var pds = profitDetailBySsn.TryGetValue(ssn, out var list) ? list : [];

                    byte years = yearsBySsn.GetValueOrDefault(new { DemographicId = demographicId, Ssn = ssn }, (byte)0);

                    byte newEnrollmentId = 0;
                    if (pds.Count > 0)
                    {
                        var enrollmentSummarizer = new EnrollmentSummarizer();
                        newEnrollmentId = enrollmentSummarizer.ComputeEnrollment(pp, years, pds);
                    }

                    if (pp.EnrollmentId != newEnrollmentId)
                    {
                        enrollmentUpdates[ssn] = newEnrollmentId;
                    }
                }

                enrollmentComputationStopWatch.Stop();

                _logger.LogInformation("Computed enrollment IDs in {LogicTime:mm\\:ss}. {UpdateCount} employees need updates.", enrollmentComputationStopWatch.Elapsed,
                    enrollmentUpdates.Count);

                // Bulk update
                if (enrollmentUpdates.Count > 0)
                {
                    var bulkStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var oracleConnection = (ctx.Database.GetDbConnection() as OracleConnection)!;

                    await EnsureTempEnrollmentUpdatesTableExistsAsync(oracleConnection, ct);
                    await BulkUpdateEnrollmentIds(oracleConnection, profitYear, enrollmentUpdates, ct);
                    bulkStopwatch.Stop();

                    _logger.LogInformation("Bulk updated {UpdateCount} enrollment status in {BulkTime:mm\\:ss}", enrollmentUpdates.Count, bulkStopwatch.Elapsed);
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
                    enrollmentUpdates.Count);

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

    private static async Task EnsureTempEnrollmentUpdatesTableExistsAsync(OracleConnection conn, CancellationToken cancellation)
    {
        const string checkSql = @"
            SELECT COUNT(*) FROM all_tables 
            WHERE table_name = 'TEMP_ENROLLMENT_UPDATES' AND owner = USER";

        await using OracleCommand checkCmd = new(checkSql, conn);
        bool exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellation)) > 0;

        if (!exists)
        {
            const string createSql = @"
                CREATE GLOBAL TEMPORARY TABLE temp_enrollment_updates (
                    ssn                NUMBER(9),
                    profit_year        NUMBER(5),
                    enrollment_id      NUMBER(3)
                ) ON COMMIT DELETE ROWS";

            await using OracleCommand createCmd = new(createSql, conn);
            await createCmd.ExecuteNonQueryAsync(cancellation);
        }
    }

    private static async Task BulkUpdateEnrollmentIds(OracleConnection connection,
        short profitYear,
        Dictionary<int, byte> enrollmentUpdates,
        CancellationToken cancellation)
    {
        // Create DataTable for bulk insert
        using DataTable table = new();
        table.Columns.Add("ssn", typeof(int));
        table.Columns.Add("profit_year", typeof(short));
        table.Columns.Add("enrollment_id", typeof(byte));

        foreach ((int ssn, byte enrollmentId) in enrollmentUpdates)
        {
            table.Rows.Add(ssn, profitYear, enrollmentId);
        }

        // Bulk insert into temp table
        using OracleBulkCopy bulkCopy = new(connection) { DestinationTableName = "temp_enrollment_updates" };
        bulkCopy.WriteToServer(table);

        // MERGE update using temp table
        await using OracleCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            MERGE INTO pay_profit tgt
            USING (
                SELECT tmp.enrollment_id, d.id as demographic_id, tmp.profit_year
                FROM temp_enrollment_updates tmp
                JOIN demographic d ON d.ssn = tmp.ssn
            ) src
            ON (
                tgt.demographic_id = src.demographic_id
                AND tgt.profit_year = src.profit_year
            )
            WHEN MATCHED THEN UPDATE SET
                tgt.enrollment_id = src.enrollment_id";

        await cmd.ExecuteNonQueryAsync(cancellation);
    }
}
