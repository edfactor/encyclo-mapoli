using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Demoulas.ProfitSharing.Reporting.Core;

/// <summary>
/// Utility methods for common PDF report elements (headers, footers, logos, styling).
/// Centralizes formatting logic to ensure consistency across all reports.
/// </summary>
public static class PdfUtilities
{
    /// <summary>
    /// Creates a standard company header with logo and title
    /// </summary>
    /// <param name="container">IContainer to add header to</param>
    /// <param name="reportTitle">Title to display (e.g., "Profit Sharing Report")</param>
    /// <param name="showLogo">Whether to display Market Basket logo</param>
    public static void ComposeStandardHeader(
        this IContainer container,
        string reportTitle,
        bool showLogo = true)
    {
        container.Column(column =>
        {
            if (showLogo)
            {
                column.Item().Height(60).PaddingBottom(PdfReportConfiguration.Spacing.StandardGap)
                    .Row(row =>
                    {
                        // Left: Logo
                        row.ConstantItem(80).Element(c => ComposeLogoImage(c));

                        // Right: Title and date
                        row.RelativeItem()
                            .PaddingLeft(PdfReportConfiguration.Spacing.StandardGap)
                            .Element(c => ComposeHeaderTextBlock(c, reportTitle));
                    });
            }
            else
            {
                column.Item().Text(reportTitle)
                    .FontSize(PdfReportConfiguration.FontSizes.HeaderSize)
                    .Bold()
                    .FontColor(PdfReportConfiguration.BrandColors.DemoulasBlue);
            }

            // Horizontal rule
            column.Item().Height(1).PaddingBottom(PdfReportConfiguration.Spacing.StandardGap)
                .Background(PdfReportConfiguration.BrandColors.BorderGray);
        });
    }

    /// <summary>
    /// Renders the Market Basket logo image
    /// </summary>
    private static void ComposeLogoImage(IContainer container)
    {
        byte[]? logo = PdfReportConfiguration.GetEmbeddedLogo();
        if (logo != null)
        {
            container.Image(logo);
        }
        else
        {
            // Fallback: Display "DEMOULAS" text if logo unavailable
            container.AlignMiddle().Text("DEMOULAS")
                .FontSize(PdfReportConfiguration.FontSizes.TitleSize)
                .Bold()
                .FontColor(PdfReportConfiguration.BrandColors.DemoulasBlue);
        }
    }

