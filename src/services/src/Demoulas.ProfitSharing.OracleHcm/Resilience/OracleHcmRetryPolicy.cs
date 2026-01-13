using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Demoulas.ProfitSharing.OracleHcm.Resilience;

internal static class OracleHcmRetryPolicy
{
    public static ResiliencePipeline Create(string operationName, ILogger logger, IProcessWatchdog watchdog)
    {
        RetryStrategyOptions retryOptions = new()
        {
            MaxRetryAttempts = 6,
            Delay = TimeSpan.FromSeconds(5),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder().Handle<Exception>(OracleHcmTransientFaultDetector.IsTransient),
            OnRetry = args =>
            {
                watchdog.RecordHeartbeat();
                watchdog.RecordError($"{operationName} transient failure (attempt {args.AttemptNumber}/6). Retrying in {args.RetryDelay.TotalSeconds:N0}s. ExceptionType={args.Outcome.Exception?.GetType().Name}");

                if (args.Outcome.Exception is not null)
                {
                    logger.LogWarning(args.Outcome.Exception,
                        "{Operation} transient failure (attempt {Attempt}/6). Retrying in {DelaySeconds}s",
                        operationName,
                        args.AttemptNumber,
                        args.RetryDelay.TotalSeconds);
                }
                else
                {
                    logger.LogWarning(
                        "{Operation} transient failure (attempt {Attempt}/6). Retrying in {DelaySeconds}s",
                        operationName,
                        args.AttemptNumber,
                        args.RetryDelay.TotalSeconds);
                }

                return default;
            }
        };

        return new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .Build();
    }
}
