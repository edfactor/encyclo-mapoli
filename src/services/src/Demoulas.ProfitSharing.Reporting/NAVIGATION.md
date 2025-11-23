# 📚 Reporting Module - Documentation Navigation

Quick links to find what you need.

## 🚀 Getting Started (5-30 minutes)

Start here if you're new to the reporting module:

1. **README.md** (15 min read)

    - What is this module?
    - Quick start guide
    - Overview of all components
    - Basic examples

2. **QUICK_REFERENCE.md** (10 min read)

    - 5-minute setup
    - Copy-paste examples for common elements
    - Fast troubleshooting

3. **ReportTemplate.cs** or **SampleProfitSharingReport.cs**
    - Copy one of these to create your first report
    - Inline comments explain each section
    - Ready to run

## 📖 Comprehensive Documentation

For in-depth information:

-   **ARCHITECTURE.md** (30 min read)

    -   Component architecture and design patterns
    -   How everything fits together
    -   Advanced usage patterns
    -   Migration from legacy system
    -   Performance tuning

-   **BEFORE_AND_AFTER.md** (15 min read)
    -   Comparison with legacy Accounts Receivable system
    -   Code size reductions
    -   Why the new approach is better
    -   Benefits summary

## 🔧 Core Components Reference

Technical reference for each component:

### Core Classes

-   **BasePdfReport.cs**

    -   Base class for all reports
    -   Methods: `GeneratePdf()`, `SavePdf()`
    -   Override: `ComposeHeader()`, `ComposeContent()`, `ComposeFooter()`
    -   See: README.md → "BasePdfReport"

-   **PdfReportConfiguration.cs**

    -   Centralized styling configuration
    -   Brand colors, fonts, spacing, margins
    -   Logo loading
    -   See: README.md → "PdfReportConfiguration"

-   **PdfUtilities.cs**

    -   15+ helper extension methods
    -   Common elements: headers, tables, dividers
    -   Number formatting
    -   See: QUICK_REFERENCE.md → "Common Elements"

-   **PdfReportGenerator.cs**
    -   Async PDF generation service
    -   Methods: `GeneratePdfAsync()`, `SavePdfAsync()`, `WritePdfToStreamAsync()`
    -   See: README.md → "PdfReportGenerator"

### Registration

-   **ReportingServiceCollectionExtensions.cs**
    -   DI setup: `services.AddProfitSharingReporting()`
    -   See: README.md → "Getting Started"

## 📝 Examples & Templates

-   **SampleProfitSharingReport.cs** (Examples folder)

    -   Complete, working example
    -   Demonstrates all major features
    -   Run immediately without changes

-   **ReportTemplate.cs** (Reports folder)
    -   Copy-paste template for new reports
    -   Inline instructions
    -   Best practices included
    -   Use as starting point for your reports

## 🎯 Common Tasks

### "How do I create a new report?"

1. Read: QUICK_REFERENCE.md → "TL;DR - 5 Minute Setup"
2. Copy: `Reports/ReportTemplate.cs`
3. Customize the three methods:
    - `Title` property
    - `ComposeHeader()` method
    - `ComposeContent()` method (required)
4. Inject: `PdfReportGenerator` in your endpoint
5. Generate: `await _generator.GeneratePdfAsync(report, ct)`

### "What components are available?"

Read: QUICK_REFERENCE.md → "Common Elements"

Lists all 10+ available utility methods with examples:

-   `ComposeStandardHeader()`
-   `ComposeSectionHeader()`
-   `ComposeKeyValuePair()`
-   `ComposeTableHeaderRow()` / `ComposeTableDataRow()` / `ComposeTotalsRow()`
-   `ComposeDivider()` / `ComposeSectionBreak()`
-   Number formatting: `ToCurrencyString()`, `ToCurrencyStringOrEmpty()`
-   Text formatting: `TruncateWithEllipsis()`

### "How do I customize styling?"

Read: ARCHITECTURE.md → "Styling & Customization"

Three levels:

1. **Global** - Edit `PdfReportConfiguration.cs` (all reports affected)
2. **Per-Report** - Override `ComposeHeader()` etc in your report class
3. **Per-Element** - Use QuestPDF API directly in `ComposeContent()`

### "How do I generate a PDF to disk?"

Read: QUICK_REFERENCE.md → "Testing Your Report"

```csharp
await _pdfGenerator.SavePdfAsync(report, "/path/to/file.pdf", ct);
```

### "How do I return PDF from an endpoint?"

Read: README.md → "Endpoint Results Pattern"

```csharp
byte[] pdf = await _pdfGenerator.GeneratePdfAsync(report, ct);
return Results.File(pdf, "application/pdf", "report.pdf");
```

### "What colors/fonts are available?"

Read: QUICK_REFERENCE.md → "Standard Styling"

Summary table of all available:

-   Colors (DemoulasBlue, HeaderGray, TotalsGray, etc.)
-   Font sizes (TitleSize, HeaderSize, ContentSize, FooterSize, etc.)
-   Spacing measurements (SmallGap, StandardGap, SectionBreak, etc.)

### "The logo isn't showing. What do I do?"

Read: QUICK_REFERENCE.md → "Troubleshooting"

Solution: Verify `mb_mfyd.png` is in `Resources/` folder

### "How fast is PDF generation?"

Read: ARCHITECTURE.md → "Performance Considerations"

-   Simple reports: ~100ms
-   Medium reports (10 pages): ~500ms
-   Large reports (100+ pages): 2-5 seconds

### "How do I migrate from the old system?"

