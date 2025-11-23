using Demoulas.ProfitSharing.Reporting.Core;
using QuestPDF.Fluent;

namespace Demoulas.ProfitSharing.Reporting.Examples;

/// <summary>
/// Example profit sharing report demonstrating how to use the BasePdfReport wrapper.
/// This shows the recommended pattern for creating new reports.
/// 
/// Delete or repurpose this file once you have created your first real report.
/// </summary>
public class SampleProfitSharingReport : BasePdfReport
{
    private readonly string _fiscalYear;
    private readonly decimal _totalEarnings;
    private readonly int _participantCount;
    private readonly List<ParticipantSample> _participants;

    public override string Title => $"Profit Sharing Report - {_fiscalYear}";
    public override string ReportName => "profit-sharing-statement";
    public override string GeneratedBy => "Profit Sharing System";

    public SampleProfitSharingReport(
        string fiscalYear,
        decimal totalEarnings,
        int participantCount,
        List<ParticipantSample>? participants = null)
    {
        _fiscalYear = fiscalYear;
        _totalEarnings = totalEarnings;
        _participantCount = participantCount;
        _participants = participants ?? [];
    }

    /// <summary>
    /// Composes the header with company branding
    /// </summary>
    protected override void ComposeHeader(IContainer header)
    {
        header.ComposeStandardHeader("Profit Sharing Statement", showLogo: true);
    }

    /// <summary>
    /// Composes the main report content
    /// </summary>
    protected override void ComposeContent(IContainer content)
    {
        content.Column(column =>
        {
            // Report metadata section
            column.Item().Element(ComposeMetadataSection);

            // Section break
            column.Item().ComposeSectionBreak();

            // Summary section
            column.Item().Element(ComposeSummarySection);

            // Section break
            column.Item().ComposeSectionBreak();

            // Participants table (if data available)
            if (_participants.Count > 0)
            {
                column.Item().Element(ComposeParticipantsTable);
                column.Item().ComposeSectionBreak();
            }

            // Footer notes
            column.Item().Element(ComposeFooterNotes);
        });
    }

    /// <summary>
    /// Composes the report metadata section (generation info)
    /// </summary>
    private void ComposeMetadataSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Report Information");
            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.SmallGap).Element(c =>
            {
                c.Column(col =>
                {
                    col.Item().ComposeKeyValuePair("Fiscal Year", _fiscalYear);
                    col.Item().ComposeKeyValuePair("Generated Date", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                    col.Item().ComposeKeyValuePair("Report Type", "Annual Profit Sharing Statement");
                });
            });
        });
    }

    /// <summary>
    /// Composes the summary statistics section
    /// </summary>
    private void ComposeSummarySection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Summary");
            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.SmallGap).Element(c =>
            {
                c.Column(col =>
                {
                    col.Item().ComposeKeyValuePair("Total Earnings", _totalEarnings.ToCurrencyString(), bold: true);
                    col.Item().ComposeKeyValuePair("Number of Participants", _participantCount.ToString(), bold: true);

                    if (_participantCount > 0)
                    {
                        decimal avgPerParticipant = _totalEarnings / _participantCount;
                        col.Item().ComposeKeyValuePair("Average Per Participant", avgPerParticipant.ToCurrencyString());
                    }
                });
            });
        });
    }

    /// <summary>
    /// Composes the participants table
    /// </summary>
    private void ComposeParticipantsTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Participant Details");

            column.Item().PaddingTop(PdfReportConfiguration.Spacing.StandardGap).Element(c =>
            {
                c.Column(col =>
                {
                    // Table header
                    col.Item().ComposeTableHeaderRow("Name", "Badge #", "Profit Share", "Status");

                    // Table data rows
                    bool isAlternate = false;
                    decimal total = 0;

                    foreach (var participant in _participants)
                    {
                        col.Item().ComposeTableDataRow(
                            isAlternate,
                            participant.Name.TruncateWithEllipsis(25),
                            participant.BadgeNumber,
                            participant.ProfitShare.ToCurrencyString(),
                            participant.Status
                        );

                        total += participant.ProfitShare;
                        isAlternate = !isAlternate;
                    }

                    // Totals row
                    col.Item().ComposeTotalsRow("Totals", total.ToCurrencyString());
                });
            });
        });
    }

    /// <summary>
    /// Composes footer notes/disclaimers
    /// </summary>
    private void ComposeFooterNotes(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Notes");
            column.Item().PaddingVertical(PdfReportConfiguration.Spacing.SmallGap)
                .Text("This report contains confidential profit sharing information for Demoulas Supermarkets Inc. employees only. " +
                      "All amounts are subject to applicable taxes and deductions. For questions regarding your profit sharing statement, " +
                      "please contact the Human Resources Department.")
                .FontSize(PdfReportConfiguration.FontSizes.FooterSize)
                .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);
        });
    }

    /// <summary>
    /// Sample participant data structure
    /// </summary>
    public class ParticipantSample
    {
        public string Name { get; set; } = string.Empty;
        public string BadgeNumber { get; set; } = string.Empty;
        public decimal ProfitShare { get; set; }
        public string Status { get; set; } = "Active";
    }
}
