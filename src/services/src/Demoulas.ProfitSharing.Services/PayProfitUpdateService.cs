using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            await records.ExecuteUpdateAsync(x => x.SetProperty(pp => pp.ZeroContributionReasonId, zeroContributionReasonId), cancellationToken);
        }
    }
    public async Task SetEnrollmentId(short profitYear, CancellationToken ct)
    {
        using (_logger.BeginScope("Setting EnrollmentId for ProfitYear {0}", profitYear))
        {
            await _dataContextFactory.UseWritableContext(async ctx =>
            {
                // Load all PayProfits for the year (with demographics)
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

                // Load years of service once (read-only)
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
                var yearsBySsn = await _totalService.GetYearsOfService(ctx, profitYear, calInfo.FiscalEndDate)
                    .ToDictionaryAsync(t => t.Ssn, t => t.Years, ct);

                var enrollmentSummarizer = new EnrollmentSummarizer();

                // Process in chunks to manage memory usage
                const int chunkSize = 3000; // SSNs per chunk - adjust based on memory constraints
                var totalPayProfits = allPayProfits.Count;
                var totalChunks = (int)Math.Ceiling((double)totalPayProfits / chunkSize);
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
                {
                    var chunkPayProfits = allPayProfits
                        .Skip(chunkIndex * chunkSize)
                        .Take(chunkSize)
                        .ToList();

                    var chunkSsns = chunkPayProfits.Select(pp => pp.Demographic!.Ssn).ToList();

                    // Load ProfitDetails only for this chunk's SSNs
                    var chunkProfitDetails = await ctx.ProfitDetails
                        .Where(pd => pd.ProfitYear <= profitYear && chunkSsns.Contains(pd.Ssn))
                        .AsNoTracking()
                        .ToListAsync(ct);

                    // Group and sort ProfitDetails for this chunk
                    var profitDetailBySsn = chunkProfitDetails
                        .GroupBy(pd => pd.Ssn)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderBy(pd => pd.ProfitYear)
                                .ThenBy(pd => pd.ProfitYearIteration)
                                .ThenBy(pd => pd.ProfitCodeId)
                                .ToList());

                    // Process PayProfits in this chunk
                    foreach (var pp in chunkPayProfits)
                    {
                        int ssn = pp.Demographic!.Ssn;

                        var pds = profitDetailBySsn.TryGetValue(ssn, out var list)
                            ? list
                            : new List<ProfitDetail>();

                        byte years = yearsBySsn.TryGetValue(ssn, out var y) ? y : (byte)0;

                        byte newEnrollmentId = 0;
                        if (pds.Count > 0)
                        {
                            newEnrollmentId = enrollmentSummarizer.ComputeEnrollment(pp, years, pds);
                        }

                        pp.EnrollmentId = newEnrollmentId;
                    }

                    // Save changes for this chunk
                    await ctx.SaveChangesAsync(ct);

                    // Progress logging
                    var completedChunks = chunkIndex + 1;
                    var processedPayProfits = Math.Min(completedChunks * chunkSize, totalPayProfits);
                    double percent = (processedPayProfits * 100.0) / totalPayProfits;

                    var elapsed = stopwatch.Elapsed;
                    var avgPerChunk = TimeSpan.FromTicks(elapsed.Ticks / completedChunks);
                    var remaining = TimeSpan.FromTicks(avgPerChunk.Ticks * (totalChunks - completedChunks));

                    _logger.LogInformation(
                        "Enrollment Update Completed {Processed}/{Total} ({Percent:F1}%) " +
                        "Chunk {ChunkIndex}/{TotalChunks} - ETA: {Eta:mm\\:ss} (Elapsed: {Elapsed:mm\\:ss})",
                        processedPayProfits,
                        totalPayProfits,
                        percent,
                        completedChunks,
                        totalChunks,
                        remaining,
                        elapsed);
                }

                _logger.LogInformation(
                    "Enrollment Update completed in {Elapsed:mm\\:ss} for {TotalRecords} records",
                    stopwatch.Elapsed,
                    totalPayProfits);
                
                YearEndUpdateStatus? yeus = ctx.YearEndUpdateStatuses.FirstOrDefault(yeStatus => yeStatus.ProfitYear == profitYear);
                if (yeus != null)
                {
                    yeus.IsYearEndCompleted = true;
                }
                await ctx.SaveChangesAsync(ct);

            }, ct);
        }
    }

}
