using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Reporting.Core;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Demoulas.ProfitSharing.Reporting.Reports;

/// <summary>
/// Account History PDF Report for members or legal department.
/// Displays member profile, account activity by profit year, and cumulative totals.
/// </summary>
public class AccountHistoryPdfReport : BasePdfReport
{
    private readonly MemberProfileInfo _memberProfile;
    private readonly List<AccountHistoryReportResponse> _accountHistory;
    private readonly AccountHistoryReportTotals _cumulativeTotals;
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;
    private readonly string _preparedBy;

    public override string Title => $"Account History Report - {_memberProfile.FullName}";
    public override string ReportName => "account-history-statement";
    public override string GeneratedBy => "Profit Sharing System";
    public override bool IncludePageNumbers => true;
    public override bool IncludeCompanyFooter => true;
    public override bool IncludeCoverPage => false;

    /// <summary>
    /// Account history PDF report constructor
    /// </summary>
    public AccountHistoryPdfReport(
        MemberProfileInfo memberProfile,
        List<AccountHistoryReportResponse> accountHistory,
        AccountHistoryReportTotals cumulativeTotals,
        DateOnly startDate,
        DateOnly endDate,
        string preparedBy = "Member")
    {
        _memberProfile = memberProfile ?? throw new ArgumentNullException(nameof(memberProfile));
        _accountHistory = accountHistory ?? new List<AccountHistoryReportResponse>();
        _cumulativeTotals = cumulativeTotals ?? new AccountHistoryReportTotals();
        _startDate = startDate;
        _endDate = endDate;
        _preparedBy = preparedBy;
    }

    /// <summary>
    /// Composes the header with company branding and report title
    /// </summary>
    protected override void ComposeHeader(IContainer header)
    {
        header.ComposeStandardHeader("Account History Report", showLogo: true);
    }

    /// <summary>
    /// Composes the main report content - clean, minimal styling like Monthly Statement
    /// </summary>
    protected override void ComposeContent(IContainer content)
    {
        content.Column(column =>
        {
            // Member Profile Section
            column.Item().Element(ComposeMemberProfileSection);

            // Report Parameters Section
            column.Item().PaddingTop(10).Element(ComposeReportParametersSection);

            // Account History Table
            column.Item().PaddingTop(10).Element(ComposeAccountHistoryTable);

            // Cumulative Totals Section
            column.Item().PaddingTop(10).Element(ComposeCumulativeTotalsSection);

            // Legal Notice
            column.Item().PaddingTop(15).Element(ComposeLegalNoticeSection);
        });
    }

    /// <summary>
    /// Composes the member profile section - clean style without heavy backgrounds
    /// </summary>
    private void ComposeMemberProfileSection(IContainer container)
    {
        container.Column(column =>
        {
            // Section title - simple bold text with underline
            column.Item().Text("Member Profile").FontSize(11).Bold();
            column.Item().PaddingTop(2).Height(1).Background("#000000");

            column.Item().PaddingTop(5).Row(row =>
            {
                // Left column
                row.RelativeItem().Column(leftColumn =>
                {
                    ComposeSimpleRow(leftColumn, "Full Name:", _memberProfile.FullName);
                    ComposeSimpleRow(leftColumn, "Badge Number:", _memberProfile.BadgeNumber.ToString());
                    ComposeSimpleRow(leftColumn, "SSN:", _memberProfile.MaskedSsn);
                    ComposeSimpleRow(leftColumn, "Date of Birth:", _memberProfile.DateOfBirth?.ToString("MM/dd/yyyy") ?? "N/A");
                    ComposeSimpleRow(leftColumn, "Address:", _memberProfile.Address ?? "N/A");
                    ComposeSimpleRow(leftColumn, "City, State ZIP:", $"{_memberProfile.City}, {_memberProfile.State} {_memberProfile.ZipCode}".TrimEnd());
                    ComposeSimpleRow(leftColumn, "Phone:", _memberProfile.Phone ?? "N/A");
                });

                // Right column
                row.RelativeItem().Column(rightColumn =>
                {
                    ComposeSimpleRow(rightColumn, "Hire Date:", _memberProfile.HireDate?.ToString("MM/dd/yyyy") ?? "N/A");
                    ComposeSimpleRow(rightColumn, "Termination Date:", _memberProfile.TerminationDate?.ToString("MM/dd/yyyy") ?? "N/A");
                    ComposeSimpleRow(rightColumn, "Employment Status:", _memberProfile.EmploymentStatus ?? "N/A");
                    ComposeSimpleRow(rightColumn, "Store Number:", _memberProfile.StoreNumber?.ToString() ?? "N/A");
                });
            });
        });
    }

