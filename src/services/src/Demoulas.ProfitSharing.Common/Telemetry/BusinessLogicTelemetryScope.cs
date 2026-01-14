using System.Diagnostics;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Provides a using-scoped helper for measuring business logic duration separate from database queries.
/// Usage: using (var scope = BusinessLogicTelemetryScope.Start("CalculateVesting")) { /* business logic */ }
/// </summary>
public sealed class BusinessLogicTelemetryScope : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _endpointName;
    private readonly string _operationName;
    private bool _disposed;

    private BusinessLogicTelemetryScope(string endpointName, string operationName)
    {
        _endpointName = endpointName;
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Start tracking a business logic operation (non-database).
    /// </summary>
    /// <param name="operationName">Descriptive name for the operation (e.g., "CalculateVesting", "MapToDto")</param>
    /// <param name="endpointName">Name of the endpoint executing the logic (defaults to "Unknown")</param>
    /// <returns>A disposable scope that records metrics when disposed</returns>
    public static BusinessLogicTelemetryScope Start(string operationName, string endpointName = "Unknown")
    {
        return new BusinessLogicTelemetryScope(endpointName, operationName);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _stopwatch.Stop();

        // Record business logic duration histogram
        EndpointTelemetry.BusinessLogicDurationMs.Record(_stopwatch.Elapsed.TotalMilliseconds,
            new("endpoint", _endpointName),
            new("operation_name", _operationName));

        _disposed = true;
    }
}
