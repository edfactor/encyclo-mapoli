# Demoulas.ProfitSharing.Reporting

Professional PDF report generation wrapper for the Profit Sharing application using QuestPDF.

## Overview

This module provides a clean, maintainable wrapper around QuestPDF for generating consistent, branded PDF reports with:

-   **Standardized formatting**: Company colors, fonts, spacing, and layouts
-   **Embedded branding**: Market Basket logo as embedded resource (no file dependencies)
-   **Consistent headers/footers**: Professional page structure across all reports
-   **Reusable components**: Tables, key-value pairs, dividers, section headers
-   **Easy extension**: Simple base class pattern for creating new report types

## Quick Start

### 1. Register Services in Program.cs

```csharp
// Add reporting services
services.AddProfitSharingReporting();
```

### 2. Create Your First Report

Inherit from `BasePdfReport` and implement your custom content:

```csharp
using Demoulas.ProfitSharing.Reporting.Core;
using QuestPDF.Fluent;

public class MyCustomReport : BasePdfReport
{
    public override string Title => "My Report Title";
    public override string ReportName => "my-custom-report";

    protected override void ComposeHeader(IContainer header)
    {
        header.ComposeStandardHeader("My Report Title", showLogo: true);
    }

    protected override void ComposeContent(IContainer content)
    {
        content.Column(column =>
        {
            column.Item().Text("Report content goes here");
            column.Item().ComposeSectionBreak();

            // Use utility methods for common elements
            column.Item().ComposeSectionHeader("Section Title");
            column.Item().ComposeKeyValuePair("Label", "Value");
        });
    }
}
```

### 3. Generate PDF

In your endpoint or service:

```csharp
// Inject the generator
public class MyEndpoint : Endpoint<MyRequest, Results<Ok<byte[]>, NotFound>>
{
    private readonly PdfReportGenerator _pdfGenerator;

    public MyEndpoint(PdfReportGenerator pdfGenerator)
    {
        _pdfGenerator = pdfGenerator;
    }

    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        // Create report instance with your data
        var report = new MyCustomReport();

        // Generate PDF
        byte[] pdf = await _pdfGenerator.GeneratePdfAsync(report, ct);

        // Return to client
        return Results.Ok(pdf);
    }
}
```

## Core Components

### PdfReportConfiguration

Centralized configuration for all report styling:

```csharp
// Access brand colors
string blue = PdfReportConfiguration.BrandColors.DemoulasBlue;

// Access font sizes
int headerSize = PdfReportConfiguration.FontSizes.HeaderSize;

// Load the embedded logo
byte[]? logo = PdfReportConfiguration.GetEmbeddedLogo();
```

### BasePdfReport

Abstract base class that all reports inherit from:

```csharp
public abstract class BasePdfReport : IDocument
{
    public abstract string Title { get; }
    public abstract string ReportName { get; }

    // Optional overrides
    public virtual DateTime GeneratedOn => DateTime.Now;
    public virtual string GeneratedBy => "Profit Sharing System";
    public virtual bool IncludePageNumbers => true;
    public virtual bool IncludeCompanyFooter => true;

    // Must implement
    protected abstract void ComposeContent(IContainer content);

    // Optional override
    protected virtual void ComposeHeader(IContainer header) { }

    // Optional override
    protected virtual void ComposeFooter(IContainer footer) { }

    // Generate PDF
    public byte[] GeneratePdf() { }
    public void SavePdf(string filePath) { }
}
```

### PdfUtilities

Extension methods for common report elements:

```csharp
// Header with logo
container.ComposeStandardHeader("Report Title", showLogo: true);

// Section headers
container.ComposeSectionHeader("My Section");

// Key-value pairs
container.ComposeKeyValuePair("Fiscal Year", "2025", bold: true);

// Table styling
container.ComposeTableHeaderRow("Column 1", "Column 2", "Column 3");
container.ComposeTableDataRow(isAlternate: false, "Value 1", "Value 2", "Value 3");
container.ComposeTotalsRow("TOTALS", "$1,234.56");

// Dividers and spacing
container.ComposeDivider();
container.ComposeSectionBreak();

// Number formatting
decimal amount = 1234.56m;
string formatted = amount.ToCurrencyString();  // "$1,234.56"
```

### PdfReportGenerator

Service for async PDF generation:

```csharp
private readonly PdfReportGenerator _generator;

// Generate as byte array
byte[] pdf = await _generator.GeneratePdfAsync(report, cancellationToken);

// Save to file
await _generator.SavePdfAsync(report, "/path/to/file.pdf", cancellationToken);

// Write to stream
await _generator.WritePdfToStreamAsync(report, stream, cancellationToken);
```

## Design & Styling

### Brand Colors

Defined in `PdfReportConfiguration.BrandColors`:

