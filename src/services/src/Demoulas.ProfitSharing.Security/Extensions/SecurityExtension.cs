using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Security.Extensions;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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

        if (!builder.Environment.IsTestEnvironment())
        {
            // Local Development - pull from User-Secrets - Check if a fallback test signing key is configured
            var testSigningKey = builder.Configuration["Testing:JwtSigningKey"];

            if (string.IsNullOrEmpty(testSigningKey))
            {
                // Production: Okta only
                builder.Services.AddOktaSecurity(builder.Configuration);
            }
            else
            {
                // Development: Okta with development fallback to test token validation
                builder.Services.AddOktaSecurity(
                    builder.Configuration,
                    onAuthenticationFailed: CreateTestTokenFallbackHandler(testSigningKey));
            }
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

    /// <summary>
    /// Creates a fallback authentication handler that validates test tokens when Okta authentication fails.
    /// This allows both Okta and test tokens to work during local development (or running long running integration
    /// tests side by side with the existing UI w/o stopping and starting the server.)
    /// </summary>
    private static Func<Microsoft.AspNetCore.Authentication.JwtBearer.AuthenticationFailedContext, Task> CreateTestTokenFallbackHandler(string testSigningKey)
    {
        return async context =>
        {
            var testKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSigningKey));
            var handler = new JwtSecurityTokenHandler();

            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return; // No token to validate
            }

            var token = authHeader["Bearer ".Length..];

            try
            {
                var result = await handler.ValidateTokenAsync(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = testKey
                });

                if (result.IsValid)
                {
                    // Test token validation succeeded - set the principal
                    context.Principal = result.ClaimsIdentity != null
                        ? new System.Security.Claims.ClaimsPrincipal(result.ClaimsIdentity)
                        : null;
                    context.Success();
                }
            }
            catch
            {
                // Both Okta and test token validation failed - let the original error propagate
            }
        };
    }
}
