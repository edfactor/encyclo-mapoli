using System.Diagnostics.Metrics;

namespace Demoulas.ProfitSharing.Common.Metrics;

/// <summary>
/// Instruments for demographics batch ingestion. Uses the shared GlobalMeter.
/// Naming follows semantic low-cardinality rules; dimensions provided as tags where needed.
/// </summary>
public static class DemographicsIngestMetrics
{
    private static readonly Lock _obj = new Lock();
    private static bool _initialized;
    public static Counter<long> BatchesTotal { get; private set; } = null!;
    public static Histogram<double> BatchDurationMs { get; private set; } = null!;
    public static Counter<long> RecordsRequested { get; private set; } = null!;
    public static Counter<long> PrimaryMatched { get; private set; } = null!;
    public static Counter<long> FallbackPairs { get; private set; } = null!;
    public static Counter<long> FallbackMatched { get; private set; } = null!;
    public static Counter<long> RecordsInserted { get; private set; } = null!;
    public static Counter<long> RecordsUpdated { get; private set; } = null!;
    public static Counter<long> FallbackSkippedAllZeroBadge { get; private set; } = null!;
    public static Counter<long> BatchFailures { get; private set; } = null!;

    public static void EnsureInitialized()
    {
        lock (_obj)
        {
            if (_initialized)
            {
                return;
            }
        }

        lock (_obj)
        {
            if (_initialized)
            {
                return;
            }

            var m = GlobalMeter.Meter; // triggers lazy init
            BatchesTotal = m.CreateCounter<long>("demographics.ingest.batches.total");
            BatchDurationMs = m.CreateHistogram<double>("demographics.ingest.batch.duration.ms");
            RecordsRequested = m.CreateCounter<long>("demographics.ingest.records.requested");
            PrimaryMatched = m.CreateCounter<long>("demographics.ingest.records.primary.matched");
            FallbackPairs = m.CreateCounter<long>("demographics.ingest.fallback.pairs");
            FallbackMatched = m.CreateCounter<long>("demographics.ingest.records.fallback.matched");
            RecordsInserted = m.CreateCounter<long>("demographics.ingest.records.inserted");
            RecordsUpdated = m.CreateCounter<long>("demographics.ingest.records.updated");
            FallbackSkippedAllZeroBadge = m.CreateCounter<long>("demographics.ingest.fallback.skipped_all_zero_badge");
            BatchFailures = m.CreateCounter<long>("demographics.ingest.batches.failures");
            _initialized = true;
        }
    }
}
