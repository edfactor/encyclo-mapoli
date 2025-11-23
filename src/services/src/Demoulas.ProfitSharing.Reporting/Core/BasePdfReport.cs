using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Demoulas.ProfitSharing.Reporting.Core;

/// <summary>
/// Abstract base class for all PDF reports in the Profit Sharing application.
/// Provides consistent structure: header, content, and footer with standardized formatting.
/// 
/// Usage:
/// <code>
/// public class MyReport : BasePdfReport
/// {
///     public override string Title => "My Report";
///     public override string ReportName => "my-report";
///     
///     protected override void ComposeHeader(IContainer header)
///     {
///         header.Text("My Custom Header").FontSize(14).Bold();
///     }
///     
///     protected override void ComposeContent(IContainer content)
///     {
///         content.Text("Report content goes here");
///     }
/// }
/// 
/// var report = new MyReport();
/// byte[] pdf = report.GeneratePdf();
/// </code>
/// </summary>
public abstract class BasePdfReport : IDocument
{
    /// <summary>
    /// Report title (appears in document metadata and optionally in headers)
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Report name for identification (e.g., "profit-sharing-statement")
    /// Used in file naming and logging
    /// </summary>
    public abstract string ReportName { get; }

    /// <summary>
    /// Optional: Report generation date/time stamp. Override to customize.
    /// </summary>
    public virtual DateTime GeneratedOn => DateTime.Now;

    /// <summary>
    /// Optional: User name or system that generated this report. Override to customize.
    /// </summary>
    public virtual string GeneratedBy => "Profit Sharing System";

    /// <summary>
    /// Optional: Include page numbers in footer. Default is true.
    /// </summary>
    public virtual bool IncludePageNumbers => true;

    /// <summary>
    /// Optional: Include footer with company info. Default is true.
    /// </summary>
    public virtual bool IncludeCompanyFooter => true;

    /// <summary>
    /// QuestPDF document metadata (title, author, subject)
    /// </summary>
    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = Title,
        Author = PdfReportConfiguration.ReportMetadata.Author,
        Subject = PdfReportConfiguration.ReportMetadata.Subject
    };

    /// <summary>
    /// Main document composition method called by QuestPDF
    /// Structures the document as: Header -> Content -> Footer
    /// </summary>
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.MarginVertical(PdfReportConfiguration.PageMargins.Top);
            page.MarginHorizontal(PdfReportConfiguration.PageMargins.Left);

            // Header section
            page.Header().Element(ComposeHeaderElement);

            // Main content area
            page.Content().Element(ComposeContentElement);

            // Footer section (optional)
            if (IncludePageNumbers || IncludeCompanyFooter)
            {
                page.Footer().Element(ComposeFooterElement);
            }
        });
    }

    /// <summary>
    /// Composes the page header. Override to customize.
    /// Default implementation is empty; child classes should override.
    /// </summary>
    protected virtual void ComposeHeader(IContainer header)
    {
        // Default: no header. Child classes should override to add custom headers.
    }

    /// <summary>
    /// Composes the main report content. Child classes MUST override.
    /// </summary>
    protected abstract void ComposeContent(IContainer content);

    /// <summary>
    /// Composes the page footer. Override to customize.
    /// Default implementation shows generation date/user and page numbers.
    /// </summary>
    protected virtual void ComposeFooter(IContainer footer)
    {
        footer.Row(row =>
        {
            // Left: Generation info
            row.RelativeItem().Text(c =>
            {
                c.Span($"Generated: {GeneratedOn:MM/dd/yyyy HH:mm} by {GeneratedBy}")
                    .FontSize(PdfReportConfiguration.FontSizes.FooterSize);
            });

            // Right: Page numbers (if enabled)
            if (IncludePageNumbers)
            {
                row.RelativeItem().AlignRight().Text(c =>
                {
                    c.CurrentPageNumber()
                        .FontSize(PdfReportConfiguration.FontSizes.FooterSize);
                });
            }
        });
    }

    /// <summary>
    /// Internal wrapper for header composition
    /// </summary>
    private void ComposeHeaderElement(IContainer container)
    {
        container.PaddingBottom(PdfReportConfiguration.Spacing.StandardGap);
        ComposeHeader(container);
    }

    /// <summary>
    /// Internal wrapper for content composition
    /// </summary>
    private void ComposeContentElement(IContainer container)
    {
        ComposeContent(container);
    }

    /// <summary>
    /// Internal wrapper for footer composition
    /// </summary>
    private void ComposeFooterElement(IContainer container)
    {
        container.PaddingTop(PdfReportConfiguration.Spacing.StandardGap);
        ComposeFooter(container);
    }

    /// <summary>
    /// Generates the PDF document as a byte array
    /// </summary>
    /// <returns>PDF file contents as byte array</returns>
    public byte[] GeneratePdf()
    {
        try
        {
            return QuestPDF.Fluent.Document.Create(container => container.Page(page =>
            {
                page.MarginVertical(PdfReportConfiguration.PageMargins.Top);
                page.MarginHorizontal(PdfReportConfiguration.PageMargins.Left);

                page.Header().Element(ComposeHeaderElement);
                page.Content().Element(ComposeContentElement);

                if (IncludePageNumbers || IncludeCompanyFooter)
                {
                    page.Footer().Element(ComposeFooterElement);
                }
            })).GeneratePdf();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate PDF for report '{ReportName}'", ex);
        }
    }

    /// <summary>
    /// Generates the PDF document to a file
    /// </summary>
    /// <param name="filePath">Full path where PDF file will be saved</param>
    public void SavePdf(string filePath)
    {
        byte[] pdf = GeneratePdf();
        File.WriteAllBytes(filePath, pdf);
    }
}
