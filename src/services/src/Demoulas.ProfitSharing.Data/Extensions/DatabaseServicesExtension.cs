using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;

public static class DatabaseServicesExtension
{
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder,
       IEnumerable<ContextFactoryRequest> contextFactoryRequests)
    {
        builder.Services.AddSingleton<IProfitSharingDataContextFactory>(DataContextFactory.Initialize(builder, contextFactoryRequests: contextFactoryRequests));

        return builder;
    }
}
