using System;
using System.Diagnostics.Metrics;
using System.Threading;

namespace Demoulas.ProfitSharing.Common.Metrics;

/// <summary>
/// Centralized Meter and common instruments for ProfitSharing services.
/// Keep cardinality low: only include environment/service/job_name/http_method, etc. as tags when needed.
/// </summary>
public static class GlobalMeter
{
    public const string Name = "demoulas.profitsharing";
    public static readonly Meter Meter = new Meter(Name, "1.0.0");

    // API
    public static readonly Counter<long> ApiRequests = Meter.CreateCounter<long>("api.requests.total");
    public static readonly Histogram<double> ApiRequestDurationMs = Meter.CreateHistogram<double>("api.request.duration.ms");

    // Jobs / Background work
    public static readonly Counter<long> JobRunCount = Meter.CreateCounter<long>("job.run.count");
    public static readonly Histogram<double> JobRunDurationMs = Meter.CreateHistogram<double>("job.run.duration.ms");
    public static readonly Counter<long> JobRunFailures = Meter.CreateCounter<long>("job.run.failures");
    public static readonly Counter<long> JobProcessedRecords = Meter.CreateCounter<long>("job.processed_records.total");

    // Health / incident metrics
    public static readonly Counter<long> IncidentsTotal = Meter.CreateCounter<long>("incidents.total");
    public static readonly Counter<long> IncidentResolutionsTotal = Meter.CreateCounter<long>("incidents.resolved.total");
    private static int _healthStatus = 1; // 1 healthy, 0 unhealthy
    private static int _incidentOpen; // 0 closed, 1 open

    // Deployment / build metrics
    public static readonly Counter<long> DeploymentsTotal = Meter.CreateCounter<long>("deployments.total");
    private static bool _deploymentRecorded;
    private static bool _buildAgeRegistered;
    private static DateTimeOffset? _buildTimestamp;

    // in-flight gauge implemented via an observable gauge
    private static int _jobRunsInflight;

    public static void IncrementJobInflight() => Interlocked.Increment(ref _jobRunsInflight);
    public static void DecrementJobInflight() => Interlocked.Decrement(ref _jobRunsInflight);

    /// <summary>
    /// Register observable gauges. Call once at startup.
    /// </summary>
    public static void RegisterObservableGauges()
    {
        // Note: CreateObservableGauge overloads differ across framework versions.
        // This callback returns the current in-flight job count.
        Meter.CreateObservableGauge("job.runs.inflight", () => Volatile.Read(ref _jobRunsInflight));
        if (!_buildAgeRegistered && _buildTimestamp.HasValue)
        {
            _buildAgeRegistered = true;
            Meter.CreateObservableGauge("build.age.seconds", () => (DateTimeOffset.UtcNow - _buildTimestamp!.Value).TotalSeconds);
        }

        Meter.CreateObservableGauge("health.status", () => System.Threading.Volatile.Read(ref _healthStatus));
    }

    public static void UpdateHealthStatus(bool healthy)
    {
        System.Threading.Volatile.Write(ref _healthStatus, healthy ? 1 : 0);
    }

    public static void RegisterIncidentStartIfNeeded(string status)
    {
        if (System.Threading.Interlocked.CompareExchange(ref _incidentOpen, 1, 0) == 0)
        {
            IncidentsTotal.Add(1, new KeyValuePair<string, object?>("status", status));
        }
    }

    public static void RegisterIncidentResolutionIfNeeded(string status)
    {
        if (System.Threading.Interlocked.CompareExchange(ref _incidentOpen, 0, 1) == 1)
        {
            IncidentResolutionsTotal.Add(1, new KeyValuePair<string, object?>("status", status));
        }
    }

    /// <summary>
    /// Records a deployment event once per process start. Reads common env vars: GIT_COMMIT, BUILD_VERSION, BUILD_TIMESTAMP.
    /// </summary>
    public static void RecordDeploymentStartup()
    {
        if (_deploymentRecorded)
        {
            return;
        }

        _deploymentRecorded = true;

        string? commit = Environment.GetEnvironmentVariable("GIT_COMMIT") ?? Environment.GetEnvironmentVariable("SOURCE_VERSION") ?? "unknown";
        string? branch = Environment.GetEnvironmentVariable("GIT_BRANCH") ?? Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCH") ?? "unknown";
        string? pipelineId = Environment.GetEnvironmentVariable("BUILD_PIPELINE_ID") ?? Environment.GetEnvironmentVariable("PIPELINE_ID") ?? "unknown";
        string? buildVersion = Environment.GetEnvironmentVariable("BUILD_VERSION") ?? AppDomain.CurrentDomain.FriendlyName;
        string? buildTsRaw = Environment.GetEnvironmentVariable("BUILD_TIMESTAMP");
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";

        if (buildTsRaw is not null)
        {
            // Accept common ISO formats
            string[] formats = ["O", "o", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.fffZ"]; // extend as needed
            if (DateTimeOffset.TryParseExact(buildTsRaw, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal,
                    out var ts))
            {
                _buildTimestamp = ts.ToUniversalTime();
            }
        }

        DeploymentsTotal.Add(1,
            new KeyValuePair<string, object?>("git.commit", commit),
            new KeyValuePair<string, object?>("git.branch", branch),
            new KeyValuePair<string, object?>("pipeline.id", pipelineId),
            new KeyValuePair<string, object?>("build.version", buildVersion),
            new KeyValuePair<string, object?>("environment", environment));
    }
}
