using Demoulas.Common.Contracts.Configuration;
using Demoulas.Security.Extensions;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        // All environments use Okta as the primary authentication
        _ = builder.Services.AddOktaSecurity(builder.Configuration);

        // In Development or Test mode, ALSO support Test Certs for integration tests
        // This eliminates the need to restart when switching between UI testing and integration tests
        if (builder.Environment.IsDevelopment() || builder.Environment.IsTestEnvironment())
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

            SymmetricSecurityKey testCertKey;

            if (string.IsNullOrEmpty(signingKeyValue))
            {
                // Fall back to hardcoded test key for unit tests (same key used in test JWT generation)
#pragma warning disable S6781 // JWT secret keys should not be disclosed
                testCertKey = new SymmetricSecurityKey("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
#pragma warning restore S6781 // JWT secret keys should not be disclosed
            }
            else
            {
                // The signing key from dotnet user-jwts is Base64-encoded
                testCertKey = new SymmetricSecurityKey(Convert.FromBase64String(signingKeyValue));
            }

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

                            // Read issuer and audience from configuration, with fallbacks for unit tests
                            var validIssuer = configuration["Authentication:Schemes:Bearer:ValidIssuer"];
                            if (string.IsNullOrEmpty(validIssuer))
                            {
                                validIssuer = string.IsNullOrEmpty(signingKeyValue) ? "Unit Test Issuer" : "dotnet-user-jwts";
                            }

                            // Read all valid audiences from configuration
                            var validAudiences = configuration.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>();
                            if (validAudiences == null || validAudiences.Length == 0)
                            {
                                validAudiences = string.IsNullOrEmpty(signingKeyValue)
                                    ? new[] { "Unit Test Audience" }
                                    : new[] { "https://localhost:7141" };
                            }

                            var validationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = validIssuer,
                                ValidateAudience = true,
                                ValidAudiences = validAudiences,
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
