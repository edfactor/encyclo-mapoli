using System.Diagnostics;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Provides a using-scoped helper for measuring database query duration separate from business logic.
/// Usage: using (var scope = DatabaseTelemetryScope.StartQuery("GetUnforfeitures")) { /* db query */ }
/// </summary>
public sealed class DatabaseTelemetryScope : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _operationType;
    private readonly string _endpointName;
    private readonly string _queryName;
    private bool _disposed;

    private DatabaseTelemetryScope(string operationType, string endpointName, string queryName)
    {
        _operationType = operationType;
        _endpointName = endpointName;
        _queryName = queryName;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Start tracking a database query operation.
    /// </summary>
    /// <param name="queryName">Descriptive name for the query (e.g., "GetUnforfeitures", "UpdateDemographics")</param>
    /// <param name="endpointName">Name of the endpoint executing the query (defaults to "Unknown")</param>
    /// <param name="operationType">Type of database operation: "read" or "write" (defaults to "read")</param>
    /// <returns>A disposable scope that records metrics when disposed</returns>
    public static DatabaseTelemetryScope StartQuery(string queryName, string endpointName = "Unknown", string operationType = "read")
    {
        return new DatabaseTelemetryScope(operationType, endpointName, queryName);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _stopwatch.Stop();

        // Record database query duration histogram
        EndpointTelemetry.DatabaseQueryDurationMs.Record(_stopwatch.Elapsed.TotalMilliseconds,
            new("endpoint", _endpointName),
            new("query_name", _queryName),
            new("operation_type", _operationType));

        // Record database operation counter
        EndpointTelemetry.DatabaseOperationsTotal.Add(1,
            new("endpoint", _endpointName),
            new("query_name", _queryName),
            new("operation_type", _operationType));

        _disposed = true;
    }
}
