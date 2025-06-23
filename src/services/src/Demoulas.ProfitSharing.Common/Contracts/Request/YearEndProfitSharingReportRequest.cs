using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearEndProfitSharingReportRequest : ProfitYearRequest
{
    public byte ReportId { get; set; }
    public int? BadgeNumber { get; set; }
    // All other filtering properties removed for simplification
}
