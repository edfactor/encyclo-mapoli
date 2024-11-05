using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;




var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<Program>();
builder.ConfigureOracleHcm();

List<ContextFactoryRequest> list = new List<ContextFactoryRequest>
{
    ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("StoreInfo")
};

await builder.AddDatabaseServices(list);


var host = builder.Build();

var client = host.Services.GetRequiredService<PayrollSyncClient>();

await client.RetrievePayrollBalancesAsync(CancellationToken.None);







namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
