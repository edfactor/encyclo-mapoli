using Demoulas.ProfitSharing.Reporting.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Reporting.Extensions;

/// <summary>
/// Dependency Injection extension methods for the Profit Sharing reporting module.
/// Configures QuestPDF and makes the PDF report generator available in the application.
/// </summary>
public static class ReportingServiceCollectionExtensions
{
    /// <summary>
    /// Adds Profit Sharing reporting services to the dependency injection container.
    /// 
    /// Usage in Program.cs:
    /// <code>
    /// services.AddProfitSharingReporting();
    /// </code>
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddProfitSharingReporting(this IServiceCollection services)
    {
        // Register QuestPDF license (if using commercial license)
        // QuestPDF.Settings.License = new QuestPDF.Infrastructure.LicenseKey("...");

        // Register the PDF report generator
        services.AddScoped<PdfReportGenerator>();

        return services;
    }
}
