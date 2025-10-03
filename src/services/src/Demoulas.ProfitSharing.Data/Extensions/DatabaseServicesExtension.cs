using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Contexts.Interceptor;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Data.Configuration;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;

public static class DatabaseServicesExtension
{
    /// <summary>
    /// Adds database services to the specified <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to which the database services will be added.
    /// </param>
    /// <param name="contextFactoryRequests">
    /// A collection of <see cref="ContextFactoryRequest"/> objects used to configure the database context factories.
    /// </param>
    /// <returns>
    /// The updated <see cref="IHostApplicationBuilder"/> with the database services registered.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a service of type <see cref="IProfitSharingDataContextFactory"/> is already registered in the service collection.
    /// </exception>
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder, Action<IServiceCollection,
        List<ContextFactoryRequest>>? contextFactoryRequests)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IProfitSharingDataContextFactory)))
        {
            throw new InvalidOperationException($"Service type {typeof(IProfitSharingDataContextFactory).FullName} is already registered.");
        }

        _ = builder.Services.AddSingleton<DataConfig>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            DataConfig dataConfig = new DataConfig();
            config.Bind("DataConfig", dataConfig);
            return dataConfig;
        });
        _ = builder.Services.AddScoped<AuditSaveChangesInterceptor>(provider =>
        {
            var user = provider.GetService<IAppUser>();
            var dataConfig = provider.GetRequiredService<DataConfig>();
            return new AuditSaveChangesInterceptor(dataConfig, user);
        });
        _ = builder.Services.AddSingleton<BeneficiarySaveChangesInterceptor>();
        _ = builder.Services.AddSingleton<BeneficiaryContactSaveChangesInterceptor>();
        _ = builder.Services.AddSingleton<ICommitGuardOverride, CommitGuardOverride>();
        _ = builder.Services.AddHttpContextAccessor();


        List<ContextFactoryRequest> factoryRequests = new();
        contextFactoryRequests?.Invoke(builder.Services, factoryRequests);
        IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, factoryRequests);

        _ = builder.Services.AddSingleton(factory);

        return builder;
    }
}
