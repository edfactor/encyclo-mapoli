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
        public const float SmallGap = 0.15f;       // Spacing within sections (increased for readability)
        public const float StandardGap = 0.20f;    // Standard gap between elements (increased for readability)
        public const float LargeGap = 0.30f;       // Larger gap for visual separation (increased for readability)
        public const float SectionBreak = 0.35f;   // Section break spacing (increased for better visual separation)
        public const float CoverPageGap = 0.3f;    // Gap for cover page spacing
    }

    /// <summary>
    /// Table styling defaults
    /// </summary>
    public static class TableDefaults
    {
        public const float CellPaddingVertical = 0.18f;     // Vertical padding (increased for readability)
        public const float CellPaddingHorizontal = 0.15f;   // Horizontal padding (increased for readability)
        public const float BorderWidth = 0.5f;
        public const float RowSpacing = 0.10f;              // Spacing between rows (increased for readability)
        public const float HeaderPaddingVertical = 0.20f;   // Header padding (increased for better visual hierarchy)
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
            {
                return null;
            }

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
