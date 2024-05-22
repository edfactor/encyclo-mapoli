using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Factories;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;

public static class DatabaseServicesExtension
{
   public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder,
      IEnumerable<ContextFactoryRequest> contextFactoryRequests)
   {
       return Common.Data.Contexts.Extensions.DatabaseServicesExtension.AddDatabaseServices(builder,
           DataContextFactory.Initialize(builder, contextFactoryRequests: contextFactoryRequests));
   }
}