    /// <summary>
    /// Composes the report parameters section - clean style
    /// </summary>
    private void ComposeReportParametersSection(IContainer container)
    {
        container.Column(column =>
        {
            // Section title - simple bold text with underline
            column.Item().Text("Report Parameters").FontSize(11).Bold();
            column.Item().PaddingTop(2).Height(1).Background("#000000");

            column.Item().PaddingTop(5).Row(row =>
            {
                // Left column
                row.RelativeItem().Column(leftColumn =>
                {
                    ComposeSimpleRow(leftColumn, "Report Period Start:", _startDate.ToString("MM/dd/yyyy"));
                    ComposeSimpleRow(leftColumn, "Report Period End:", _endDate.ToString("MM/dd/yyyy"));
                });

                // Right column
                row.RelativeItem().Column(rightColumn =>
                {
                    ComposeSimpleRow(rightColumn, "Prepared By:", _preparedBy);
                    ComposeSimpleRow(rightColumn, "Generated Date:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                });
            });
        });
    }

    /// <summary>
    /// Composes the account history table - clean table styling
    /// </summary>
    private void ComposeAccountHistoryTable(IContainer container)
    {
        container.Column(column =>
        {
            // Section title
            column.Item().Text("Account Activity by Profit Year").FontSize(11).Bold();
            column.Item().PaddingTop(2).Height(1).Background("#000000");

            if (_accountHistory.Count == 0)
            {
                column.Item().PaddingTop(5).Text("No account activity found for the specified period.")
                    .FontSize(10).Italic();
                return;
            }

            // Table with header and data rows
            column.Item().PaddingTop(5).Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(0.8f);  // Year
                    columns.RelativeColumn(1.3f);  // Contributions
                    columns.RelativeColumn(1.3f);  // Earnings
                    columns.RelativeColumn(1.3f);  // Forfeitures
                    columns.RelativeColumn(1.3f);  // Withdrawals
                    columns.RelativeColumn(1.3f);  // Vested Balance
                    columns.RelativeColumn(0.8f);  // Vesting %
                    columns.RelativeColumn(0.7f);  // Years
                    columns.RelativeColumn(1.3f);  // Ending Balance
                });

                // Header row with bottom border
                table.Header(header =>
                {
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3)
                        .Text("Year").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Contributions").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Earnings").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Forfeitures").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Withdrawals").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Vested Balance").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Vesting %").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Years").FontSize(10).Bold();
                    header.Cell().BorderBottom(1).BorderColor("#000000").Padding(3).AlignRight()
                        .Text("Ending Balance").FontSize(10).Bold();
                });

