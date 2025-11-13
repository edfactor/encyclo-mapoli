using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
///     Helper for timing async operations with automatic logging
/// </summary>
internal static class TimingHelper
{
    public static async Task<(T Result, double DurationMs)> TimeAsync<T>(Func<Task<T>> operation)
    {
        var start = DateTime.UtcNow;
        var result = await operation();
        var duration = (DateTime.UtcNow - start).TotalMilliseconds;
        return (result, duration);
    }
    
    public static async Task<(T Result, double DurationMs)> TimeAndLogAsync<T>(
        this ILogger logger,
        string operationName,
        Func<Task<T>> operation,
        Func<T, int>? getCount = null)
    {
        var (result, duration) = await TimeAsync(operation);
        
        if (getCount != null)
        {
            logger.LogInformation("{Name} completed in {DurationMs:F2}ms with {RecordCount} entries", 
                operationName, duration, getCount(result));
        }
        else
        {
            logger.LogInformation("{Name} completed in {DurationMs:F2}ms", operationName, duration);
        }
        
        return (result, duration);
    }
}
