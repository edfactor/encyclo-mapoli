using System.Diagnostics;
using System.Net;
using System.Reflection;
using Asp.Versioning;
using Demoulas.ProfitSharing.Api.Converters;
using Demoulas.ProfitSharing.Api.Middleware;
using Demoulas.ProfitSharing.Api.Utilities;
using Demoulas.ProfitSharing.ServiceDefaults;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyModel;
using Serilog;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring logging.
/// </summary>
internal static class EndpointExtension
{
    internal static WebApplicationBuilder ConfigureDefaultEndpoints(this WebApplicationBuilder builder)
    {
        _ = builder.WebHost.UseKestrelHttpsConfiguration();
        _ = builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            // Remove the Server header
            serverOptions.AddServerHeader = false;
        });

        _ = builder.AddServiceDefaults();

        _ = builder.AddProjectServices();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _ = builder.Services.AddEndpointsApiExplorer();

        // Compression
        _ = builder.Services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
        });

        _ = builder.Services.AddHttpContextAccessor();

        _ = builder.Services
            .AddFastEndpoints(options =>
            {
                options.IncludeAbstractValidators = true;

                /*
                 * This resolves a bug in Fast-Endpoints where they are loading the current AppDomain assemblies
                 * however, in .NET, assemblies are loaded on domain, and if not yet referenced in code, this will fail.
                 * Using the Dependency injection system to discover all the assemblies, and then loading those explicitly fixes the issue.
                 * https://github.com/FastEndpoints/FastEndpoints/issues/653
                 */
                options.DisableAutoDiscovery = true;
                List<Assembly>? assemblies = DependencyContext.Default?
                    .RuntimeLibraries
                    .Where(a => a.Name.Contains("Demoulas", StringComparison.InvariantCultureIgnoreCase))
                    .Select(a => Assembly.Load(a.Name))
                    .ToList();

                options.Assemblies = assemblies;
            })
            .AddVersioning(o =>
            {
                //https://fast-endpoints.com/docs/api-versioning#enable-versioning
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
                o.ReportApiVersions = true;
                o.UnsupportedApiVersionStatusCode = (int)HttpStatusCode.NotImplemented;
            })
            .AddOutputCache(options => options.UseCaseSensitivePaths = false)
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.WriteIndented = Debugger.IsAttached;
                options.SerializerOptions.Converters.Add(new DateOnlyConverter());
                options.SerializerOptions.Converters.Add(new DateTimeConverter());
                options.SerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
            })
            .AddAntiforgery(o => o.HeaderName = "x-csrf-token");


        _ = builder.Services.AddSingleton<AppVersionInfo>();

        return builder;
    }

    public static IApplicationBuilder UseDefaultEndpoints(this WebApplication app)
    {
        _ = app.UseSerilogRequestLogging();
        _ = app.UseMiddleware<BadRequestExceptionMiddleware>();
        _ = app.UseMiddleware<ServerTimingMiddleware>();
        _ = app.UseVersionHeader();
        _ = app
            //.UseAuthentication()
            //.UseAuthorization()
            .UseResponseCompression()
            .UseOutputCache()
            .UseFastEndpoints(c =>
            {
                c.Versioning.Prefix = "v";
                c.Endpoints.RoutePrefix = "api";
                c.Endpoints.Filter = ep =>
                {
                    if (ep.EndpointTags?.Contains("Deprecated") is true)
                    {
                        return Debugger.IsAttached; // don't register this endpoint unless attached to the debugger
                    }

                    return true;
                };
            })
            .UseAntiforgery();

        if (!app.Environment.IsProduction())
        {
            _ = app.UseSwaggerGen();
        }

        return app;
    }
}
