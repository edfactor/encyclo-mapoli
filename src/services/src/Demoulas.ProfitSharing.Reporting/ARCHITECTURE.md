# PDF Reporting Architecture & Migration Guide

## Architecture Overview

The Profit Sharing Reporting module provides a clean, extensible PDF report generation system using QuestPDF.

### Design Principles

1. **Separation of Concerns**: Styling/configuration separate from report content
2. **Consistency**: Centralized branding, fonts, colors, spacing
3. **Reusability**: Common elements available as extension methods
4. **Extensibility**: Simple base class for creating new report types
5. **Simplicity**: Minimal boilerplate for creating new reports

### Component Hierarchy

```
┌─────────────────────────────────────────────────────────────┐
│                   Your Report Class                          │
│            (extends BasePdfReport)                           │
└─────────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│              BasePdfReport (Abstract Base)                   │
│  • Manages page layout and structure                        │
│  • Handles headers, content, footers                        │
│  • Generates PDF output                                     │
└─────────────────────────────────────────────────────────────┘
                           │
         ┌─────────────────┼─────────────────┐
         ▼                 ▼                 ▼
    ┌─────────┐    ┌─────────────┐    ┌──────────┐
    │PdfUtils │    │  Config     │    │Generator │
    │         │    │             │    │          │
    │Tables   │    │Colors       │    │Async     │
    │Headers  │    │Fonts        │    │Stream I/O│
    │Footers  │    │Spacing      │    │File Save │
    │Dividers │    │Logo loading │    │          │
    └─────────┘    └─────────────┘    └──────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │  QuestPDF (Library)  │
                │  • Document Gen      │
                │  • Layout Engine     │
                │  • PDF Rendering     │
                └──────────────────────┘
```

---

## File Structure

```
Demoulas.ProfitSharing.Reporting/
│
├── Core/                                    # Core wrapper components
│   ├── PdfReportConfiguration.cs            # Centralized styling config
│   ├── BasePdfReport.cs                     # Base class for all reports
│   ├── PdfUtilities.cs                      # Helper extension methods
│   └── PdfReportGenerator.cs                # Async PDF generation service
│
├── Extensions/
│   └── ReportingServiceCollectionExtensions.cs  # DI registration
│
├── Examples/
│   └── SampleProfitSharingReport.cs         # Complete working example
│
├── Reports/                                 # Actual report implementations
│   ├── ReportTemplate.cs                    # Template for new reports
│   └── [YourReportName].cs                  # Your custom reports here
│
├── Resources/
│   └── mb_mfyd.png                          # Embedded Market Basket logo
│
├── README.md                                # Full documentation
├── QUICK_REFERENCE.md                       # Developer cheat sheet
└── Demoulas.ProfitSharing.Reporting.csproj  # Project file
```

---

## Core Classes Explained

### 1. PdfReportConfiguration

**Purpose**: Centralized styling and branding configuration

**Contains**:

-   Brand colors (DemoulasBlue, HeaderGray, etc.)
-   Font sizes (TitleSize, ContentSize, FooterSize, etc.)
-   Page margins and spacing measurements
-   Embedded logo resource loader
-   Report metadata (author, subject)

**Usage**:

```csharp
// Access colors
var blue = PdfReportConfiguration.BrandColors.DemoulasBlue;

// Get logo
byte[]? logo = PdfReportConfiguration.GetEmbeddedLogo();

// Use spacing
float gap = PdfReportConfiguration.Spacing.StandardGap;
```

**Benefits**:

-   Change colors/fonts globally (1 file)
-   Consistent spacing throughout app
-   Logo as embedded resource (no file dependencies)
-   Self-documenting style constants

### 2. BasePdfReport

**Purpose**: Abstract base class all reports inherit from

**Structure**:

```
Document Page Layout:
┌────────────────────────────┐
│   ComposeHeader()          │  ← Override for custom headers
├────────────────────────────┤
│                            │
│  ComposeContent()          │  ← MUST implement in subclass
│  (Main Report Content)     │
│                            │
├────────────────────────────┤
│   ComposeFooter()          │  ← Optional override for custom footer
└────────────────────────────┘
```

