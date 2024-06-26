using System.Configuration;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Configuration;
using Elastic.CommonSchema;
using FastEndpoints.Swagger;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring Swagger.
/// </summary>
/// <remarks>
/// This class includes methods for adding Swagger to the WebApplicationBuilder and for setting up the security scheme.
/// </remarks>
internal static class SwaggerHelper
{
    internal static WebApplicationBuilder AddSwaggerOpenApi(this WebApplicationBuilder builder, byte version = 1,
        string title = "Demoulas Super Markets, Inc",
        string? description = null,
        bool enableJwtBearerAuth = true,
        OpenApiContact? contactDetails = null,
        Action<Dictionary<string, string>>? tagDescriptions = null,
    Action<AspNetCoreOpenApiDocumentGeneratorSettings>? documentSettings = null )
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            description = $"Environment Name : {builder.Environment.EnvironmentName}";
        }

        // Load Okta settings
        const string notSet = "Not Set";
        OktaSettings oktaSettings = builder.Configuration.GetSection("Okta").Get<OktaSettings>() ?? new OktaSettings
        {
            AuthorizationEndpoint = notSet,
            ClientId = notSet,
            Issuer = notSet,
            TokenEndpoint = notSet
        };

        // Add Okta settings to the DI container
       _ = builder.Services.AddSingleton(oktaSettings);

        _ = builder.Services.SwaggerDocument(o =>
         {
             if (version > 1)
             {
                 o.MinEndpointVersion = version;
                 o.MaxEndpointVersion = version;
             }

             o.TagCase = TagCase.LowerCase;
             o.ExcludeNonFastEndpoints = !Debugger.IsAttached;
             o.ShortSchemaNames = true;
             o.AutoTagPathSegmentIndex = 0;
             o.TagDescriptions = tagDescriptions;
             o.EnableJWTBearerAuth = enableJwtBearerAuth;
             o.DocumentSettings = (s =>
             {
                 s.DocumentName = $"Release {version}.0";
                 s.Title = title;
                 s.Version = $"v{version}.0";
                 s.Description = description;
                 s.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
                 s.AddSecurity("oauth2", new OpenApiSecurityScheme
                 {
                     Type = OpenApiSecuritySchemeType.OAuth2,
                     Flows = new OpenApiOAuthFlows
                     {
                         AuthorizationCode = new OpenApiOAuthFlow
                         {
                             AuthorizationUrl = $"{oktaSettings.AuthorizationEndpoint}",
                             TokenUrl = $"{oktaSettings.TokenEndpoint}",
                             Scopes = new Dictionary<string, string>
                             {
                                 { "openid", "OpenID Connect scope" },
                                 { "profile", "Profile scope" },
                                 { "email", "Email scope" }
                             }
                         }
                     }
                 });
                 s.PostProcess = doc => { doc.Produces = new List<string> { "application/json" }; };
                 s.PostProcess = document =>
                 {
                     document.Info.Contact = contactDetails ?? new OpenApiContact
                     {
                         Name = "NGDS Development Team", Email = "rfbreakey@demoulasmarketbasket.com"
                     };
                 };
             }) + documentSettings;
         });

        return builder;
    }
}
