using System.Runtime.InteropServices;
using Demoulas.ProfitSharing.OracleHcm.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.AddOracleHcmSynchronization();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddWindowsService(options => { options.ServiceName = "Demoulas ProfitSharing OracleHcm Sync"; });
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddSystemd();
}

var host = builder.Build();
await host.RunAsync();
