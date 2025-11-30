using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

public record QPay066Report
{
    public QPay066Report(string reportText)
    {
        members = QPay066ReportParser.ParseRecords(reportText).ToArray();
        totals = QPay066ReportParser.ParseTotals(reportText);

        // Do self check.
        QPay066Totals totalsComputed = QPay066ReportParser.ComputeTotals([.. members]);
        totals.ShouldBeEquivalentTo(totalsComputed);
    }

    private QPay066Totals totals { get; }
    private QPay066Record[] members { get; }
}
