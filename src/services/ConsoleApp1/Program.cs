using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;




var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<Program>();
builder.ConfigureOracleHcm();

var host = builder.Build();

var client = host.Services.GetRequiredService<PayrollSyncClient>();

await client.RetrievePayrollBalancesAsync([100000005363137, 100000005321744, 100000005360352]);







namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
