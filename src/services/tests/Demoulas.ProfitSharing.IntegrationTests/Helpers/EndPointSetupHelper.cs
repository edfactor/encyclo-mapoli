using Demoulas.ProfitSharing.Api;
using Demoulas.Util;
using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.IntegrationTests.Helpers;

public sealed class EndPointSetupHelper
{
    private readonly ITestOutputHelper _outputHelper;

    private EndPointSetupHelper(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public static EndPointSetupHelper CreateInstance(ITestOutputHelper? outputHelper)
    {
        ArgumentNullException.ThrowIfNull(outputHelper);

        return new EndPointSetupHelper(outputHelper);
    }

    public TEndpoint Create<TEndpoint>() where TEndpoint : class, IEndpoint
    {
        var ep = Factory.Create<TEndpoint>(ctx =>
        {
            ctx.Request.Headers.ContentType = "application/json";
            ctx.Request.Headers.Accept = "application/json";

            ctx.AddTestServices(s =>
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.json")
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables(); // You can add other configuration sources here
                var configuration = configurationBuilder.Build();
                s.AddSingleton<IConfiguration>(configuration);


                s.AddDistributedMemoryCache();
                s.AddSingleton<IMemoryCache, MemoryCache>();
                s.AddSingleton<ICacheProvider, MemoryCacheProvider>();
            });

            var loggerFactory = ctx.RequestServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddXUnit(_outputHelper, LogLevel.Debug);
        });

        return ep;
    }
}