**Key Methods**:

-   `GeneratePdf()` - Synchronously generate PDF (returns byte[])
-   `SavePdf(path)` - Save PDF to file
-   `ComposeHeader()` - Override to customize page headers
-   `ComposeContent()` - MUST implement with report content
-   `ComposeFooter()` - Override to customize page footers

**Properties**:

-   `Title` - Document title (metadata)
-   `ReportName` - Report identifier
-   `GeneratedOn` - Timestamp (override as needed)
-   `GeneratedBy` - Who generated it
-   `IncludePageNumbers` - Show page numbers
-   `IncludeCompanyFooter` - Show generation info

**Example**:

```csharp
public class MyReport : BasePdfReport
{
    public override string Title => "My Report";
    public override string ReportName => "my-report";

    protected override void ComposeContent(IContainer content)
    {
        content.Column(col =>
        {
            col.Item().Text("Report content");
        });
    }
}
```

### 3. PdfUtilities

**Purpose**: Extension methods for common report elements

**Key Extensions**:

-   `ComposeStandardHeader()` - Logo + title header
-   `ComposeSectionHeader()` - Section titles
-   `ComposeKeyValuePair()` - Label + value display
-   `ComposeTableHeaderRow()` - Table headers
-   `ComposeTableDataRow()` - Table data rows
-   `ComposeTotalsRow()` - Totals rows (emphasized)
-   `ComposeDivider()` - Horizontal line
-   `ComposeSectionBreak()` - Vertical spacing

**Number Formatting**:

-   `ToCurrencyString()` - Format decimal as "$X.XX"
-   `ToCurrencyStringOrEmpty()` - Format with "-" fallback
-   `TruncateWithEllipsis()` - Safely truncate long text

**Benefits**:

-   Consistent table styling across reports
-   No need to define colors/fonts repeatedly
-   Type-safe and IntelliSense-friendly
-   Self-documenting method names

### 4. PdfReportGenerator

**Purpose**: Service for async PDF generation

**Methods**:

-   `GeneratePdfAsync()` - Generate PDF asynchronously
-   `SavePdfAsync()` - Generate and save to file
-   `WritePdfToStreamAsync()` - Generate and write to stream

**Benefits**:

-   Async/await support for endpoint integration
-   No blocking on report generation
-   Stream support for HTTP responses
-   Cancellation token support

---

## Usage Patterns

### Pattern 1: Generate in Endpoint

```csharp
public class GetReportEndpoint : Endpoint<GetReportRequest, Results<Ok<byte[]>, NotFound>>
{
    private readonly PdfReportGenerator _pdfGenerator;
    private readonly IReportDataService _dataService;

    public override async Task HandleAsync(GetReportRequest req, CancellationToken ct)
    {
        // Get data
        var data = await _dataService.GetDataAsync(ct);

        // Create report
        var report = new MyReport(data);

        // Generate
        byte[] pdf = await _pdfGenerator.GeneratePdfAsync(report, ct);

        // Return
        return Results.File(pdf, "application/pdf", "report.pdf");
    }
}
```

### Pattern 2: Save to Disk

```csharp
// Batch process - generate multiple reports
foreach (var item in items)
{
    var report = new MyReport(item);
    await _pdfGenerator.SavePdfAsync(
        report,
        $"/output/{item.Id}.pdf",
        ct
    );
}
```

### Pattern 3: Stream to Client

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var report = new MyReport();

    // Write directly to response stream
    HttpContext.Response.ContentType = "application/pdf";
    HttpContext.Response.Headers.Add(
        "Content-Disposition",
        "attachment; filename=report.pdf"
    );

    await _pdfGenerator.WritePdfToStreamAsync(report, HttpContext.Response.Body, ct);
}
```

---

## Creating New Reports

### Step 1: Create Report Class

Copy `Reports/ReportTemplate.cs`:

```csharp
public class MyCustomReport : BasePdfReport
{
    public override string Title => "My Custom Report";
    public override string ReportName => "my-custom-report";

    protected override void ComposeContent(IContainer content)
    {
        // Implement your layout
    }
}
```

### Step 2: Add Data Properties

```csharp
private readonly string _fiscalYear;
private readonly List<DataItem> _items;

