using Demoulas.Common.Contracts.Configuration;
using Demoulas.Security.Extensions;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Security.Extensions;

public static class SecurityExtension
{
    public static IHostApplicationBuilder AddSecurityServices(this WebApplicationBuilder builder)
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

        if (!builder.Environment.IsTestEnvironment() && Environment.GetEnvironmentVariable("YEMATCH_USE_TEST_CERTS") == null)
        {
            builder.Services.AddOktaSecurity(builder.Configuration);
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            builder.Services.AddTestingSecurity(builder.Configuration);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        _ = builder.ConfigureSecurityPolicies();

        return builder;
    }
}
