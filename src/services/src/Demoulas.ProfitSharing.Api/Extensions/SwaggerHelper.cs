using System.Diagnostics;
using FastEndpoints.Swagger;
using NSwag;
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
        string title = "DeMoulas Super Markets, Inc",
        string? description = null,
        OpenApiContact? contactDetails = null,
        Action<Dictionary<string, string>>? tagDescriptions = null)
    {
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
             o.EnableJWTBearerAuth = false;
             o.DocumentSettings = s =>
             {
                 s.DocumentName = $"Release {version}.0";
                 s.Title = title;
                 s.Version = $"v{version}.0";
                 s.Description = description;
                 s.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
                 s.PostProcess = doc =>
                 {
                     doc.Produces = new List<string>
                     {
                        "application/json"
                     };
                 };
                 s.PostProcess = document =>
                 {
                     document.Info.Contact = contactDetails ?? new OpenApiContact
                     {
                         Name = "NGDS Core Team",
                         Email = "rharding@demoulasmarketbasket.com"
                     };
                 };
             };
         });

        return builder;
    }
}
