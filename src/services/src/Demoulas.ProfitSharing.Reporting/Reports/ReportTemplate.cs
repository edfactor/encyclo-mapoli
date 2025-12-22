using Demoulas.ProfitSharing.Reporting.Core;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Demoulas.ProfitSharing.Reporting.Reports;

/// <summary>
/// TEMPLATE: Copy this file and customize for new reports.
/// 
/// Instructions:
/// 1. Copy this file: ReportTemplate.cs -> YourReportName.cs
/// 2. Rename class: ReportTemplate -> YourReportName
/// 3. Update Title and ReportName properties
/// 4. Add your data properties to constructor
/// 5. Implement ComposeContent() with your layout
/// 6. Delete this comment block
/// 
/// Documentation: See README.md and QUICK_REFERENCE.md
/// </summary>
public class ReportTemplate : BasePdfReport
{
    private readonly string _reportDescription;

    public override string Title => "Report Template Title";
    public override string ReportName => "report-template";

    public ReportTemplate(string reportDescription = "Template Report")
    {
        _reportDescription = reportDescription;
    }

    /// <summary>
    /// Optional: Customize the page header.
    /// Called for each page automatically.
    /// Default: empty (no header)
    /// </summary>
    protected override void ComposeHeader(IContainer header)
    {
        // Use standard branding header
        header.ComposeStandardHeader(Title, showLogo: true);
    }

    /// <summary>
    /// REQUIRED: Implement your report content here.
    /// Use the column layout for vertical stacking of elements.
    /// </summary>
    protected override void ComposeContent(IContainer content)
    {
        content.Column(column =>
        {
            // Report metadata/info section
            column.Item().Element(ComposeInfoSection);

            column.Item().ComposeSectionBreak();

            // Main content section
            column.Item().Element(ComposeMainSection);

            column.Item().ComposeSectionBreak();

            // Footer/notes section
            column.Item().Element(ComposeFooterNotesSection);
        });
    }

    /// <summary>
    /// Composes the report information section (title, dates, etc.)
    /// </summary>
    private void ComposeInfoSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Report Information");

            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.SmallGap)
                .Element(c =>
                {
                    c.Column(col =>
                    {
                        col.Item().ComposeKeyValuePair("Report Type", Title);
                        col.Item().ComposeKeyValuePair("Generated", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                        col.Item().ComposeKeyValuePair("Description", _reportDescription);
                    });
                });
        });
    }

    /// <summary>
    /// Composes the main report content section
    /// Replace with your actual content
    /// </summary>
    private static void ComposeMainSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Main Content");

            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.StandardGap)
                .Element(c =>
                {
                    c.Column(col =>
                    {
                        // Example: Simple key-value display
                        col.Item().ComposeKeyValuePair("Item 1", "Value 1");
                        col.Item().ComposeKeyValuePair("Item 2", "Value 2");
                        col.Item().ComposeKeyValuePair("Total", "$1,000.00", bold: true);

                        // Example: Table display
                        col.Item().PaddingTop(PdfReportConfiguration.Spacing.StandardGap).Element(ComposeExampleTable);
                    });
                });
        });
    }

    /// <summary>
    /// Example table for reference - replace with your table logic
    /// </summary>
    private static void ComposeExampleTable(IContainer container)
    {
        container.Column(column =>
        {
            // Table header
            column.Item().ComposeTableHeaderRow("Name", "Amount", "Status");

            // Table rows
            column.Item().ComposeTableDataRow(false, "Item A", "$500.00", "Active");
            column.Item().ComposeTableDataRow(true, "Item B", "$300.00", "Inactive");
            column.Item().ComposeTableDataRow(false, "Item C", "$200.00", "Active");

            // Table totals
            column.Item().ComposeTotalsRow("TOTALS", "$1,000.00");
        });
    }

    /// <summary>
    /// Composes footer notes/disclaimers section
    /// </summary>
    private static void ComposeFooterNotesSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Notes");

            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.SmallGap)
                .Text("This is a template report. Replace this text with actual report footer notes and disclaimers as needed.")
                .FontSize(PdfReportConfiguration.FontSizes.FooterSize)
                .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);
        });
    }
}

