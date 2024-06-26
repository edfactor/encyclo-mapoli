using System.Drawing.Text;
using Demoulas.ProfitSharing.Common.Configuration;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring logging.
/// </summary>
internal static class LoggerConfigurationExtension
{
    internal static WebApplicationBuilder SetDefaultLoggerConfiguration(this WebApplicationBuilder builder, ElasticSearchConfig smartConfig)
    {
        return SetDefaultLoggerConfiguration(builder, config =>
        {
            config.EnableElasticSearchLogging = smartConfig.EnableElasticSearchLogging;
            config.ProjectName = smartConfig.ProjectName;
            config.Namespace = smartConfig.Namespace;
            config.ElasticSearchNodes = smartConfig.ElasticSearchNodes;
        });
    }

    internal static WebApplicationBuilder SetDefaultLoggerConfiguration(this WebApplicationBuilder builder, Action<ElasticSearchConfig> config)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ElasticSearchConfig esConfig = new ElasticSearchConfig();
        config.Invoke(esConfig);


        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        var sinks = new LoggerConfiguration()
            .Enrich.WithOpenTelemetryTraceId()
            .Enrich.WithOpenTelemetrySpanId()
            .Enrich.FromLogContext()
            .Enrich.FromMassTransit()
            .Enrich.WithMemoryUsage()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithProperty("Environment", builder.Environment)
            .Enrich.WithSensitiveDataMasking(options =>
            {
                options.Mode = MaskingMode.Globally;
                options.MaskingOperators =
                [
                    new EmailAddressMaskingOperator(),
                    new IbanMaskingOperator(), // International Bank Account Number
                    new CreditCardMaskingOperator()
                ];
            })
            .ReadFrom.Configuration(builder.Configuration)
            .Filter.ByExcluding(
                Matching.WithProperty<string>("RequestPath", v =>
                    "/metrics".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                    "/ready".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                    "/live".Equals(v, StringComparison.OrdinalIgnoreCase)))
            .WriteTo.Debug(outputTemplate: outputTemplate)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: outputTemplate);

        if (esConfig.EnableElasticSearchLogging)
        {
            sinks.WriteTo.Elasticsearch(esConfig.ElasticSearchNodes ?? new List<Uri>(0), opts =>
            {
                opts.DataStream = new DataStreamName(type: "logs", esConfig.ProjectName ?? builder.Environment.ApplicationName,
                    esConfig.Namespace ?? builder.Environment.EnvironmentName);
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.ConfigureChannel = channelOpts => { channelOpts.BufferOptions = new BufferOptions { ExportMaxConcurrency = 10 }; };
            });
        }

        Log.Logger = sinks.CreateLogger();

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        builder.Host.UseSerilog();

        return builder;
    }
}
