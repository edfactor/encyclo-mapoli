using System.Diagnostics.Metrics;
using Demoulas.Common.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection; 

namespace Demoulas.ProfitSharing.Common.Metrics;

/// <summary>
/// Centralized Meter and common instruments for ProfitSharing services.
/// Keep cardinality low: only include environment/service/job_name/http_method, etc. as tags when needed.
/// </summary>
public static class GlobalMeter
{
    public const string Name = "demoulas.profitsharing";
    private static Meter? _meter;
    private static readonly object _initLock = new();
    public static Meter Meter
    {
        get
        {
            if (_meter is null)
            {
                EnsureInitializedLazy();
            }
            return _meter!;
        }
    }

    // API (initialized after meter creation)
    public static Counter<long> ApiRequests { get; private set; } = null!;
    public static Histogram<double> ApiRequestDurationMs { get; private set; } = null!;

    // Jobs / Background work
    public static Counter<long> JobRunCount { get; private set; } = null!;
    public static Histogram<double> JobRunDurationMs { get; private set; } = null!;
    public static Counter<long> JobRunFailures { get; private set; } = null!;
    public static Counter<long> JobProcessedRecords { get; private set; } = null!;

    // Health / incident metrics
    public static Counter<long> IncidentsTotal { get; private set; } = null!;
    public static Counter<long> IncidentResolutionsTotal { get; private set; } = null!;
    private static int _healthStatus = 1; // 1 healthy, 0 unhealthy
    private static int _incidentOpen; // 0 closed, 1 open
    private static IAppVersionInfo? _appVersionInfo;

    // Deployment / build metrics
    public static Counter<long> DeploymentsTotal { get; private set; } = null!;
    
    // in-flight gauge implemented via an observable gauge
    private static int _jobRunsInflight;

    public static void IncrementJobInflight() => Interlocked.Increment(ref _jobRunsInflight);
    public static void DecrementJobInflight() => Interlocked.Decrement(ref _jobRunsInflight);

    /// <summary>
    /// Register observable gauges. Call once at startup.
    /// </summary>
    public static void RegisterObservableGauges()
    {
        if (!_initialized || _meter is null)
        {
            EnsureInitializedLazy();
        }

        // Note: CreateObservableGauge overloads differ across framework versions.
        // This callback returns the current in-flight job count.
        Meter.CreateObservableGauge("job.runs.inflight", () => Volatile.Read(ref _jobRunsInflight));
        Meter.CreateObservableGauge("health.status", () => Volatile.Read(ref _healthStatus));
    }

    /// <summary>
    /// Optionally supply the root service provider to allow resolving version info.
    /// Call once during startup after building the app/host.
    /// </summary>
    private static bool _initialized;

    public static void InitializeFromServices(IServiceProvider serviceProvider)
    {
        if (_initialized) {return;}
        lock (_initLock)
        {
            if (_initialized) {return;}
            try
            {
                _appVersionInfo = serviceProvider.GetService<IAppVersionInfo>();
            }
            catch
            {
                // ignore
            }
            string version = _appVersionInfo?.BuildNumber ?? "0.0.0";
            InitializeCore(version);
        }
    }

    private static void EnsureInitializedLazy()
    {
        if (_initialized) {return;}
        lock (_initLock)
        {
            if (_initialized) {return;}
            InitializeCore("0.0.0");
        }
    }

    private static void InitializeCore(string version)
    {
        _meter ??= new Meter(Name, version);


        ApiRequests = Meter.CreateCounter<long>("api.requests.total");
        ApiRequestDurationMs = Meter.CreateHistogram<double>("api.request.duration.ms");
        JobRunCount = Meter.CreateCounter<long>("job.run.count");
        JobRunDurationMs = Meter.CreateHistogram<double>("job.run.duration.ms");
        JobRunFailures = Meter.CreateCounter<long>("job.run.failures");
        JobProcessedRecords = Meter.CreateCounter<long>("job.processed_records.total");
        IncidentsTotal = Meter.CreateCounter<long>("incidents.total");
        IncidentResolutionsTotal = Meter.CreateCounter<long>("incidents.resolved.total");
        DeploymentsTotal = Meter.CreateCounter<long>("deployments.total");
        _initialized = true;
    }

    public static void UpdateHealthStatus(bool healthy)
    {
        Volatile.Write(ref _healthStatus, healthy ? 1 : 0);
    }

    public static void RegisterIncidentStartIfNeeded(string status)
    {
        if (Interlocked.CompareExchange(ref _incidentOpen, 1, 0) == 0)
        {
            IncidentsTotal.Add(1, new KeyValuePair<string, object?>("status", status));
        }
    }

    public static void RegisterIncidentResolutionIfNeeded(string status)
    {
        if (Interlocked.CompareExchange(ref _incidentOpen, 0, 1) == 1)
        {
            IncidentResolutionsTotal.Add(1, new KeyValuePair<string, object?>("status", status));
        }
    }

    /// <summary>
    /// Records a deployment event once per process start. Reads common env vars: GIT_COMMIT, BUILD_VERSION, BUILD_TIMESTAMP.
    /// </summary>
    public static void RecordDeploymentStartup()
    {
        string buildVersion = _appVersionInfo?.BuildNumber ?? "0.0.0"; // No env fallback
        string? gitHash = _appVersionInfo?.GitHash ?? "unknown";
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";

        
        DeploymentsTotal.Add(1,
            new KeyValuePair<string, object?>("git.commit", gitHash),
            new KeyValuePair<string, object?>("build.version", buildVersion),
            new KeyValuePair<string, object?>("environment", environment));
    }
}
