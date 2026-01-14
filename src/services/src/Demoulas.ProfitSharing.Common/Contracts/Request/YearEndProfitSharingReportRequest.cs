using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Report;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearEndProfitSharingReportRequest : BadgeNumberRequest
{
    [Description("The report type to generate. See YearEndProfitSharingReportId for options.")]
    public YearEndProfitSharingReportId ReportId { get; set; }

    public static new YearEndProfitSharingReportRequest RequestExample()
    {
        return new YearEndProfitSharingReportRequest
        {
            ProfitYear = 2024,
            BadgeNumber = 123456,
            ReportId = YearEndProfitSharingReportId.Age21OrOlderWith1000Hours
        };
    }
}
