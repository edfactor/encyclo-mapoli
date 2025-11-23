using Demoulas.ProfitSharing.Reporting.Core;
using Demoulas.Common.Contracts.Contracts.Response;
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
    private readonly string _preparedFor;

    public override string Title => $"Account History Report - {_memberProfile.FullName}";
    public override string ReportName => "account-history-statement";
    public override string GeneratedBy => "Profit Sharing System";
    public override bool IncludePageNumbers => true;
    public override bool IncludeCompanyFooter => true;

    /// <summary>
    /// Account history PDF report constructor
    /// </summary>
    public AccountHistoryPdfReport(
        MemberProfileInfo memberProfile,
        List<AccountHistoryReportResponse> accountHistory,
        AccountHistoryReportTotals cumulativeTotals,
        DateOnly startDate,
        DateOnly endDate,
        string preparedFor = "Member")
    {
        _memberProfile = memberProfile ?? throw new ArgumentNullException(nameof(memberProfile));
        _accountHistory = accountHistory ?? new List<AccountHistoryReportResponse>();
        _cumulativeTotals = cumulativeTotals ?? new AccountHistoryReportTotals();
        _startDate = startDate;
        _endDate = endDate;
        _preparedFor = preparedFor;
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

            // Section break
            column.Item().ComposeSectionBreak();

            // Report Parameters Section
            column.Item().Element(ComposeReportParametersSection);

            // Section break
            column.Item().ComposeSectionBreak();

            // Account History Table
            column.Item().Element(ComposeAccountHistoryTable);

            // Section break
            column.Item().ComposeSectionBreak();

            // Cumulative Totals Section
            column.Item().Element(ComposeCumulativeTotalsSection);

            // Section break
            column.Item().ComposeSectionBreak();

            // Legal Notice
            column.Item().Element(ComposeLegalNoticeSection);
        });
    }

    /// <summary>
    /// Composes the member profile section with contact information
    /// </summary>
    private void ComposeMemberProfileSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Member Profile", "#F0F0F0");

            column.Item().Container(c =>
            {
                c.Padding(PdfReportConfiguration.Spacing.StandardGap);

                c.Column(innerColumn =>
                {
                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Full Name", _memberProfile.FullName, bold: true);
                        row.RelativeColumn().ComposeKeyValuePair("Badge Number", _memberProfile.BadgeNumber.ToString());
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("SSN", _memberProfile.MaskedSsn);
                        row.RelativeColumn().ComposeKeyValuePair("Date of Birth", _memberProfile.DateOfBirth?.ToString("MM/dd/yyyy") ?? "N/A");
                    });

                    innerColumn.Item().ComposeDivider(0.5f);

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Address", _memberProfile.Address ?? "N/A");
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("City, State ZIP",
                            $"{_memberProfile.City}, {_memberProfile.State} {_memberProfile.ZipCode}".TrimEnd());
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Phone", _memberProfile.Phone ?? "N/A");
                    });

                    innerColumn.Item().ComposeDivider(0.5f);

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Hire Date", _memberProfile.HireDate?.ToString("MM/dd/yyyy") ?? "N/A");
                        row.RelativeColumn().ComposeKeyValuePair("Termination Date", _memberProfile.TerminationDate?.ToString("MM/dd/yyyy") ?? "Current Employee");
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Employment Status", _memberProfile.EmploymentStatus ?? "N/A");
                        row.RelativeColumn().ComposeKeyValuePair("Store Number", _memberProfile.StoreNumber?.ToString() ?? "N/A");
                    });
                });
            });
        });
    }

    /// <summary>
    /// Composes the report parameters section
    /// </summary>
    private void ComposeReportParametersSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ComposeSectionHeader("Report Parameters", "#F0F0F0");

            column.Item().Container(c =>
            {
                c.Padding(PdfReportConfiguration.Spacing.StandardGap);

                c.Column(innerColumn =>
                {
                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Report Period Start", _startDate.ToString("MM/dd/yyyy"));
                        row.RelativeColumn().ComposeKeyValuePair("Report Period End", _endDate.ToString("MM/dd/yyyy"));
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Prepared For", _preparedFor);
                        row.RelativeColumn().ComposeKeyValuePair("Total Years Reported", _accountHistory.Count.ToString());
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeKeyValuePair("Generated Date", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    });
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
                column.Item().Container(c =>
                {
                    c.Padding(PdfReportConfiguration.Spacing.StandardGap);
                    c.Text("No account activity found for the specified period.")
                        .FontSize(PdfReportConfiguration.FontSizes.ContentSize)
                        .FontColor(PdfReportConfiguration.BrandColors.TextDarkGray);
                });
                return;
            }

            column.Item().Container(c =>
            {
                c.Padding(PdfReportConfiguration.Spacing.StandardGap);

                c.Column(innerColumn =>
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

            column.Item().Container(c =>
            {
                c.Padding(PdfReportConfiguration.Spacing.StandardGap);

                c.Column(innerColumn =>
                {
                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeTotalsRow("Total Contributions", _cumulativeTotals.TotalContributions.ToCurrencyString());
                        row.RelativeColumn().ComposeTotalsRow("Total Earnings", _cumulativeTotals.TotalEarnings.ToCurrencyString());
                    });

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().ComposeTotalsRow("Total Forfeitures", _cumulativeTotals.TotalForfeitures.ToCurrencyString());
                        row.RelativeColumn().ComposeTotalsRow("Total Withdrawals", _cumulativeTotals.TotalWithdrawals.ToCurrencyString());
                    });

                    innerColumn.Item().ComposeDivider(1);

                    innerColumn.Item().Row(row =>
                    {
                        row.RelativeColumn().Element(el =>
                        {
                            el.Container(cont =>
                            {
                                cont.Padding(5);
                                cont.Background(PdfReportConfiguration.BrandColors.TotalsGray);
                                cont.Column(col =>
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
                });
            });
        });
    }

    /// <summary>
    /// Composes the legal notice section
    /// </summary>
    private void ComposeLegalNoticeSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Container(c =>
            {
                c.Padding(5);
                c.Background("#EEEEEE");
                c.BorderColor(PdfReportConfiguration.BrandColors.BorderGray);
                c.Border(1);

                c.Column(innerColumn =>
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
