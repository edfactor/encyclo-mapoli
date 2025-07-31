using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Report;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearEndProfitSharingReportRequest : BadgeNumberRequest
{
    [Description("The report type to generate. See YearEndProfitSharingReportId for options.")]
    public YearEndProfitSharingReportId ReportId { get; set; }
}