public MyCustomReport(string fiscalYear, List<DataItem> items)
{
    _fiscalYear = fiscalYear;
    _items = items;
}
```

### Step 3: Implement ComposeContent

```csharp
protected override void ComposeContent(IContainer content)
{
    content.Column(column =>
    {
        column.Item().Element(ComposeHeader);
        column.Item().ComposeSectionBreak();
        column.Item().Element(ComposeTable);
        column.Item().ComposeSectionBreak();
        column.Item().Element(ComposeFooter);
    });
}

private void ComposeHeader(IContainer container)
{
    container.Column(col =>
    {
        col.Item().ComposeSectionHeader("Report Data");
        col.Item().ComposeKeyValuePair("Year", _fiscalYear);
    });
}

private void ComposeTable(IContainer container)
{
    container.Column(col =>
    {
        col.Item().ComposeTableHeaderRow("Column 1", "Column 2");

        bool alternate = false;
        foreach (var item in _items)
        {
            col.Item().ComposeTableDataRow(
                alternate,
                item.Value1,
                item.Value2
            );
            alternate = !alternate;
        }
    });
}
```

### Step 4: Use in Endpoint

```csharp
byte[] pdf = await _pdfGenerator.GeneratePdfAsync(
    new MyCustomReport(_fiscalYear, _items),
    ct
);
return Results.File(pdf, "application/pdf", "report.pdf");
```

---

## Styling & Customization

### Global Changes

Edit `PdfReportConfiguration.cs`:

```csharp
// Change all report titles to red
public static class BrandColors
{
    public static readonly string DemoulasBlue = "#FF0000"; // Changed!
}

// Change all content font size
public static class FontSizes
{
    public const int ContentSize = 11; // Was 10
}
```

### Per-Report Changes

Override in report class:

```csharp
public class MyReport : BasePdfReport
{
    protected override void ComposeHeader(IContainer header)
    {
        header.Text("Custom Header")
            .FontSize(16)
            .FontColor("#FF0000");
    }
}
```

### Per-Element Changes

Use QuestPDF API directly:

```csharp
container.Text("Custom Text")
    .FontSize(12)
    .Bold()
    .FontColor("#0033AA")
    .AlignCenter();
```

---

## Testing Reports

### Unit Test

```csharp
[Fact]
public void MyReport_GeneratesPdf_WithoutErrors()
{
    // Arrange
    var report = new MyReport();

    // Act
    byte[] pdf = report.GeneratePdf();

    // Assert
    Assert.NotNull(pdf);
    Assert.True(pdf.Length > 0);
    Assert.True(IsPdfValid(pdf));
}

private bool IsPdfValid(byte[] pdf)
{
    // PDF files start with "%PDF"
    return pdf.Length > 4 &&
           pdf[0] == 0x25 &&
           pdf[1] == 0x50 &&
           pdf[2] == 0x44 &&
           pdf[3] == 0x46;
}
```

### Manual Testing

```csharp
// Generate to disk for inspection
var report = new MyReport();
report.SavePdf("test-output.pdf");
// Open test-output.pdf in PDF viewer
```

### Integration Testing

```csharp
[Fact]
public async Task Endpoint_ReturnsPdf_OnSuccess()
{
    // Arrange
    var client = new TestClient();

    // Act
    var response = await client.GetAsync("/api/report");

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

    byte[] pdf = await response.Content.ReadAsByteArrayAsync();
    Assert.True(IsPdfValid(pdf));
}
```

---

## Migration from Legacy System

If migrating from `Demoulas.AccountsReceivable.Reporting`:

### Old Pattern (Accounts Receivable)

```csharp
public class AgingReportExportDocument : IExportableDocument
{
    public IList<Paragraph> Header => _headers;
    public IList<Paragraph> Content => _content;
    public IList<Paragraph> Footer => [_footer];
}

