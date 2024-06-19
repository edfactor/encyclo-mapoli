using System.Drawing.Text;
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
    internal static WebApplicationBuilder SetDefaultLoggerConfiguration(this WebApplicationBuilder builder, string indexPrefix)
    {
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
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
            .WriteTo.Debug(outputTemplate: outputTemplate)
            .WriteTo.Console(outputTemplate: outputTemplate, theme: AnsiConsoleTheme.Code)
            //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://products.elasticsearch:9200"))
            //{
            //    FailureCallback = e => Console.WriteLine($"Unable to submit event {e.MessageTemplate}"),
            //    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
            //                       EmitEventFailureHandling.WriteToFailureSink |
            //                       EmitEventFailureHandling.RaiseCallback,
            //    AutoRegisterTemplate = true,
            //    OverwriteTemplate = true,
            //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            //    IndexFormat = $"{indexPrefix}-{0:yyyy.MM}",
            //    BatchPostingLimit = byte.MaxValue,
            //    BatchAction = ElasticOpType.Create,
            //    InlineFields = true,
            //    NumberOfShards = 20,
            //    NumberOfReplicas = 10,
            //})
            .ReadFrom.Configuration(builder.Configuration)
            .Filter.ByExcluding(
                Matching.WithProperty<string>("RequestPath", v =>
                    "/metrics".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                    "/ready".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                    "/live".Equals(v, StringComparison.OrdinalIgnoreCase)))
            .CreateLogger();

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        builder.Host.UseSerilog();

        return builder;
    }
}