                // Data rows
                foreach (var row in _accountHistory)
                {
                    table.Cell().Padding(3).Text(row.ProfitYear.ToString()).FontSize(10);
                    table.Cell().Padding(3).AlignRight().Text(row.Contributions.ToCurrencyString()).FontSize(10);
                    table.Cell().Padding(3).AlignRight().Text(row.Earnings.ToCurrencyString()).FontSize(10);
                    table.Cell().Padding(3).AlignRight().Text(row.Forfeitures.ToCurrencyString()).FontSize(10);
                    table.Cell().Padding(3).AlignRight().Text(row.Withdrawals.ToCurrencyString()).FontSize(10);
                    table.Cell().Padding(3).AlignRight().Text(row.VestedBalance.ToCurrencyString()).FontSize(10);

                    var vestingPercentText = row.VestingPercent.HasValue
                        ? $"{(row.VestingPercent.Value * 100):0}%"
                        : "N/A";
                    table.Cell().Padding(3).AlignRight().Text(vestingPercentText).FontSize(10);

                    var yearsText = row.YearsInPlan.HasValue
                        ? row.YearsInPlan.Value.ToString()
                        : "N/A";
                    table.Cell().Padding(3).AlignRight().Text(yearsText).FontSize(10);

                    table.Cell().Padding(3).AlignRight().Text(row.EndingBalance.ToCurrencyString()).FontSize(10).Bold();
                }
            });
        });
    }

    /// <summary>
    /// Composes the cumulative totals section - clean styling
    /// </summary>
    private void ComposeCumulativeTotalsSection(IContainer container)
    {
        container.Column(column =>
        {
            // Section title
            column.Item().Text("Cumulative Totals").FontSize(11).Bold();
            column.Item().PaddingTop(2).Height(1).Background("#000000");

            column.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                });

                // Total rows
                table.Cell().Padding(2).Text("Total Contributions").FontSize(10);
                table.Cell().Padding(2).AlignRight().Text(_cumulativeTotals.TotalContributions.ToCurrencyString()).FontSize(10);

                table.Cell().Padding(2).Text("Total Earnings").FontSize(10);
                table.Cell().Padding(2).AlignRight().Text(_cumulativeTotals.TotalEarnings.ToCurrencyString()).FontSize(10);

                table.Cell().Padding(2).Text("Total Forfeitures").FontSize(10);
                table.Cell().Padding(2).AlignRight().Text(_cumulativeTotals.TotalForfeitures.ToCurrencyString()).FontSize(10);

                table.Cell().Padding(2).Text("Total Withdrawals").FontSize(10);
                table.Cell().Padding(2).AlignRight().Text(_cumulativeTotals.TotalWithdrawals.ToCurrencyString()).FontSize(10);
            });

            // Total Vested Balance - highlighted
            column.Item().PaddingTop(5).Height(1).Background("#000000");
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("Total Vested Balance").FontSize(11).Bold();
                row.RelativeItem().AlignRight()
                    .Text(_cumulativeTotals.TotalVestedBalance.ToCurrencyString())
                    .FontSize(12).Bold().FontColor(PdfReportConfiguration.BrandColors.DemoulasBlue);
            });
        });
    }

    /// <summary>
    /// Composes the legal notice section - boxed with border
    /// </summary>
    private static void ComposeLegalNoticeSection(IContainer container)
    {
        container.Border(1).BorderColor("#000000").Padding(8).Column(column =>
        {
            column.Item().Text("Legal Notice").FontSize(10).Bold();
            column.Item().PaddingTop(3).Text(
                "This Account History Report is confidential and contains sensitive personal and financial information. " +
                "It is intended solely for the member listed above or authorized representatives including the Company's legal department. " +
                "Unauthorized disclosure, copying, or distribution of this report is strictly prohibited. " +
                "This statement is provided as of the date generated and may not reflect subsequent transactions or changes. " +
                "For questions regarding this report, please contact the Profit Sharing Administration office.")
                .FontSize(8).LineHeight(1.3f);
        });
    }

    /// <summary>
    /// Helper to compose a simple label:value row
    /// </summary>
    private static void ComposeSimpleRow(ColumnDescriptor column, string label, string value)
    {
        column.Item().PaddingVertical(1).Row(row =>
        {
            row.ConstantItem(100).Text(label).FontSize(10).FontColor("#666666");
            row.RelativeItem().Text(value).FontSize(10).Bold();
        });
    }

    /// <summary>
    /// Member profile data transfer object
    /// </summary>
    public sealed class MemberProfileInfo
    {
        public string FullName { get; set; } = string.Empty;
        public int BadgeNumber { get; set; }
        public string MaskedSsn { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public DateOnly? HireDate { get; set; }
        public DateOnly? TerminationDate { get; set; }
        public string? EmploymentStatus { get; set; }
        public int? StoreNumber { get; set; }
    }
}
