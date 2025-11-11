using Demoulas.Common.Contracts.Configuration;
using Demoulas.Security.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        // All environments use Okta as the primary authentication
        _ = builder.Services.AddOktaSecurity(builder.Configuration);

        // In Development mode, ALSO support Test Certs for integration tests
        // This eliminates the need to restart when switching between UI testing and integration tests
        if (builder.Environment.IsDevelopment())
        {
            _ = builder.Services.AddTestCertificateSupport(builder.Configuration);
        }

        _ = builder.ConfigureSecurityPolicies();

        return builder;
    }

    /// <summary>
    /// Adds test certificate support to the existing Okta authentication using dotnet user-jwts.
    /// This allows integration tests (using test JWTs) to work alongside UI (using Okta)
    /// without needing to restart the backend.
    /// </summary>
    private static IServiceCollection AddTestCertificateSupport(this IServiceCollection services, IConfiguration configuration)
    {
        // Post-configure the existing JWT Bearer authentication to add test JWT fallback
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            // Read signing key from dotnet user-jwts configuration (stored in user secrets)
            var signingKeyValue = configuration["Authentication:Schemes:Bearer:SigningKeys:0:Value"];

            if (string.IsNullOrEmpty(signingKeyValue))
            {
                throw new InvalidOperationException(
                    "No signing key found in user secrets. Run: dotnet user-jwts create --audience https://localhost:7141");
            }

            var testCertKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyValue));

            // Store original events
            var originalEvents = options.Events ?? new JwtBearerEvents();

            // Create new events that try test cert validation if Okta fails
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = originalEvents.OnMessageReceived,
                OnTokenValidated = originalEvents.OnTokenValidated,
                OnChallenge = originalEvents.OnChallenge,
                OnForbidden = originalEvents.OnForbidden,
                OnAuthenticationFailed = async context =>
                {
                    // If Okta validation failed, try test certificate validation
                    var authHeader = context.Request.Headers.Authorization.ToString();
                    var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? authHeader.Substring("Bearer ".Length).Trim()
                        : authHeader;

                    if (!string.IsNullOrEmpty(token))
                    {
                        try
                        {
                            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                            // Read issuer and audience from configuration
                            var validIssuer = configuration["Authentication:Schemes:Bearer:ValidIssuer"] ?? "dotnet-user-jwts";
                            var validAudience = configuration["Authentication:Schemes:Bearer:ValidAudiences:0"] ?? "https://localhost:7141";

                            var validationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = validIssuer,
                                ValidateAudience = true,
                                ValidAudience = validAudience,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = testCertKey,
                                ClockSkew = TimeSpan.FromMinutes(5)
                            };

                            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

                            if (result.IsValid)
                            {
                                // Success! Replace the failed authentication with the valid one
                                context.Principal = new System.Security.Claims.ClaimsPrincipal(result.ClaimsIdentity);
                                context.Success();
                                return;
                            }
                        }
                        catch
                        {
                            // Test cert validation also failed - let it fail
                        }
                    }

                    // Call original handler if it exists
                    if (originalEvents.OnAuthenticationFailed != null)
                    {
                        await originalEvents.OnAuthenticationFailed(context);
                    }
                }
            };

            // Allow HTTP in development (required for localhost testing)
            options.RequireHttpsMetadata = false;
        });

        return services;
    }
}
