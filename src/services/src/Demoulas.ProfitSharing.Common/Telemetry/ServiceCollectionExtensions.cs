using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Extension methods for registering telemetry services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Profit Sharing specific telemetry configuration
    /// Note: This extends the base OpenTelemetry setup from Aspire extensions
    /// </summary>
    public static IServiceCollection AddProfitSharingTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure telemetry settings
        services.Configure<TelemetryConfiguration>(
            configuration.GetSection(TelemetryConfiguration.SectionName));

        // Initialize endpoint telemetry metrics
        EndpointTelemetry.Initialize();

        // Extend existing OpenTelemetry configuration (don't replace base setup)
        services.ConfigureOpenTelemetryMeterProvider(builder =>
        {
            builder.AddMeter(EndpointTelemetry.Meter.Name);
        });

        services.ConfigureOpenTelemetryTracerProvider(builder =>
        {
            builder.AddSource(EndpointTelemetry.ActivitySource.Name);
        });

        return services;
    }
}
