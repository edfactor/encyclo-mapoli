using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Demoulas.ProfitSharing.Api.OpenApi;

internal sealed class OpenApiSecuritySchemeFixProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        OpenApiDocument document = context.Document;

        IDictionary<string, OpenApiSecurityScheme> securitySchemes = document.Components.SecuritySchemes;

        if (securitySchemes.TryGetValue("oauth2", out OpenApiSecurityScheme? oauth2Scheme))
        {
            securitySchemes["oauth2"] = new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Description = oauth2Scheme.Description,
                Flows = oauth2Scheme.Flows
            };
        }

        if (!securitySchemes.ContainsKey("bearer"))
        {
            securitySchemes["bearer"] = new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\""
            };
        }
    }
}
