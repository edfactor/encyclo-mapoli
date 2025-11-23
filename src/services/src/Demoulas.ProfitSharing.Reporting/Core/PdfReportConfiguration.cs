namespace Demoulas.ProfitSharing.Reporting.Core;

/// <summary>
/// Centralized configuration for PDF report styling, fonts, colors, and branding.
/// This ensures all reports maintain consistent look and feel with client-preferred formatting.
/// </summary>
public static class PdfReportConfiguration
{
    /// <summary>
    /// Logo embedded resource name. Located in Resources/mb_mfyd.png
    /// </summary>
    public const string EmbeddedLogoResourceName = "Demoulas.ProfitSharing.Reporting.Resources.mb_mfyd.png";

    /// <summary>
    /// Standard page margins (in inches)
    /// </summary>
    public static class PageMargins
    {
        public const float Top = 0.5f;
        public const float Bottom = 0.5f;
        public const float Left = 0.5f;
        public const float Right = 0.5f;
    }

    /// <summary>
    /// Standard font sizes and styling
    /// </summary>
    public static class FontSizes
    {
        public const int TitleSize = 14;
        public const int HeaderSize = 12;
        public const int LabelSize = 11;
        public const int ContentSize = 10;
        public const int FooterSize = 8;
        public const int TotalsSize = 11;
    }

    /// <summary>
    /// Brand colors used across reports
    /// </summary>
    public static class BrandColors
    {
        public static readonly string DemoulasBlue = "#0033AA"; // Company blue
        public static readonly string HeaderGray = "#F0F0F0";   // Light gray for headers
        public static readonly string TotalsGray = "#E8E8E8";   // Light gray for totals rows
        public static readonly string BorderGray = "#CCCCCC";   // Grid borders
        public static readonly string TextBlack = "#000000";    // Primary text
        public static readonly string TextDarkGray = "#333333"; // Secondary text
    }

    /// <summary>
    /// Report metadata (author, subject)
    /// </summary>
    public static class ReportMetadata
    {
        public const string Author = "Demoulas Smart Profit Sharing";
        public const string Subject = "Demoulas Supermarkets Inc.";
    }

    /// <summary>
    /// Standard spacing measurements (in inches)
    /// </summary>
    public static class Spacing
    {
        public const float SmallGap = 0.15f;       // Tighter spacing within sections
        public const float StandardGap = 0.25f;    // Standard gap between elements
        public const float LargeGap = 0.35f;       // Larger gap for visual separation
        public const float SectionBreak = 0.4f;    // Section break spacing
        public const float CoverPageGap = 0.5f;    // Large gap for cover page spacing
    }

    /// <summary>
    /// Table styling defaults
    /// </summary>
    public static class TableDefaults
    {
        public const float CellPaddingVertical = 0.18f;      // Vertical padding for better readability
        public const float CellPaddingHorizontal = 0.15f;    // Horizontal padding for breathing room
        public const float BorderWidth = 0.5f;
        public const float RowSpacing = 0.1f;               // Additional space between rows
        public const float HeaderPaddingVertical = 0.2f;    // Header row extra padding
    }

    /// <summary>
    /// Gets the embedded Market Basket logo as byte array from embedded resources
    /// </summary>
    /// <returns>Logo image bytes, or null if not found</returns>
    public static byte[]? GetEmbeddedLogo()
    {
        try
        {
            var assembly = typeof(PdfReportConfiguration).Assembly;
            using var stream = assembly.GetManifestResourceStream(EmbeddedLogoResourceName);
            if (stream is null)
                return null;

            byte[] logo = new byte[stream.Length];
            _ = stream.Read(logo, 0, logo.Length);
            return logo;
        }
        catch
        {
            // If logo loading fails, return null; report will degrade gracefully
            return null;
        }
    }
}
