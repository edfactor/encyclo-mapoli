using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demoulas.ProfitSharing.AppHost;

public static class ApiDocumentationResourceBuilderExtensions
{
    /// <summary>
    /// Configures the resource builder to include Swagger UI for API documentation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the resource that implements <see cref="IResourceWithEndpoints"/>.
    /// </typeparam>
    /// <param name="builder">
    /// The resource builder to configure.
    /// </param>
    /// <returns>
    /// The configured resource builder with Swagger UI enabled.
    /// </returns>
    public static IResourceBuilder<T> WithSwaggerUi<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("swagger-ui-docs", "Swagger UI Documentation", "swagger");
    }

    /// <summary>
    /// Configures the resource builder to use Redoc for API documentation.
    /// </summary>
    /// <typeparam name="T">The type of the resource that implements <see cref="IResourceWithEndpoints"/>.</typeparam>
    /// <param name="builder">The resource builder to configure.</param>
    /// <returns>The configured resource builder.</returns>
    public static IResourceBuilder<T> WithRedoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("redoc-ui-docs", "Redoc UI Documentation", "redoc");
    }

    /// <summary>
    /// Configures the resource builder to include scalar UI documentation.
    /// </summary>
    /// <typeparam name="T">The type of the resource that implements <see cref="IResourceWithEndpoints"/>.</typeparam>
    /// <param name="builder">The resource builder to configure.</param>
    /// <returns>The configured <see cref="IResourceBuilder{T}"/> instance.</returns>
    public static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("scalar-ui-docs", "Scalar UI Documentation", "scalar");
    }


    /// <summary>
    /// Configures the resource builder to include OpenAPI documentation with the specified parameters.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the resource that implements <see cref="IResourceWithEndpoints"/>.
    /// </typeparam>
    /// <param name="builder">
    /// The resource builder to configure.
    /// </param>
    /// <param name="name">
    /// The unique name for the OpenAPI documentation command.
    /// </param>
    /// <param name="displayName">
    /// The display name for the OpenAPI documentation command.
    /// </param>
    /// <param name="openApiUiPath">
    /// The relative path to the OpenAPI UI documentation.
    /// </param>
    /// <returns>
    /// The configured <see cref="IResourceBuilder{T}"/> instance.
    /// </returns>
    /// <remarks>
    /// This method adds a command to the resource builder that opens the specified OpenAPI documentation
    /// in the default web browser. The command's state is determined by the health status of the resource.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown if an error occurs while attempting to open the OpenAPI documentation.
    /// </exception>
    public static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder,
        string name,
        string displayName,
        string openApiUiPath)
        where T : IResourceWithEndpoints
    {
        return builder.WithCommand(name, displayName, executeCommand: _ =>
            {
                try
                {
                    var endpoint = builder.GetEndpoint("https");
                    var url = $"{endpoint.Url}/{openApiUiPath}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    return Task.FromResult(new ExecuteCommandResult { Success = true });
                }
                catch (Exception e)
                {
                    return Task.FromResult(new ExecuteCommandResult { Success = false, ErrorMessage = e.ToString() });
                }
            },
            commandOptions: new CommandOptions
            {
                UpdateState = context =>
                    context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy ? ResourceCommandState.Enabled : ResourceCommandState.Disabled,
                IconName = "Document",
                IconVariant = IconVariant.Filled
            });

    }

}