var exporter = new PdfExporter();
byte[] pdf = await exporter.ExportAsPdfAsync(doc);
```

### New Pattern (Profit Sharing)

```csharp
public class AgingReport : BasePdfReport
{
    protected override void ComposeHeader(IContainer header) { }
    protected override void ComposeContent(IContainer content) { }
    protected override void ComposeFooter(IContainer footer) { }
}

var generator = new PdfReportGenerator();
byte[] pdf = await generator.GeneratePdfAsync(new AgingReport());
```

### Migration Steps

1. Create new report class in `Reports/` folder
2. Inherit from `BasePdfReport`
3. Implement `ComposeContent()` with layout logic
4. Replace `Paragraph` and `Table` constructs with `PdfUtilities` methods
5. Update endpoint to use `PdfReportGenerator` instead of `PdfExporter`
6. Remove reference to old reporting module

### Mapping Table

| Legacy                                 | New                                                 |
| -------------------------------------- | --------------------------------------------------- |
| `IExportableDocument`                  | `BasePdfReport`                                     |
| `PdfExporter.ExportAsPdfAsync()`       | `PdfReportGenerator.GeneratePdfAsync()`             |
| `Paragraph` collection                 | QuestPDF `IContainer` fluent API                    |
| `Table` class                          | `ComposeTableHeaderRow()` / `ComposeTableDataRow()` |
| `PdfUtils.CreateDefaultPolicyFooter()` | `ComposeFooter()` override                          |
| `PageSetup`                            | Automatic (managed by base class)                   |

---

## Performance Considerations

### Generation Time

-   Simple reports: ~100ms
-   Medium reports (10+ pages): ~500ms
-   Large reports (100+ pages): 2-5 seconds
-   Very large reports (1000+ pages): 10-30 seconds

### Memory Usage

-   PDF in memory: ~3-5x uncompressed size
-   Example: 10MB data → 30-50MB PDF in memory

### Optimization

1. **Batch Processing**: Generate multiple reports in sequence (avoid memory buildup)

    ```csharp
    foreach (var item in items)
    {
        var report = new MyReport(item);
        await _generator.SavePdfAsync(report, $"{item.Id}.pdf");
        GC.Collect(); // Optional: Force GC between large reports
    }
    ```

2. **Pagination**: For large datasets, split into pages

    ```csharp
    foreach (var page in data.Chunk(1000))
    {
        // Process page...
    }
    ```

3. **Async**: Always use async to avoid blocking thread pool
    ```csharp
    byte[] pdf = await _generator.GeneratePdfAsync(report, ct);
    ```

---

## Troubleshooting

| Issue             | Cause                       | Solution                                                              |
| ----------------- | --------------------------- | --------------------------------------------------------------------- |
| Logo not showing  | Embedded resource not found | Verify `Resources/mb_mfyd.png` exists and is set as Embedded Resource |
| Text overlaps     | Content too large           | Use `.TruncateWithEllipsis()` or reduce font size                     |
| Wrong colors      | Configuration override      | Check `PdfReportConfiguration.BrandColors`                            |
| Large file size   | Complex content             | Simplify layout or reduce image quality                               |
| Slow generation   | Large dataset               | Split into pages or optimize loops                                    |
| Page breaks wrong | Layout issues               | Add explicit `.PageBreak()` elements                                  |

---

## Security Notes

1. **No Hardcoded Secrets**: Never embed API keys in reports
2. **PII Handling**: Mask sensitive data before passing to report
3. **Access Control**: Validate user permissions before generating
4. **Audit Logging**: Log which reports were generated by whom
5. **File Permissions**: If saving to disk, secure file permissions

---

## QuestPDF Resources

-   **Getting Started**: https://www.questpdf.com/getting-started.html
-   **API Documentation**: https://www.questpdf.com/api-documentation
-   **Community**: https://github.com/QuestPDF
-   **License**: Community (free) or Commercial

---

## Summary

The Profit Sharing Reporting module provides:

-   ✅ Clean, extensible base class for reports
-   ✅ Centralized styling and configuration
-   ✅ Reusable UI components (tables, headers, footers)
-   ✅ Async PDF generation
-   ✅ Embedded logo (no file dependencies)
-   ✅ Professional, client-preferred formatting

For new reports, copy the template and implement `ComposeContent()`.
