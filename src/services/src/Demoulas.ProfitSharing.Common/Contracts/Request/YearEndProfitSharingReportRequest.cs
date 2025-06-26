using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Report;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearEndProfitSharingReportRequest : ProfitYearRequest
{
    [Description("The report type to generate. See YearEndProfitSharingReportId for options.")]
    public YearEndProfitSharingReportId ReportId { get; set; }
    public int? BadgeNumber { get; set; }
    // All other filtering properties removed for simplification
}