-   **DemoulasBlue** (#0033AA) - Primary brand color for titles and headers
-   **HeaderGray** (#F0F0F0) - Light gray for table headers and backgrounds
-   **TotalsGray** (#E8E8E8) - Light gray for totals rows
-   **BorderGray** (#CCCCCC) - Grid and divider lines
-   **TextBlack** (#000000) - Primary text color
-   **TextDarkGray** (#333333) - Secondary text color

### Font Sizes

Defined in `PdfReportConfiguration.FontSizes`:

-   TitleSize = 14 pt
-   HeaderSize = 12 pt
-   LabelSize = 11 pt
-   ContentSize = 10 pt
-   FooterSize = 8 pt
-   TotalsSize = 11 pt

### Spacing

Consistent spacing throughout reports (all in inches):

-   SmallGap = 0.1"
-   StandardGap = 0.2"
-   LargeGap = 0.3"
-   SectionBreak = 0.5"

### Page Layout

-   Margins: 0.5" on all sides
-   Header: Company branding and page title
-   Content: Main report body
-   Footer: Generation timestamp, user, and page numbers

## Advanced Usage

### Multi-Page Reports

The base class automatically handles pagination. Just add content:

```csharp
protected override void ComposeContent(IContainer content)
{
    content.Column(column =>
    {
        for (int i = 0; i < 100; i++)
        {
            column.Item().Text($"Item {i}");
            // QuestPDF handles page breaks automatically
        }
    });
}
```

### Custom Headers Per Section

Override `ComposeHeader()` to customize by page:

```csharp
protected override void ComposeHeader(IContainer header)
{
    if (pageNumber == 1)
    {
        header.ComposeStandardHeader("First Page Title");
    }
    else
    {
        header.Text("Continuation...").FontSize(10);
    }
}
```

### Hiding Standard Elements

Control footer and page numbers:

```csharp
public override bool IncludePageNumbers => false;
public override bool IncludeCompanyFooter => false;
```

### Custom Metadata

Override generation info:

```csharp
public override DateTime GeneratedOn => new DateTime(2025, 1, 15, 14, 30, 0);
public override string GeneratedBy => "Year-End Batch Process";
```

## Examples

See `Examples/SampleProfitSharingReport.cs` for a complete working example.

## File Structure

```
Demoulas.ProfitSharing.Reporting/
├── Core/
│   ├── PdfReportConfiguration.cs      # Centralized styling config
│   ├── BasePdfReport.cs               # Base class for all reports
│   ├── PdfUtilities.cs                # Helper extension methods
│   └── PdfReportGenerator.cs          # PDF generation service
├── Examples/
│   └── SampleProfitSharingReport.cs   # Example implementation
├── Extensions/
│   └── ReportingServiceCollectionExtensions.cs  # DI setup
├── Resources/
│   └── mb_mfyd.png                    # Embedded logo
└── Demoulas.ProfitSharing.Reporting.csproj
```

## Testing

When testing reports:

1. Generate PDF with test data
2. Verify file creates successfully
3. Open in PDF viewer to validate formatting
4. Check embedded resource loading (logo appears)
5. Verify page breaks on large datasets

Example test:

```csharp
[Fact]
public void MyReport_GeneratesPdf_WithoutErrors()
{
    // Arrange
    var report = new MyCustomReport();

    // Act
    byte[] pdf = report.GeneratePdf();

    // Assert
    Assert.NotNull(pdf);
    Assert.True(pdf.Length > 0);
    Assert.True(pdf.AsSpan().StartsWith(new byte[] { 0x25, 0x50, 0x44, 0x46 })); // PDF signature
}
```

## QuestPDF License

This project uses QuestPDF. For production use beyond the community license limits, see: https://www.questpdf.com/pricing.html

Current setup uses Community License (free for < 5 developers).

## Documentation

-   **QuestPDF Docs**: https://www.questpdf.com/getting-started.html
-   **This Module**: See code comments in core classes
-   **Examples**: See SampleProfitSharingReport.cs

## Migration from Legacy Reporting

If migrating from Demoulas.AccountsReceivable.Reporting:

1. Create new report class inheriting from `BasePdfReport`
2. Implement `ComposeContent()` method
3. Replace legacy `IExportableDocument` patterns with `BasePdfReport`
4. Use `PdfUtilities` for common elements instead of custom formatting
5. Update endpoint to use `PdfReportGenerator` service

## Performance Notes

-   PDF generation runs synchronously on thread pool via `PdfReportGenerator`
-   Large reports (1000+ pages) may take 2-5 seconds
-   Embedded logo (mb_mfyd.png) is loaded once per report instance
-   No external dependencies (logo is embedded resource)

## Security

-   Reports contain no hardcoded secrets
-   PII should be masked before passing to report
-   Generated PDFs are not cached on disk (memory-only)
-   Sensitive operations should audit which reports were generated

## Support

For issues:

1. Check QuestPDF documentation: https://www.questpdf.com
2. Review SampleProfitSharingReport.cs for examples
3. Check PdfUtilities for available helper methods
4. Verify logo is in Resources/ folder as embedded resource