    /// <summary>
    /// Composes header text block (title, generated date)
    /// </summary>
    private static void ComposeHeaderTextBlock(IContainer container, string reportTitle)
    {
        container.Column(column =>
        {
            column.Item().Text(reportTitle)
                .FontSize(PdfReportConfiguration.FontSizes.HeaderSize)
                .Bold()
                .FontColor(PdfReportConfiguration.BrandColors.DemoulasBlue);

            column.Item().PaddingTop(PdfReportConfiguration.Spacing.SmallGap)
                .Text(DateTime.Now.ToString("MM/dd/yyyy"))
                .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);
        });
    }

    /// <summary>
    /// Creates a standard section header (with optional background color)
    /// </summary>
    /// <param name="container">IContainer to add section header to</param>
    /// <param name="sectionTitle">Title of the section</param>
    /// <param name="backgroundColor">Optional background color (hex or named color)</param>
    public static void ComposeSectionHeader(
        this IContainer container,
        string sectionTitle,
        string? backgroundColor = null)
    {
        var headerContainer = container.PaddingVertical(PdfReportConfiguration.Spacing.SmallGap);

        if (!string.IsNullOrEmpty(backgroundColor))
        {
            headerContainer = headerContainer.Background(backgroundColor);
        }

        headerContainer.Text(sectionTitle)
            .FontSize(PdfReportConfiguration.FontSizes.LabelSize)
            .Bold()
            .FontColor(PdfReportConfiguration.BrandColors.TextBlack);
    }

    /// <summary>
    /// Creates a simple key-value pair display (label and value)
    /// </summary>
    /// <param name="container">IContainer to add pair to</param>
    /// <param name="label">Label/key text</param>
    /// <param name="value">Value text</param>
    /// <param name="bold">Whether to bold the value</param>
    public static void ComposeKeyValuePair(
        this IContainer container,
        string label,
        string value,
        bool bold = false)
    {
        container.Row(row =>
        {
            row.RelativeItem()
                .Text(label + ":")
                .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);

            row.RelativeItem()
                .AlignRight()
                .Element(c =>
                {
                    var text = c.Text(value)
                        .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                        .FontColor(PdfReportConfiguration.BrandColors.TextBlack);

                    if (bold)
                    {
                        text.Bold();
                    }
                });
        });
    }

    /// <summary>
    /// Creates a table header row with standardized styling
    /// </summary>
    /// <param name="container">IContainer to add row to</param>
    /// <param name="columns">Column headers</param>
    public static void ComposeTableHeaderRow(
        this IContainer container,
        params string[] columns)
    {
        container.Background(PdfReportConfiguration.BrandColors.HeaderGray)
            .PaddingVertical(PdfReportConfiguration.TableDefaults.CellPaddingVertical)
            .Row(row =>
            {
                foreach (var column in columns)
                {
                    row.RelativeItem()
                        .PaddingHorizontal(PdfReportConfiguration.TableDefaults.CellPaddingHorizontal)
                        .Text(column)
                        .FontSize(PdfReportConfiguration.FontSizes.LabelSize)
                        .Bold()
                        .FontColor(PdfReportConfiguration.BrandColors.TextBlack);
                }
            });
    }

    /// <summary>
    /// Creates a table data row with standardized styling
    /// </summary>
    /// <param name="container">IContainer to add row to</param>
    /// <param name="isAlternate">Whether to use alternate (slightly shaded) background</param>
    /// <param name="values">Column values</param>
    public static void ComposeTableDataRow(
        this IContainer container,
        bool isAlternate,
        params string[] values)
    {
        var rowContainer = container
            .PaddingVertical(PdfReportConfiguration.TableDefaults.CellPaddingVertical);

        if (isAlternate)
        {
            rowContainer = rowContainer.Background(PdfReportConfiguration.BrandColors.HeaderGray);
        }

        rowContainer.Row(row =>
        {
            foreach (var value in values)
            {
                row.RelativeItem()
                    .PaddingHorizontal(PdfReportConfiguration.TableDefaults.CellPaddingHorizontal)
                    .Text(value)
                    .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                    .FontColor(PdfReportConfiguration.BrandColors.TextBlack);
            }
        });
    }

    /// <summary>
    /// Creates a totals row (emphasized styling)
    /// </summary>
    /// <param name="container">IContainer to add totals row to</param>
    /// <param name="label">Totals label</param>
    /// <param name="value">Totals value</param>
    public static void ComposeTotalsRow(
        this IContainer container,
        string label,
        string value)
    {
        container.Background(PdfReportConfiguration.BrandColors.TotalsGray)
            .PaddingVertical(PdfReportConfiguration.TableDefaults.CellPaddingVertical)
            .Row(row =>
            {
                row.RelativeItem()
                    .PaddingHorizontal(PdfReportConfiguration.TableDefaults.CellPaddingHorizontal)
                    .Text(label)
                    .FontSize(PdfReportConfiguration.FontSizes.TotalsSize)
                    .Bold()
                    .FontColor(PdfReportConfiguration.BrandColors.TextBlack);

                row.RelativeItem()
                    .AlignRight()
                    .PaddingHorizontal(PdfReportConfiguration.TableDefaults.CellPaddingHorizontal)
                    .Text(value)
                    .FontSize(PdfReportConfiguration.FontSizes.TotalsSize)
                    .Bold()
                    .FontColor(PdfReportConfiguration.BrandColors.TextBlack);
            });
    }

    /// <summary>
    /// Creates a horizontal divider line
    /// </summary>
    /// <param name="container">IContainer to add divider to</param>
    /// <param name="thickness">Line thickness (default 1)</param>
    public static void ComposeDivider(this IContainer container, float thickness = 1)
    {
        container.PaddingVertical(PdfReportConfiguration.Spacing.SmallGap)
            .Height(thickness)
            .Background(PdfReportConfiguration.BrandColors.BorderGray);
    }

    /// <summary>
    /// Creates a section spacing element
    /// </summary>
    /// <param name="container">IContainer to add spacing to</param>
    public static void ComposeSectionBreak(this IContainer container)
    {
        container.Height(PdfReportConfiguration.Spacing.SectionBreak);
    }

    /// <summary>
    /// Formats a decimal value as currency string
    /// </summary>
    public static string ToCurrencyString(this decimal value)
    {
        return value.ToString("C2");
    }

    /// <summary>
    /// Formats a nullable decimal value as currency string or empty
    /// </summary>
    public static string ToCurrencyStringOrEmpty(this decimal? value)
    {
        return value?.ToString("C2") ?? "-";
    }

    /// <summary>
    /// Safely truncate text to a maximum length with ellipsis
    /// </summary>
    public static string TruncateWithEllipsis(this string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (text.Length <= maxLength)
            return text;

        return text[..(maxLength - 3)] + "...";
    }
}
