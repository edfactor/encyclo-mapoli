using Demoulas.ProfitSharing.Reporting.Core;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
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
    /// Composes the main report content
    /// </summary>
    protected override void ComposeContent(IContainer content)
    {
        content.Column(column =>
        {
            // Member Profile Section
            column.Item().Element(ComposeMemberProfileSection);

            // Vertical space before Report Parameters
            column.Item().PaddingBottom(0.25f);

            // Report Parameters Section
            column.Item().Element(ComposeReportParametersSection);

            // Vertical space before Account Activity Table - EXPLICIT SPACING
            column.Item().PaddingBottom(0.30f);

            // Account History Table
            column.Item().Element(ComposeAccountHistoryTable);

            // Vertical space before Cumulative Totals
            column.Item().PaddingBottom(0.25f);

            // Cumulative Totals Section
            column.Item().Element(ComposeCumulativeTotalsSection);

            // Vertical space before Legal Notice
            column.Item().PaddingBottom(0.15f);

            // Legal Notice
            column.Item().Element(ComposeLegalNoticeSection);
        });
    }

    /// <summary>
    /// Composes the member profile section with contact information (two-column layout)
    /// </summary>
    private void ComposeMemberProfileSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Member Profile", "#F0F0F0");

            column.Item()
                .Padding(PdfReportConfiguration.Spacing.StandardGap)
                .Row(row =>
                {
                    // Left column
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Spacing(PdfReportConfiguration.TableDefaults.RowSpacing);
                        leftColumn.Item().ComposeKeyValuePair("Full Name", _memberProfile.FullName, bold: true);
                        leftColumn.Item().ComposeKeyValuePair("Badge Number", _memberProfile.BadgeNumber.ToString());
                        leftColumn.Item().ComposeKeyValuePair("SSN", _memberProfile.MaskedSsn);
                        leftColumn.Item().ComposeKeyValuePair("Date of Birth", _memberProfile.DateOfBirth?.ToString("MM/dd/yyyy") ?? "N/A");
                        leftColumn.Item().ComposeDivider(0.5f);
                        leftColumn.Item().ComposeKeyValuePair("Address", _memberProfile.Address ?? "N/A");
                        leftColumn.Item().ComposeKeyValuePair("City, State ZIP", $"{_memberProfile.City}, {_memberProfile.State} {_memberProfile.ZipCode}".TrimEnd());
                        leftColumn.Item().ComposeKeyValuePair("Phone", _memberProfile.Phone ?? "N/A");
                    });

                    // Right column
                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Spacing(PdfReportConfiguration.TableDefaults.RowSpacing);
                        rightColumn.Item().ComposeKeyValuePair("Hire Date", _memberProfile.HireDate?.ToString("MM/dd/yyyy") ?? "N/A");
                        rightColumn.Item().ComposeKeyValuePair("Termination Date", _memberProfile.TerminationDate?.ToString("MM/dd/yyyy") ?? "Current Employee");
                        rightColumn.Item().ComposeKeyValuePair("Employment Status", _memberProfile.EmploymentStatus ?? "N/A");
                        rightColumn.Item().ComposeKeyValuePair("Store Number", _memberProfile.StoreNumber?.ToString() ?? "N/A");
                    });
                });
        });
    }

    /// <summary>
    /// Composes the report parameters section (two-column layout)
    /// </summary>
    private void ComposeReportParametersSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Report Parameters", "#F0F0F0");

            column.Item()
                .Padding(PdfReportConfiguration.Spacing.StandardGap)
                .Row(row =>
                {
                    // Left column
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Spacing(PdfReportConfiguration.TableDefaults.RowSpacing);
                        leftColumn.Item().ComposeKeyValuePair("Report Period Start", _startDate.ToString("MM/dd/yyyy"));
                        leftColumn.Item().ComposeKeyValuePair("Report Period End", _endDate.ToString("MM/dd/yyyy"));
                    });

                    // Right column
                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Spacing(PdfReportConfiguration.TableDefaults.RowSpacing);
                        rightColumn.Item().ComposeKeyValuePair("Prepared By", _preparedBy);
                        rightColumn.Item().ComposeKeyValuePair("Generated Date", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    });
                });
        });
    }

    /// <summary>
    /// Composes the account history table with all transaction details
    /// </summary>
    private void ComposeAccountHistoryTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Account Activity by Profit Year", "#F0F0F0");

            if (_accountHistory.Count == 0)
            {
                column.Item()
                    .Padding(PdfReportConfiguration.Spacing.StandardGap)
                    .Text("No account activity found for the specified period.")
                    .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                    .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);
                return;
            }

            column.Item()
                .Padding(PdfReportConfiguration.Spacing.StandardGap)
                .Column(innerColumn =>
                {
                    // Table Header
                    innerColumn.Item().ComposeTableHeaderRow(
                        "Year",
                        "Contributions",
                        "Earnings",
                        "Forfeitures",
                        "Withdrawals",
                        "Ending Balance");

                    // Table Body - alternating row colors
                    var isAlternate = false;
                    foreach (var row in _accountHistory)
                    {
                        innerColumn.Item().ComposeTableDataRow(
                            isAlternate,
                            row.ProfitYear.ToString(),
                            row.Contributions.ToCurrencyString(),
                            row.Earnings.ToCurrencyString(),
                            row.Forfeitures.ToCurrencyString(),
                            row.Withdrawals.ToCurrencyString(),
                            row.EndingBalance.ToCurrencyString());

                        isAlternate = !isAlternate;
                    }

                    innerColumn.Item().ComposeDivider(1);
                });
        });
    }

    /// <summary>
    /// Composes the cumulative totals section
    /// </summary>
    private void ComposeCumulativeTotalsSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Cumulative Totals", "#F0F0F0");

            column.Item()
                .Padding(PdfReportConfiguration.Spacing.StandardGap)
                .Column(innerColumn =>
                {
                    // Add proper spacing between items
                    innerColumn.Spacing(PdfReportConfiguration.TableDefaults.RowSpacing);

                    innerColumn.Item().ComposeTotalsRow("Total Contributions", _cumulativeTotals.TotalContributions.ToCurrencyString());
                    innerColumn.Item().ComposeTotalsRow("Total Earnings", _cumulativeTotals.TotalEarnings.ToCurrencyString());
                    innerColumn.Item().ComposeTotalsRow("Total Forfeitures", _cumulativeTotals.TotalForfeitures.ToCurrencyString());
                    innerColumn.Item().ComposeTotalsRow("Total Withdrawals", _cumulativeTotals.TotalWithdrawals.ToCurrencyString());

                    innerColumn.Item().ComposeDivider(1);

                    innerColumn.Item().Element(el =>
                    {
                        el.Padding(5)
                            .Background(PdfReportConfiguration.BrandColors.TotalsGray)
                            .Column(col =>
                            {
                                col.Item().Text("Total Vested Balance")
                                    .FontSize(PdfReportConfiguration.FontSizes.LabelSize)
                                    .FontColor(PdfReportConfiguration.BrandColors.TextBlack)
                                    .Bold();

                                col.Item().Text(_cumulativeTotals.TotalVestedBalance.ToCurrencyString())
                                    .FontSize(PdfReportConfiguration.FontSizes.TotalsSize)
                                    .FontColor(PdfReportConfiguration.BrandColors.DemoulasBlue)
                                    .Bold();
                            });
                    });
                });
        });
    }

    /// <summary>
    /// Composes the legal notice section
    /// </summary>
    private static void ComposeLegalNoticeSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item()
                .Padding(5)
                .Background("#EEEEEE")
                .BorderColor(PdfReportConfiguration.BrandColors.BorderGray)
                .Border(1)
                .Column(innerColumn =>
                {
                    innerColumn.Item().Text("Legal Notice")
                        .FontSize(PdfReportConfiguration.FontSizes.LabelSize)
                        .FontColor(PdfReportConfiguration.BrandColors.TextBlack)
                        .Bold();

                    innerColumn.Item().Text(
                        "This Account History Report is confidential and contains sensitive personal and financial information. " +
                        "It is intended solely for the member listed above or authorized representatives including the Company's legal department. " +
                        "Unauthorized disclosure, copying, or distribution of this report is strictly prohibited. " +
                        "This statement is provided as of the date generated and may not reflect subsequent transactions or changes. " +
                        "For questions regarding this report, please contact the Profit Sharing Administration office.")
                        .FontSize(PdfReportConfiguration.FontSizes.FooterSize)
                        .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray)
                        .LineHeight(1.2f);
                });
        });
    }

    /// <summary>
    /// Member profile data transfer object
    /// </summary>
    public class MemberProfileInfo
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
