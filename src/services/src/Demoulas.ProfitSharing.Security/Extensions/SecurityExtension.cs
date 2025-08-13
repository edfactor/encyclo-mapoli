using Demoulas.Common.Contracts.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Security.Extensions;

public static class SecurityExtension
{
    public static IHostApplicationBuilder AddSecurityServices(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddScoped<IClaimsTransformation, ImpersonationAndEnvironmentAwareClaimsTransformation>();
        _ = builder.Services.AddSingleton<OktaConfiguration>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            OktaConfiguration settings = new OktaConfiguration
            {
                OktaDomain = string.Empty,
                AuthorizationServerId = string.Empty,
                Audience = string.Empty,
                RolePrefix = string.Empty
            };
            config.Bind("Okta", settings);
            return settings;
        });

        

        return builder;
    }
}