Read: ARCHITECTURE.md → "Migration from Legacy System"

Mapping table of old → new patterns

## 📊 File Organization

```
Demoulas.ProfitSharing.Reporting/
│
├── Core/                           (Core wrapper components)
│   ├── BasePdfReport.cs           • Base class for all reports
│   ├── PdfReportConfiguration.cs  • Styling configuration
│   ├── PdfUtilities.cs            • Helper extension methods
│   └── PdfReportGenerator.cs      • Async generation service
│
├── Examples/
│   └── SampleProfitSharingReport.cs  • Complete working example
│
├── Reports/
│   └── ReportTemplate.cs          • Copy-paste template
│
├── Extensions/
│   └── ReportingServiceCollectionExtensions.cs  • DI setup
│
├── Resources/
│   └── mb_mfyd.png                • Embedded logo
│
├── Documentation/
│   ├── README.md                  • Main documentation (450+ lines)
│   ├── QUICK_REFERENCE.md         • Developer cheat sheet (200+ lines)
│   ├── ARCHITECTURE.md            • Architecture guide (500+ lines)
│   ├── BEFORE_AND_AFTER.md        • Comparison with legacy (300+ lines)
│   ├── IMPLEMENTATION_SUMMARY.md  • What was created (400+ lines)
│   ├── IMPLEMENTATION_CHECKLIST.md • Verification guide
│   └── NAVIGATION.md              • This file!
│
└── Demoulas.ProfitSharing.Reporting.csproj  • Project file
```

## 🔍 Search Guide

Looking for something specific? Try these keywords:

**"How to" Topics:**

-   Creating reports → QUICK_REFERENCE.md
-   Styling → ARCHITECTURE.md → "Styling & Customization"
-   Tables → QUICK_REFERENCE.md → "Common Elements"
-   Testing → README.md → "Testing"
-   Performance → ARCHITECTURE.md → "Performance Considerations"
-   Migration → ARCHITECTURE.md → "Migration from Legacy System"
-   Troubleshooting → QUICK_REFERENCE.md → "Troubleshooting"

**Component References:**

-   BasePdfReport → README.md → "BasePdfReport"
-   PdfReportConfiguration → README.md → "PdfReportConfiguration"
-   PdfUtilities → README.md → "Utilities Reference"
-   PdfReportGenerator → README.md → "PdfReportGenerator"

**Examples:**

-   Sample report → Examples/SampleProfitSharingReport.cs
-   Template → Reports/ReportTemplate.cs
-   Endpoint integration → QUICK_REFERENCE.md → "Use in Your Endpoint"

**Comparison:**

-   Legacy vs new → BEFORE_AND_AFTER.md
-   Benefits summary → IMPLEMENTATION_SUMMARY.md

## ⏱️ Reading Time Estimates

-   **QUICK_REFERENCE.md**: 10 min (start here!)
-   **README.md**: 20 min (good overview)
-   **ARCHITECTURE.md**: 30 min (deep dive)
-   **BEFORE_AND_AFTER.md**: 15 min (optional)
-   **SampleProfitSharingReport.cs**: 10 min (read example code)

**Total to become proficient: 60-90 minutes**

## 🎓 Learning Path

### Beginner (just want to create a report)

1. QUICK_REFERENCE.md (10 min)
2. Copy ReportTemplate.cs (5 min)
3. Implement `ComposeContent()` (20 min)
4. Done!

### Intermediate (want to understand the system)

1. README.md (20 min)
2. SampleProfitSharingReport.cs (10 min)
3. QUICK_REFERENCE.md (10 min)
4. Try creating 2-3 reports

### Advanced (want to extend and customize)

1. ARCHITECTURE.md (30 min)
2. Read all Core/\*.cs files (30 min)
3. BEFORE_AND_AFTER.md (15 min)
4. Review PdfUtilities.cs in depth (20 min)

## 🚦 Quick Decision Tree

```
Need to...?

├─ Create a new report?
│  └─ QUICK_REFERENCE.md (TL;DR section)
│
├─ Add a custom component?
│  └─ ARCHITECTURE.md (Advanced Usage)
│
├─ Change styling globally?
│  └─ QUICK_REFERENCE.md (Standard Styling)
│
├─ Return PDF from endpoint?
│  └─ README.md (Usage section)
│
├─ Test a report?
│  └─ README.md (Testing section)
│
├─ Migrate old code?
│  └─ ARCHITECTURE.md (Migration section)
│
├─ Understand the design?
│  └─ ARCHITECTURE.md (Architecture Overview)
│
└─ See what's different from legacy?
   └─ BEFORE_AND_AFTER.md
```

## 📞 Still Have Questions?

1. **Search the docs** - Most answers are in the 1000+ lines of documentation
2. **Check examples** - SampleProfitSharingReport.cs shows most patterns
3. **Try the template** - ReportTemplate.cs has inline comments
4. **Review QuestPDF docs** - https://www.questpdf.com for fluent API questions

## ✅ Verification Checklist

Before using in production:

-   [ ] Read QUICK_REFERENCE.md
-   [ ] Run SampleProfitSharingReport.cs
-   [ ] Create a test report from ReportTemplate.cs
-   [ ] Generate PDF and verify it opens
-   [ ] Check logo appears in PDF
-   [ ] Verify colors and fonts match client preference
-   [ ] Test in endpoint with data
-   [ ] Add telemetry to endpoint (see TELEMETRY_GUIDE.md)

## 🎉 You're Ready!

Pick up QUICK_REFERENCE.md and start creating reports. You've got this!

**Happy reporting!** 📊
