using Demoulas.ProfitSharing.Api.OpenApi;
using NSwag.AspNetCore;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class OpenApiSecuritySchemeFixExtensions
{
    public static void TryAddOpenApiSecuritySchemeFixProcessor(this IServiceProvider services)
    {
        OpenApiDocumentRegistration[] registrations = services.GetServices<OpenApiDocumentRegistration>().ToArray();

        foreach (OpenApiDocumentRegistration reg in registrations)
        {
            object? generatorSettings =
                reg.GetType().GetProperty("GeneratorSettings")?.GetValue(reg)
                ?? reg.GetType().GetProperty("Settings")?.GetValue(reg)
                ?? reg.GetType().GetProperty("DocumentGeneratorSettings")?.GetValue(reg);

            if (generatorSettings is null)
            {
                continue;
            }

            object? documentProcessorsObj = generatorSettings.GetType().GetProperty("DocumentProcessors")?.GetValue(generatorSettings);
            if (documentProcessorsObj is not System.Collections.IEnumerable documentProcessors)
            {
                continue;
            }

            bool alreadyAdded = documentProcessors.Cast<object>().Any(processor => processor is OpenApiSecuritySchemeFixProcessor);
            if (alreadyAdded)
            {
                continue;
            }

            var addMethod = documentProcessorsObj.GetType().GetMethod("Add");
            if (addMethod is null)
            {
                continue;
            }

            _ = addMethod.Invoke(documentProcessorsObj, [new OpenApiSecuritySchemeFixProcessor()]);
        }
    }
}
