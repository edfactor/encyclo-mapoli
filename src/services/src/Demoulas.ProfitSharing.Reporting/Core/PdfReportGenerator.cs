namespace Demoulas.ProfitSharing.Reporting.Core;

/// <summary>
/// Service for generating PDF reports from report instances.
/// Provides convenient methods for async PDF generation and file operations.
/// </summary>
public class PdfReportGenerator
{
    /// <summary>
    /// Generates PDF from a report instance asynchronously
    /// </summary>
    /// <param name="report">Report instance to generate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file contents as byte array</returns>
    public Task<byte[]> GeneratePdfAsync(BasePdfReport report, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => report.GeneratePdf(), cancellationToken);
    }

    /// <summary>
    /// Generates PDF and saves to file asynchronously
    /// </summary>
    /// <param name="report">Report instance to generate</param>
    /// <param name="filePath">Full path where PDF file will be saved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SavePdfAsync(BasePdfReport report, string filePath, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => report.SavePdf(filePath), cancellationToken);
    }

    /// <summary>
    /// Generates PDF and uploads to stream asynchronously
    /// </summary>
    /// <param name="report">Report instance to generate</param>
    /// <param name="outputStream">Stream to write PDF to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task WritePdfToStreamAsync(
        BasePdfReport report,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        byte[] pdf = await GeneratePdfAsync(report, cancellationToken);
        await outputStream.WriteAsync(pdf, 0, pdf.Length, cancellationToken);
    }
}
