using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;

namespace Demoulas.ProfitSharing.Api.Extensions;

public static class RateLimitingExtensions
{
    private const string RateLimitPartitionKey = "rate-limit.partition-key";
    private const string RateLimitPartitionSourceKey = "rate-limit.partition-source";
    private const string GlobalLimiterName = "global";

    public static WebApplicationBuilder AddProfitSharingRateLimiting(this WebApplicationBuilder builder)
    {
        IConfigurationSection rateLimitingSection = builder.Configuration.GetSection("RateLimiting");
        bool rateLimitingEnabled = rateLimitingSection.GetValue("Enabled", true);

        if (!rateLimitingEnabled)
        {
            return builder;
        }

        IConfigurationSection globalLimiterSection = rateLimitingSection.GetSection("Global");

        int tokenLimit = globalLimiterSection.GetValue("TokenLimit", 600);
        int tokensPerPeriod = globalLimiterSection.GetValue("TokensPerPeriod", 120);
        int replenishmentSeconds = globalLimiterSection.GetValue("ReplenishmentPeriodSeconds", 60);
        int queueLimit = globalLimiterSection.GetValue("QueueLimit", 0);
        int defaultRetryAfterSeconds = globalLimiterSection.GetValue("RetryAfterSeconds", 60);

        TimeSpan replenishmentPeriod = TimeSpan.FromSeconds(replenishmentSeconds);

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                string partitionKey = ResolvePartitionKey(context, out string source);
                context.Items[RateLimitPartitionKey] = partitionKey;
                context.Items[RateLimitPartitionSourceKey] = source;

                return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = tokenLimit,
                    TokensPerPeriod = tokensPerPeriod,
                    ReplenishmentPeriod = replenishmentPeriod,
                    AutoReplenishment = true,
                    QueueLimit = queueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            options.OnRejected = (context, _) =>
            {
                HttpContext httpContext = context.HttpContext;
                string endpointName = httpContext.GetEndpoint()?.DisplayName ?? httpContext.Request.Path;

                IAppUser? appUser = httpContext.RequestServices.GetService(typeof(IAppUser)) as IAppUser;
                string userRole = string.Join(",", appUser?.GetUserAllRoles() ?? new List<string>());
                if (string.IsNullOrWhiteSpace(userRole))
                {
                    userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
                }

                string partitionKey = httpContext.Items.TryGetValue(RateLimitPartitionKey, out object? keyObj)
                    ? keyObj?.ToString() ?? "unknown"
                    : "unknown";
                string partitionSource = httpContext.Items.TryGetValue(RateLimitPartitionSourceKey, out object? sourceObj)
                    ? sourceObj?.ToString() ?? "unknown"
                    : "unknown";

                string maskedPartitionKey = MaskIdentifier(partitionKey);

                ILogger<Program> logger = httpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(
                    "Rate limit exceeded. Limiter={LimiterName} Endpoint={EndpointName} UserRole={UserRole} PartitionSource={PartitionSource} UserKey={UserKey}",
                    GlobalLimiterName,
                    endpointName,
                    userRole,
                    partitionSource,
                    maskedPartitionKey);

                EndpointTelemetry.EndpointErrorsTotal.Add(1,
                    new("endpoint.name", endpointName),
                    new("error.type", "RateLimit"),
                    new("user.role", userRole));

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    int retryAfterSeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
                    httpContext.Response.Headers.Append("Retry-After", retryAfterSeconds.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    httpContext.Response.Headers.Append("Retry-After",
                        defaultRetryAfterSeconds.ToString(CultureInfo.InvariantCulture));
                }

                return ValueTask.CompletedTask;
            };
        });

        return builder;
    }

    public static WebApplication UseProfitSharingRateLimiting(this WebApplication app)
    {
        bool rateLimitingEnabled = app.Configuration.GetSection("RateLimiting").GetValue("Enabled", true);
        if (rateLimitingEnabled)
        {
            app.UseRateLimiter();
        }

        return app;
    }

    private static string ResolvePartitionKey(HttpContext context, out string source)
    {
        IAppUser? appUser = context.RequestServices.GetService(typeof(IAppUser)) as IAppUser;
        string? userKey = appUser?.UserName;

        if (string.IsNullOrWhiteSpace(userKey))
        {
            userKey = appUser?.Email;
        }

        if (!string.IsNullOrWhiteSpace(userKey))
        {
            source = "IAppUser";
            return $"user:{userKey}";
        }

        string? sub = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrWhiteSpace(sub))
        {
            source = "sub";
            return $"sub:{sub}";
        }

        string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        source = "ip";
        return $"ip:{ip}";
    }

    private static string MaskIdentifier(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "unknown";
        }

        int separatorIndex = value.IndexOf(':', StringComparison.Ordinal);
        string prefix = separatorIndex > -1 ? value[..(separatorIndex + 1)] : string.Empty;
        string raw = separatorIndex > -1 ? value[(separatorIndex + 1)..] : value;

        if (raw.Length <= 4)
        {
            return $"{prefix}***";
        }

        return $"{prefix}{raw[0]}***{raw[^1]}";
    }
}
